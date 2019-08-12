using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Shooter.Location
{
    static public class CellTools
    {
        static int cellTypesAmount;
        static Dictionary<CellType, int> cellShares;  
        static int cacheCellShareSum =0;
        static CellTools()
        {
            System.Array cellTypeValues = System.Enum.GetValues(typeof(CellType));
            cellTypesAmount = cellTypeValues.Length;
            cellShares = new Dictionary<CellType, int>();
            foreach (var type in cellTypeValues)
            {
                cellShares.Add((CellType)type, 0);
            }
            cacheCellShareSum = cellShares.Sum(x => x.Value);
        }
        static public CellType RandomCellType => (CellType)Random.Range(0, cellTypesAmount);

        static public void SetCellShare(CellType type, int share)
        {
            if (cellShares.ContainsKey(type))
            {
                cellShares[type] = share;
                cacheCellShareSum = cellShares.Sum(x => x.Value);
            }
            else
                Debug.LogError("There is no key equals" + type.ToString() + " in share dictionary");
        }
        static public CellType RandomCellByShare
        {
            get
            {
                if (cacheCellShareSum == 0) return RandomCellType;

                int rand = Random.Range(0, cacheCellShareSum);
                int tmp = 0;
                foreach (var record in cellShares)
                {
                    tmp += record.Value;
                    if (rand <= tmp) return record.Key;
                }
                return RandomCellType;
            }
        }
        
    }
}