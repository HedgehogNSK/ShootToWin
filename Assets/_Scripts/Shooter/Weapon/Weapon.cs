using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public interface IWeapon
    {
        float ReloadTime { get; }
        int Damage { get; }        
        float AttackDispersion { get; }
        float Range { get; }
        void Attack(Vector3 direction);
    }
}

