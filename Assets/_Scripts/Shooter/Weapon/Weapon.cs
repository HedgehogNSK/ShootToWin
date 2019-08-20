using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public abstract class Weapon: MonoBehaviour
    {
        public abstract float ReloadTime { get; }
        public abstract int Damage { get; }        
        public abstract float AttackDispersion { get; }
        public abstract float Range { get; }
        public abstract ParticleSystem HitParticles { get; }
        public abstract void Attack(IAttacker attacker,Vector3 direction);
    }
}

