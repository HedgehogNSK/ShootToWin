using UnityEngine;
namespace Shooter.GameSettings
{
    [CreateAssetMenu(order = 753)]
    public class DwellerSettings : ScriptableObject
    {
#pragma warning disable CS0649
        [SerializeField]Dweller prefab;
        [SerializeField]private int health;
        [SerializeField] private float speed;
#pragma warning restore CS0649

        public Dweller Prefab => prefab;
        public int BaseHealth => health;
        public float BaseSpeed => speed;
    }
}