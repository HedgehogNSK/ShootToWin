using UnityEditor;

namespace Shooter.Location
{
    [CustomPropertyDrawer(typeof(CellDictionary))]
    public class CellDictionaryDrawer : DictionaryDrawer<CellType, Cell> { }
}
