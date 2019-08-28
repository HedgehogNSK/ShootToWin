using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.GameSettings
{
    [System.Serializable]
    public struct LocationSize
    {
        [SerializeField]int width;
        [SerializeField]int depth;

        public int Width => width;
        public int Depth => depth;
        public LocationSize(int width, int depth)
        {
            this.width = width;
            this.depth = depth;
        }

        

    }
}
