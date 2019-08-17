using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Shooter
{
    public abstract class Dweller : NetworkBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] int health;
        [SerializeField] float speed;
#pragma warning restore CS0649
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

        public float Speed => speed;

        public void SetPosition(Vector3 newPosition)
        {
            transform.position = newPosition;
        }

        protected abstract void Die();
    }
}