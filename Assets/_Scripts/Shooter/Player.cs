using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : Dweller
    {
        [SerializeField] float damage = 20;
        public float Damage => damage;
        Rigidbody rigid;
        Vector3 moveDirection;
        Vector3 eyesDirection;
        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();        
        }

        private void FixedUpdate()
        {
            Move();
        }
       
        private void Move()
        {
#if KEYBOARD
            Vector3 currentDir = Vector3.zero;
            if (Input.GetKey(KeyCode.UpArrow)) currentDir += Vector3.forward;
            if (Input.GetKey(KeyCode.DownArrow)) currentDir += Vector3.back;
            if (Input.GetKey(KeyCode.LeftArrow)) currentDir += Vector3.left;
            if (Input.GetKey(KeyCode.RightArrow)) currentDir += Vector3.right;

            rigid.MovePosition(Speed * currentDir.normalized * Time.fixedDeltaTime + rigid.position);
#else
            rigid.velocity = Speed * direction;
#endif
        }

        public void SetDirection(Vector3 joystickDirection)
        {

        }



    }
}

