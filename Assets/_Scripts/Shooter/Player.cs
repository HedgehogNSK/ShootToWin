using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hedge.Tools;

namespace Shooter
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : Dweller, IHitable
    {
        [SerializeField] float damage = 20;
        public float Damage => damage;
        Rigidbody rigid;
        IWeapon weapon;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            weapon = GetComponentInChildren<Gun>();


        }

        private void FixedUpdate()
        {
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
            Vector3 movementDirection = Vector3.zero;
            if (Input.GetKey(KeyCode.UpArrow)) movementDirection += Vector3.forward;
            if (Input.GetKey(KeyCode.DownArrow)) movementDirection += Vector3.back;
            if (Input.GetKey(KeyCode.LeftArrow)) movementDirection += Vector3.left;
            if (Input.GetKey(KeyCode.RightArrow)) movementDirection += Vector3.right;

            rigid.MovePosition(Speed * movementDirection.normalized * Time.fixedDeltaTime + rigid.position);
#else
            rigid.velocity = Speed * direction;
#endif
        }

        [SerializeField]LayerMask layerMask;
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
        public void SetDirection(Vector3 joystickDirection)
        {

        }

        public void Attack()
        {

            if (weapon==null) return;
            weapon.Attack(transform.forward);
        }

        public void Strike(HitInfo hit)
        {
            Health -= hit.Damage;
        }

        protected override void Die()
        {
            throw new NotImplementedException();
        }
    }
}

