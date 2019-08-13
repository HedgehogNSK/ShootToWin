using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public abstract class Dweller : MonoBehaviour
    {
        [SerializeField] int health;
        public int Health { get => health;
            protected set
            {
                if (value <= 0)
                {
                    health = 0;
                    Die();
                }
                else
                {
                    health = value;
                }
            } }
        [SerializeField] float speed;
        public float Speed => speed;

        public void SetPosition(Vector3 newPosition)
        {
            transform.position = newPosition;
        }

        protected abstract void Die();
    }
}