using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter {
    public class Dummy : Dweller, IHitable
    {
        public void GetStrike(HitArgs hit)
        {
            Debug.Log("[GameObject]:" +gameObject.name+" got " + hit._Weapon.Damage + " damage");            
        }

        public override void Initialize()
        {
            Health = baseHealth;
            Speed = baseSpeed;
        }

        protected override void Die()
        {
            Destroy(gameObject);
        }


    }
}

