using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public class HitInfo
    {
        int damage;
        Vector3 direction;
        float distance;
        public int Damage => damage;
        public Vector3 HitDirection => direction;
        public float Distance => distance;

        //TODO: Fluent builder
        public HitInfo(int damage)
        {
            this.damage = damage;
        }

        public HitInfo(Vector3 direction, int damage) : this(damage)
        {
            this.direction = direction;
        }
    }
}