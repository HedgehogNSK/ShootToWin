using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public class HitArgsBuilder 
    {
        private HitArgs hit;

        public HitArgsBuilder()
        {
            hit = new HitArgs();

        }

        public HitArgsBuilder SetDamage(Weapon weapon)
        {
            if (weapon == null)
            {
                Debug.LogError("Weapon equals null");                
            }
            else
            hit._Weapon =weapon;
            return this;
        }

        public HitArgsBuilder SetDirection(Vector3 direction)
        {
            hit.Direction = direction;
            return this;
        }

        public HitArgsBuilder SetAttacker(IAttacker attacker)
        {
            hit.Attacker = attacker;
            return this;
        }

        public HitArgsBuilder SetDistance(float distance)
        {
            hit.Distance = distance;
            return this;
        }
        public static implicit operator HitArgs(HitArgsBuilder builder)
        {
            return builder.hit;
        }

    }
}