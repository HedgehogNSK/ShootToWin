using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;

namespace Shooter
{
    public class CustomNetworkManager : NetworkManager
    {
        const float RESPAWN_TIME = 3;
        Location.BattleField battleField;

        private void LoadBattleField()
        {
            battleField = Instantiate(spawnPrefabs[0]).GetComponent<Location.BattleField>();            
            battleField.Generate(20, 20);

        }        

        private bool IsBattleFieldLoaded => battleField != null;

        public override void OnStartServer()
        {
            LoadBattleField();
        }
        public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
        {
            
            GameObject player = Instantiate(playerPrefab, battleField.GetRandomPositionFree2Walk, playerPrefab.transform.rotation);
            NetworkServer.AddPlayerForConnection(conn, player);

            player.GetComponent<Player>().OnDead += RespawnPlayer;
        }

        private void RespawnPlayer(Player player)
        {
            StartCoroutine(RespawnPlayerCoroutine(RESPAWN_TIME, player));            

        }

        IEnumerator RespawnPlayerCoroutine(float time, Player player)
        {
            yield return new WaitForSecondsRealtime(time);
            player.gameObject.SetActive(true);
            player.SetPosition(battleField.GetRandomPositionFree2Walk);
            player.Initialize();
        }
    }

}
