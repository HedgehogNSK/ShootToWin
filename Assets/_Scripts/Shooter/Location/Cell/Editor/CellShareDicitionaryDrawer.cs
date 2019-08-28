using UnityEditor;

namespace Shooter.Location
{
    [CustomPropertyDrawer(typeof(CellShareDictionary))]
    public class CellShareDictionaryDrawer : DictionaryDrawer<CellType, int> { }
}
