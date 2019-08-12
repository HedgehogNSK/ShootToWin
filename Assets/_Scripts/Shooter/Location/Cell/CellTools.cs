using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Location
{
    static public class CellTools
    {
        static int cellTypesAmount;
        static CellTools()
        {
            cellTypesAmount = System.Enum.GetValues(typeof(CellType)).Length;
        }
        static public CellType RandomCellType => (CellType)Random.Range(0, cellTypesAmount);
    }
}