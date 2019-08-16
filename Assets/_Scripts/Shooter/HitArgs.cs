using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public class HitArgs : System.EventArgs
    {
      
        public int Damage { get; set; }
        public Vector3 Direction { get; set; }
        public float Distance { get; set; }
        public Dweller Attacker { get; set; }


        public static HitArgsBuilder CreateBuilder()
        {
            return new HitArgsBuilder();
        }
    }
}