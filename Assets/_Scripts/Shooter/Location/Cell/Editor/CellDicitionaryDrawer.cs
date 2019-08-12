using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

using Hedge.Tools;
namespace Shooter.Location
{
    [CustomPropertyDrawer(typeof(CellDictionary))]
    public class CellDictionaryDrawer : DictionaryDrawer<CellType, Cell> { }
}
