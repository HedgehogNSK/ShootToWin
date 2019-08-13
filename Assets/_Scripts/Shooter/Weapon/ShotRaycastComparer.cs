using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public class ShotRaycastComparer : IEqualityComparer<RaycastHit>
    {
        public bool Equals(RaycastHit x, RaycastHit y)
        {
            if (x.collider.gameObject.Equals(y.collider.gameObject)) return true;
            return false;
        }

        public int GetHashCode(RaycastHit obj)
        {
            
            return obj.collider.GetHashCode()+obj.collider.gameObject.GetHashCode(); 
        }
    }
}