﻿using UnityEngine;
using Mirror;
namespace Shooter
{
    public abstract class Dweller : NetworkBehaviour
    {
#pragma warning disable CS0649
        [SerializeField]protected int baseHealth;
        [SerializeField]protected float baseSpeed;
#pragma warning restore CS0649
        [SyncVar]int health;
        [SyncVar]float speed;
        public virtual int Health
        {
            get => health;

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
            }
        }

        public virtual float Speed
        {
            get => speed;
            protected set
            {
                speed = value;
            }
        }

        public virtual void BasicSetup(GameSettings.DwellerSettings settings)
        {
            baseHealth = settings.BaseHealth;
            baseSpeed = settings.BaseSpeed;
        }
        public void SetPosition(Vector3 newPosition)
        {
            transform.position = newPosition;
        }
        public abstract void Initialize();

        protected abstract void Die();
    }
}