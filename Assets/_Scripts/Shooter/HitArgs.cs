using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public class HitArgs : System.EventArgs
    {
        int damage;
        Vector3 direction;
        float distance;
        Dweller attacker;

        public int Damage => damage;
        public Vector3 HitDirection => direction;
        public float Distance => distance;
        public Dweller Attacker => attacker;

        //TODO: Fluent builder
        public HitArgs(int damage)
        {
            this.damage = damage;
        }

        public HitArgs(Vector3 direction, int damage) : this(damage)
        {
            this.direction = direction;
        }

        public HitArgs(Dweller attacker, Vector3 direction, int damage) :  this(direction, damage)
        {
            this.attacker = attacker;
        }
    }
}