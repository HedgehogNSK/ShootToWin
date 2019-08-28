﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    [System.Serializable]
    public class HitArgs : System.EventArgs
    {
      
        public Weapon _Weapon { get; set; }
        public Vector3 Direction { get; set; }
        public float Distance { get; set; }
        public IAttacker Attacker { get; set; }


        public static HitArgsBuilder CreateBuilder()
        {
            return new HitArgsBuilder();
        }
    }
}