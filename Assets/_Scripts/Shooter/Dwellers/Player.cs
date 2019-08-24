using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers.Mobile;
using Hedge.UI;
using Mirror;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Shooter
{
    [RequireComponent(typeof(Rigidbody), typeof(NetworkIdentity))]
    sealed public class Player : Dweller, IHitable, IAttacker
    {
#pragma warning disable CS0649

        public Transform hand;
        [SerializeField] Weapon weaponPrefab;
#pragma warning restore CS0649

        public Weapon weapon { get; private set; }

        Rigidbody rigid;
        new Collider collider;
        Vector3 movementDirection = Vector3.zero;
        Quaternion lookRotation = Quaternion.identity;

        [SyncVar] int frags = 10;
        public int Frags
        {
            get { return frags; }
            set
            {
                if (value >= 0)
                {
                    frags = value;
                    DataSpreader.OnUpdate?.Invoke(DataType.Points, frags);

                }
                else
                    Debug.LogError("Amount of frags can't be negative");

            }
        }

        public override int Health
        {
            get => base.Health;
            protected set
            {
                base.Health = value;
                if (isLocalPlayer)
                {
                    DataSpreader.OnUpdate?.Invoke(DataType.Health, Health);
                }
            }
        }

        public event Action<Player> OnDead;

        void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            weapon = GetComponentInChildren<Gun>();
            collider = GetComponent<Collider>();
            
        }

        [Command]
        public void CmdOnStartInitialize()
        {
            Initialize();
            Frags = 0;
        }

        public override void Initialize()
        {
            Speed = baseSpeed;
            Health = baseHealth;
            TransformDataSpreader.ForceSetMaxParameter(DataType.Health, Health);
            lookRotation = transform.rotation;
            movementDirection = Vector3.zero;
            
        }

        [TargetRpc]
        public void TargetGetWeapon(NetworkConnection networkConnection,NetworkIdentity weaponIdentity)
        {
            
            Debug.Log(weaponIdentity.name + " " + hand);
            Weapon weaponToUpdate = weaponIdentity.GetComponent<Weapon>();
            Debug.Log("[GameObject]" + gameObject.name + transform.position + " " + isLocalPlayer + " :" + weaponToUpdate);
            weaponToUpdate.transform.SetParent(hand);
            weaponToUpdate.transform.localPosition = weaponPrefab.transform.position;
            weaponToUpdate.transform.localRotation = weaponPrefab.transform.rotation;
            weaponToUpdate.transform.localScale = weaponPrefab.transform.lossyScale;
            weapon = weaponToUpdate;
        }

        [Command]
        public void CmdSetWeapon()
        {
            if (!weapon)
            {
                weapon = Instantiate(weaponPrefab, hand);
                NetworkServer.Spawn(weapon.gameObject);
            }
           
            TargetGetWeapon(connectionToClient,weapon.netIdentity);

        }

        private void Start()
        {
            ConnectControllers(true);

            if (isLocalPlayer)
            {
                Initialize();
                Frags = 0;
            }


            Debug.Log("[GameObject]+"+name+"("+GetInstanceID()+")Has Autorithy: "+this.hasAuthority);
            if (hasAuthority)
            {
                CmdSetWeapon();
            }
            
        }

        private void FixedUpdate()
        {
            Move();
            Rotate();
#if KEYBOARD
            if (Input.GetKeyDown(KeyCode.Mouse0))
                Attack();
            if(Input.GetKeyUp(KeyCode.Minus))                          
                Health -= 20;
            
#endif 
        }

        private void Move()
        {

#if KEYBOARD
              if (!isLocalPlayer) return;
            movementDirection = Vector3.zero;
            if (Input.GetKey(KeyCode.UpArrow)) movementDirection += Vector3.forward;
            if (Input.GetKey(KeyCode.DownArrow)) movementDirection += Vector3.back;
            if (Input.GetKey(KeyCode.LeftArrow)) movementDirection += Vector3.left;
            if (Input.GetKey(KeyCode.RightArrow)) movementDirection += Vector3.right;

             
#endif

            rigid.MovePosition(Speed * movementDirection.normalized * Time.fixedDeltaTime + rigid.position);

        }

#if MOUSE
        LayerMask layerMask;
#endif
        private void Rotate()
        {

#if MOUSE
              if (!isLocalPlayer) return;
            Camera cam = Camera.main;            

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out RaycastHit raycastHit,20,layerMask,QueryTriggerInteraction.Ignore))
            {
                movementRotation = Quaternion.LookRotation(raycastHit.point.XZ() + transform.position.Y(), Vector3.up);
            }
#endif
            rigid.MoveRotation(lookRotation);
        }
        public void SetMoveDirection(Joystick joystick, Vector2 direction)
        {
            movementDirection = new Vector3(direction.x, 0, direction.y).normalized;
        }

        public void TakeAim(Joystick joystick, Vector2 forward, bool fire)
        {

            if (fire) CmdAttack();
            else
                SetRotation(forward);
        }
        void SetRotation(Vector2 forward)
        {
            lookRotation = Quaternion.LookRotation(new Vector3(forward.x, 0, forward.y), Vector3.up);

        }

        [Command]
        void CmdAttack()
        {
            if (weapon == null) return;
            weapon.Attack(this, transform.forward);
        }

        public void GetStrike(HitArgs hit)
        {
            int healthBefore = Health;
            Health -= hit._Weapon.Damage;
            if (healthBefore > 0 && Health <= 0 && hit.Attacker != null)
            {
                hit.Attacker.AddKill(this);
            }
            else
            {
                CmdHitAnimation(hit);
            }


        }
       

        protected override void Die()
        {
            ConnectControllers(false);
            DieAnimation();
            OnDead?.Invoke(this);
            

        }

        public void AddKill(IHitable target)
        {
            Frags++;
        }

        void ConnectControllers(bool connect)
        {
            if (!isLocalPlayer) return;
            if (connect)
            {
                SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
                MoveJoystick.OnMove += SetMoveDirection;
                AttackJoystick.OnAim += TakeAim;
                

            }
            else
            {
                SceneManager.UnloadSceneAsync(1);
                MoveJoystick.OnMove -= SetMoveDirection;
                AttackJoystick.OnAim -= TakeAim;
            }
        }


        [Command]
        private void CmdHitAnimation(HitArgs hit)
        {
            if (hit._Weapon.HitParticles != null)
            {
                ParticleSystem particle = Instantiate(hit._Weapon.HitParticles);
                particle.transform.localPosition = transform.position - hit.Direction * 0.1f;
                NetworkServer.Spawn(particle.gameObject);
                Destroy(particle.gameObject, particle.main.duration);
            }
        }
        void DieAnimation()
        {
            gameObject.SetActive(false);
        }

        public void OnDisable()
        {
            ConnectControllers(false);
        }
    }
}

