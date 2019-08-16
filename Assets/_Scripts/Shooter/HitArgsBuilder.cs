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

        public HitArgsBuilder SetDamage(int damage)
        {
            hit.Damage = damage > 0 ? damage:0 ;
            return this;
        }

        public HitArgsBuilder SetDirection(Vector3 direction)
        {
            hit.Direction = direction;
            return this;
        }

        public HitArgsBuilder SetAttacker(Dweller attacker)
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