using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public abstract class Dweller : MonoBehaviour
    {
        [SerializeField] int health;
        public int Health => health;
        [SerializeField] float speed;
        public float Speed => speed;

        public void SetPosition(Vector3 newPosition)
        {
            transform.position = newPosition;
        }
    }
}