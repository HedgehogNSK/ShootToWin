using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers.Mobile;
using Hedge.UI;
using Mirror;
using UnityEngine.SceneManagement;
using System.Linq;
using Hedge.Tools;
namespace Shooter
{
    [RequireComponent(typeof(Rigidbody), typeof(NetworkIdentity))]
    sealed public class Player : Dweller, IHitable, IAttacker
    {
        const float RESPAWN_TIME = 3;
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
            Frags = 0;
        }

        [Command]
        public void CmdOnStartInitialize()
        {
            RpcInitialize();

        }

        [ClientRpc]
        public void RpcInitialize()
        {
            Initialize();
        }

        public override void Initialize()
        {
            Speed = baseSpeed;
            Health = baseHealth;
            lookRotation = transform.rotation;
            movementDirection = Vector3.zero;

        }

        private void SetWeapon()
        {
            if (!weapon)
            { weapon = Instantiate(weaponPrefab, hand); }
        }
        private void Start()
        {
            SetWeapon();
            ConnectControllers(true);

            if (isLocalPlayer)
            {
                CmdOnStartInitialize();
            }

        }

        private void FixedUpdate()
        {
            Move();
            Rotate();
#if MOUSE
            if (Input.GetKey(KeyCode.Mouse0))
                CmdAttack();            
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
        public LayerMask layerMask;
#endif
        private void Rotate()
        {

#if MOUSE
              if (!isLocalPlayer) return;
            Camera cam = Camera.main;            

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out RaycastHit raycastHit,100,layerMask,QueryTriggerInteraction.Ignore))
            {
                lookRotation = Quaternion.LookRotation(raycastHit.point.XZ() - transform.position.XZ(), Vector3.up);
               
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
            RpcAttack();
        }

        [ClientRpc]
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
                HitAnimation(hit);
            }
        }

        [ClientRpc]
        public void RpcRespawnPlayer(NetworkIdentity Identity, Vector3 position)
        {
            Player player = Identity.GetComponent<Player>();
            if (player)
                player.SetPosition(position);

            Location.BattleField battleField = FindObjectOfType<Location.BattleField>();
            if (battleField)
                battleField.StartCoroutine(RespawnPlayerCoroutine(RESPAWN_TIME, Identity));
        }

        IEnumerator RespawnPlayerCoroutine(float time, NetworkIdentity identity)
        {
            yield return new WaitForSecondsRealtime(time);
            Start();
            identity.gameObject.SetActive(true);
            
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

        private void HitAnimation(HitArgs hit)
        {
            if (hit._Weapon.HitParticles != null)
            {
                ParticleSystem particle = Instantiate(hit._Weapon.HitParticles);
                particle.transform.localPosition = transform.position - hit.Direction * 0.1f;
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

