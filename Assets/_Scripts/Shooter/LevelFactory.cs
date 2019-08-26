using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    using Location;
    static public class LevelFactory
    {
        static public BattleField CreateLocation(BattleField locationPrefab)
        {
            BattleField battleField = Object.Instantiate(locationPrefab);

            CellTools.SetCellShare(CellType.Ground, 100);
            CellTools.SetCellShare(CellType.Grass, 20);
            CellTools.SetCellShare(CellType.Water, 20);
            CellTools.SetCellShare(CellType.Wall, 10);

            battleField.Generate(5, 5);
            return battleField;
        }

        static public Player CreatePlayer(Player playerPrefab, BattleField battleField)
        {
            Player player = Object.Instantiate(playerPrefab, battleField.GetRandomPositionFree2Walk, playerPrefab.transform.rotation);
            player.OnDead += (x) =>
            {
                player.RpcRespawnPlayer(x.netIdentity, battleField.GetRandomPositionFree2Walk);
            };
            return player;
        }
    }

}
