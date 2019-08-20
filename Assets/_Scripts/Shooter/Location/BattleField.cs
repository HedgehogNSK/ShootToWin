using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

namespace Shooter.Location
{
   
    public class BattleField : NetworkBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] CellDictionary cellDictionary;
#pragma warning restore CS0649
        Cell[,] battleCells;
        List<Cell> borderCells;

        Transform parentDir;


#if KEYBOARD
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            { Generate(height, width); }
        }

#endif

        public void Generate(int x, int y)
        {
            Initialize();

            
            //Creating Battle Field
            battleCells = new Cell[x,y];
            CellTools.SetCellShare(CellType.Ground, 100);
            CellTools.SetCellShare(CellType.Grass, 20);
            CellTools.SetCellShare(CellType.Water, 20);
            CellTools.SetCellShare(CellType.Wall, 10);
            for (int i =0; i!=x; i++)
            {
                for(int j=0; j!=y; j++)
                {
                    CellType randomCell = CellTools.RandomCellByShare;
                    cellDictionary.TryGetValue(randomCell, out Cell value);
                    if (value != null)
                    {
                        battleCells[i, j] = Instantiate(value, new Vector3 (i, value.transform.position.y, j),Quaternion.identity,parentDir);
                        NetworkServer.Spawn(battleCells[i, j].gameObject);
                        
                    }
                }
            }

            //Creating Borders
            int borderWallsAmount = 2 * x + 2 * y - 4;           
            {
                
                for (int i = 0; i != x; i++)
                {
                    borderCells.Add(Instantiate(cellDictionary[CellType.Wall], new Vector3(i, cellDictionary[CellType.Wall].transform.position.y, y), Quaternion.identity, parentDir));
                    borderCells.Add(Instantiate(cellDictionary[CellType.Wall], new Vector3(i, cellDictionary[CellType.Wall].transform.position.y, -1), Quaternion.identity, parentDir));
                }
                for (int i = -1; i != y+1; i++)
                {
                    
                    borderCells.Add(Instantiate(cellDictionary[CellType.Wall], new Vector3(x, cellDictionary[CellType.Wall].transform.position.y, i), Quaternion.identity, parentDir));
                    borderCells.Add(Instantiate(cellDictionary[CellType.Wall], new Vector3(-1,  cellDictionary[CellType.Wall].transform.position.y, i), Quaternion.identity, parentDir));
                }
               
            }
            foreach(var cell in borderCells)
                NetworkServer.Spawn(cell.gameObject);
        }
        
        void Initialize()
        {
            if (parentDir != null)
                Destroy(parentDir.gameObject);
            if (borderCells == null)
                borderCells = new List<Cell>();
            else
                borderCells.Clear();
            NetworkServer.Spawn(gameObject);
            parentDir = transform;
        }
        
        public Vector3 GetRandomPositionFree2Walk {
            get
            {
                if (battleCells == null)
                {
                    Debug.LogError("Battle Cells aren't been created yet");
                    return Vector3.zero;
                }

                List<Cell> groundCells = new List<Cell>();
                for (int i =0; i!= battleCells.GetLength(0); i++)
                {
                    for(int j=0; j!= battleCells.GetLength(1);j++)
                    {
                        if (battleCells[i, j] is Ground)
                            groundCells.Add(battleCells[i,j]);
                    }

                }               
                int rand = Random.Range(0, groundCells.Count);
                return groundCells[rand].transform.position;
            }
        }

    }
}