using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public interface IAttacker
    {
        int Frags { get; }
        void AddKill(IHitable target);
    }
}

