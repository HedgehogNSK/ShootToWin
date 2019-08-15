using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public interface IHitable
    {
        void Strike(HitArgs hit);
    }
}

