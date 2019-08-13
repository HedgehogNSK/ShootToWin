using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter {
    public class Dummy : Dweller, IHitable
    {
        public void Strike(HitInfo hit)
        {
            Debug.Log("[GameObject]:" +gameObject.name+" got " + hit.Damage + " damage");            
        }

        protected override void Die()
        {
            Destroy(gameObject);
        }


    }
}

