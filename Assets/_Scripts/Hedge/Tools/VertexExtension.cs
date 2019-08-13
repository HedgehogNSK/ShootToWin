using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hedge.Tools {
    static public class VertexExtension
    {
        public static Vector3 RotateAroundY(this Vector3 vector, float angle)
        {
            float rad = Mathf.Deg2Rad * angle;
            float x = vector.x * Mathf.Cos(rad) - vector.z * Mathf.Sin(rad);
            float z = vector.x * Mathf.Sin(rad) + vector.z * Mathf.Cos(rad);
            return new Vector3(x, vector.y, z);
        }
    }
}

