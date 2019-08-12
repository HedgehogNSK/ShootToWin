using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hedge.Tools;

namespace Shooter.Location
{
   
    public class BattleField : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] CellDictionary cellDictionary;
#pragma warning restore CS0649
        Cell[,] battleCells;
        List<Cell> borderCells;
        Transform parentDir;

        void Start()
        {            
            Generate(10, 10);
        }

        public void Generate(int x, int y)
        {
            Initialize();

            //Creating Battle Field
            battleCells = new Cell[x,y];
            for(int i =0; i!=x; i++)
            {
                for(int j=0; j!=y; j++)
                {
                    cellDictionary.TryGetValue(CellTools.RandomCellByShare, out Cell value);
                    if (value != null)
                    {
                        battleCells[i, j] = Instantiate(value, new Vector3 (i,0,j),Quaternion.identity,parentDir);
                    }
                }
            }

            //Creating Borders
            int borderWallsAmount = 2 * x + 2 * y - 4;           
            {
                
                for (int i = 0; i != x; i++)
                {
                    borderCells.Add(Instantiate(cellDictionary[CellType.Wall], new Vector3(x, 0, i), Quaternion.identity, parentDir));
                    borderCells.Add(Instantiate(cellDictionary[CellType.Wall], new Vector3(i, 0, -1), Quaternion.identity, parentDir));
                }
                for (int i = -1; i != y+1; i++)
                {
                    borderCells.Add(Instantiate(cellDictionary[CellType.Wall], new Vector3(i, 0, y), Quaternion.identity, parentDir));
                    borderCells.Add(Instantiate(cellDictionary[CellType.Wall], new Vector3(-1, 0, i), Quaternion.identity, parentDir));
                }
               
            }
        }

        //TODO: ADD SHARES FOR CELLS ON MAP
        void Initialize()
        {
            if (parentDir != null)
                Destroy(parentDir);
            if (borderCells == null)
                borderCells = new List<Cell>();
            else
                borderCells.Clear();

            parentDir = new GameObject("Location").transform;
        }

    }
}