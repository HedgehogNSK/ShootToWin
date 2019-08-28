using UnityEngine;


namespace Shooter.GameSettings
{
    using Location;
    [CreateAssetMenu(order = 752)]
    public class LocationSettings : ScriptableObject
    {
#pragma warning disable CS0649
       [SerializeField] BattleField locationPrefab;
       [SerializeField] LocationSize size;
       [SerializeField] CellShareDictionary cellShareDictionary;
#pragma warning restore CS0649

        public BattleField LocationPrefab => locationPrefab;
        public LocationSize Size => size;
        public CellShareDictionary @CellShareDictionary => cellShareDictionary;

    }

}
