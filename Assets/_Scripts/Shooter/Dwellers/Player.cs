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


        int frags;
        public int Frags
        {
            get { return frags; }
            set
            {
                if (value >= 0)
                {
                    frags = value;
                    CounterText.Update?.Invoke(TextType.Points, frags) ;
                    
            }
                else
                    Debug.LogError("Amount of frags can't be negative");
                
            }
        }

        public event Action<IHitable> OnDead;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            weapon = GetComponentInChildren<Gun>();
            collider = GetComponent<Collider>();

            weapon = Instantiate(weaponPrefab,hand);

           MoveJoystick.OnMove += SetMoveDirection;
           AttackJoystick.OnAim += TakeAim;
           Frags = 0;

        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
                return;

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
            MoveJoystick.OnMove -= SetMoveDirection;
            AttackJoystick.OnAim -= TakeAim;
        }
        protected override void Die()
        {           
            OnDead?.Invoke(this);
            DieAnimation();

        }
        void DieAnimation()
        {
            Destroy(gameObject);
        }
        public void AddKill(IHitable target)
        {
            Frags++;
        }
    }
}

