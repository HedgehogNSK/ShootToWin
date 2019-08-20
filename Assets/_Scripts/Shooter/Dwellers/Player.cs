using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers.Mobile;
using Hedge.UI;
using Mirror;

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


        [SyncVar]int frags =10;
        public int Frags
        {
            get { return frags; }
            set
            {
                if (value >= 0)
                {
                    frags = value;
                    CounterLogger.OnUpdate?.Invoke(CounterType.Points, frags) ;
                    
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
                    Debug.Log(gameObject.name + "[" + gameObject.GetInstanceID() + "] :Downgrade HP");
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
            Frags = 0;
            ConnectControllers(true);
        }
        void Start()
        {
            if (isLocalPlayer)
            {
                Initialize(); 
            }
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;
            if(Input.GetKeyUp(KeyCode.Minus))
            {               
                Health -= 20;
            }
            Move();
            Rotate();
#if KEYBOARD
            if (Input.GetKeyDown(KeyCode.Mouse0))
                Attack();
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

        private void Rotate()
        {

#if KEYBOARD
            Camera cam = Camera.main;            

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out RaycastHit raycastHit,20,layerMask,QueryTriggerInteraction.Ignore))
            {
                transform.LookAt(raycastHit.point.XZ()+transform.position.Y(), Vector3.up);
            }
#endif
        }
        public void SetMoveDirection(Joystick joystick, Vector2 direction)
        {
            movementDirection = new Vector3(direction.x,0,direction.y).normalized;
        }

        public void TakeAim(Joystick joystick,Vector2 forward,bool fire)
        {
            
            if (fire) Attack();
            else
                SetRotation(forward);
        }
        void SetRotation( Vector2 forward)
        {
            
            rigid.MoveRotation(Quaternion.LookRotation(new Vector3(forward.x, 0, forward.y),Vector3.up));
        }

        
        void Attack()
        {

            if (weapon==null) return;            
            weapon.Attack(this, transform.forward);
        }

        public void GetStrike(HitArgs hit)
        {
            int healthBefore = Health;
            Health -= hit.Damage;      
            if (healthBefore>0 && Health <= 0 && hit.Attacker != null)
            {
                hit.Attacker.AddKill(this);
            }
                
        }

        public void OnDestroy()
        {
            ConnectControllers(false);
        }
        protected override void Die()
        {
            ConnectControllers(false);

            OnDead?.Invoke(this);
            DieAnimation();

        }
        void DieAnimation()
        {
            gameObject.SetActive(false);
        }
        public void AddKill(IHitable target)
        {
            Frags++;
        }

        void ConnectControllers(bool connect)
        {
            if (connect)
            {
                MoveJoystick.OnMove += SetMoveDirection;
                AttackJoystick.OnAim += TakeAim;
            }
            else
            {
                MoveJoystick.OnMove -= SetMoveDirection;
                AttackJoystick.OnAim -= TakeAim;
            }
        }
               
        
    }
}

