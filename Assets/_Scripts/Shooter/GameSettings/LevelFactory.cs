using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    using Location;
    using GameSettings;
    static public class LevelFactory
    {
        
        static public BattleField CreateLocation(LocationSettings locationSettings)
        {
            BattleField battleField = Object.Instantiate(locationSettings.LocationPrefab);            
            foreach(var cell in locationSettings.CellShareDictionary)
            { CellTools.SetCellShare(cell.Key, cell.Value); }            

            battleField.Generate(locationSettings.Size);
            return battleField;
        }

        static public BattleField CreateLocation(LevelSettings levelSettings)
        {
            return CreateLocation(levelSettings.LocationSettings);
        }

        static public Player CreatePlayer(LevelSettings levelSettings, BattleField battleField =null)
        {
            if (!(levelSettings.PlayerSettings.Prefab is Player))
            {
                Debug.LogError("Prefab of the Player wasn't set in LevelSettings");
                return null;
            }
            else
            {
                Player playerPrefab = (Player)levelSettings.PlayerSettings.Prefab;
                Vector3 position = battleField ? battleField.GetRandomPositionFree2Walk : playerPrefab.transform.position;
                Player player = Object.Instantiate(playerPrefab, position, playerPrefab.transform.rotation);
                
                player.OnDead += (x) =>
                {
                    player.RpcRespawnPlayer(position,levelSettings.PlayerSettings.RespawnTime);
                };
                return player;

            }
        }
    }

}
