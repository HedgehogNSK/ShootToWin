using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Location
{
   
    public class BattleField : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] Cell floorPrefab;
        [SerializeField] Cell groundPrefab;
        [SerializeField] Cell waterPrefab;
        [SerializeField] Cell grassPrefab;
        [SerializeField] Cell cellPrefabs;
#pragma warning restore CS0649
        Cell[,] battleCells;
        Transform parentDir;

        Dictionary<CellType, Cell> cellCatalog;
        private void Awake()
        {
            cellCatalog = new Dictionary<CellType, Cell>();
            foreach(var type in System.Enum.GetValues(typeof(CellType)))
            {
                //System.Enum.TryParse<CellType>()
                //cellCatalog.Add(type,)
            }
        }
        void Start()
        {
            if (!parentDir)
            {
                parentDir = Instantiate(new GameObject("Location")).transform;
            }
        }

        public void Generate(int x, int y)
        {
            battleCells = new Cell[x,y];
            for(int i =0; i!=x; i++)
            {
                for(int j=0; j!=y; j++)
                {
                    battleCells[i, j] = Instantiate(cellPrefab);
                }
            }
        }
    }
}