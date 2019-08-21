using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers.Mobile;
using Hedge.UI;
using Hedge.Tools;
using Mirror;
using UnityEngine.SceneManagement;

namespace Shooter
{
    [RequireComponent(typeof(Rigidbody))]
    sealed public class Player : Dweller, IHitable, IAttacker
    {
#pragma warning disable CS0649

        [SerializeField] Transform hand;
        [SerializeField] Weapon weaponPrefab;
#pragma warning restore CS0649

        Weapon weapon;

        Rigidbody rigid;
        new Collider collider;
        Vector3 movementDirection;
        Quaternion movementRotation;

        [SyncVar] int frags = 10;
        public int Frags
        {
            get { return frags; }
            set
            {
                if (value >= 0)
                {
                    frags = value;
                    CounterLogger.OnUpdate?.Invoke(CounterType.Points, frags);

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
                    CounterLogger.OnUpdate?.Invoke(CounterType.Health, Health);
                }
            }
        }

        public event Action<Player> OnDead;

        void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            weapon = GetComponentInChildren<Gun>();
            collider = GetComponent<Collider>();
            weapon = Instantiate(weaponPrefab, hand);
        }

        public override void Initialize()
        {
            Speed = baseSpeed;
            Health = baseHealth;           
            movementRotation = transform.rotation;
        }

     
        public void SetWeapon()
        {
            
            NetworkServer.Spawn(weapon.gameObject);
        }
        
        private void Start()
        {
            Initialize();           
            ConnectControllers(true);
            Frags = 0;
        }
        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;

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
            Camera cam = Camera.main;            

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out RaycastHit raycastHit,20,layerMask,QueryTriggerInteraction.Ignore))
            {
                movementRotation = Quaternion.LookRotation(raycastHit.point.XZ() + transform.position.Y(), Vector3.up);
            }
#endif
            rigid.MoveRotation(movementRotation);  
        }
        public void SetMoveDirection(Joystick joystick, Vector2 direction)
        {
            movementDirection = new Vector3(direction.x, 0, direction.y).normalized;
        }

        public void TakeAim(Joystick joystick, Vector2 forward, bool fire)
        {

            if (fire) RpcAttack();
            else
                SetRotation(forward);
        }
        void SetRotation(Vector2 forward)
        {
            movementRotation = Quaternion.LookRotation(new Vector3(forward.x, 0, forward.y), Vector3.up);
           
        }

        [ClientRpcAttribute]
        void RpcAttack()
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

            OnDead?.Invoke(this);
            DieAnimation();

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
                particle.transform.localPosition = transform.position- hit.Direction*0.1f;
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

