using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.GameSettings
{
    [CreateAssetMenu(order = 751)]
    public class LevelSettings : ScriptableObject
    {
#pragma warning disable CS0649
        [SerializeField] LocationSettings locationSettings;
        [SerializeField] PlayerSettings playerSettings;
#pragma warning restore CS0649

        public LocationSettings @LocationSettings => locationSettings;
        public PlayerSettings PlayerSettings => playerSettings;
    }
}