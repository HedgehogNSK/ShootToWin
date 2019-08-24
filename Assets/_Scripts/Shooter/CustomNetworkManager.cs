using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Shooter
{
    public class CustomNetworkManager : NetworkManager
    {
       
        Location.BattleField battleField;
        List<Player> players = new List<Player>();
        private void LoadBattleField()
        {
            battleField = Instantiate(spawnPrefabs[0]).GetComponent<Location.BattleField>();
            NetworkServer.Spawn(battleField.gameObject);
            battleField.Generate(5, 5);
        }        

        private bool IsBattleFieldLoaded => battleField != null;

        public override void OnStartServer()
        {
            LoadBattleField();
        }
        public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
        {

            Player player = Instantiate(playerPrefab, battleField.GetRandomPositionFree2Walk, playerPrefab.transform.rotation).GetComponent<Player>();
            if (NetworkServer.AddPlayerForConnection(conn, player.gameObject))
            {
          
                player.OnDead += (x) => 
                {
                    player.RpcRespawnPlayer(x.netIdentity, battleField.GetRandomPositionFree2Walk);
                };
                players.Add(player);
            }
        }

        
        

    }

}
