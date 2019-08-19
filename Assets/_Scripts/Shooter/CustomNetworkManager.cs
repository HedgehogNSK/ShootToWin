using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
namespace Shooter
{
    public class CustomNetworkManager : NetworkManager
    {

        Location.BattleField battleField;

        private void LoadBattleField()
        {
            if (IsBattleFieldLoaded) return;

            battleField = Instantiate(spawnPrefabs[0]).GetComponent<Location.BattleField>();
            NetworkServer.Spawn(battleField.gameObject);
            battleField.Generate(20, 20);

        }        

        private bool IsBattleFieldLoaded => battleField != null;

        public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
        {

            LoadBattleField();            
            GameObject player = Instantiate(playerPrefab, battleField.GetRandomPositionFree2Walk, playerPrefab.transform.rotation);
            NetworkServer.AddPlayerForConnection(conn, player);

        }

       


    }

}
