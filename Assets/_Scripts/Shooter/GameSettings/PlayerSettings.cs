using UnityEngine;
namespace Shooter.GameSettings
{
    [CreateAssetMenu(order = 754)]
    public class PlayerSettings : DwellerSettings
    {
#pragma warning disable CS0649
        [SerializeField] private float respawnTime;
        [SerializeField] private Weapon weaponPrefab;
#pragma warning restore CS0649

        public float RespawnTime => respawnTime;
        public Weapon BaseWeaponPrefab => weaponPrefab;
    }
}