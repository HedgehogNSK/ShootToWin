using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

namespace Shooter
{
    public class CustomNetworkManager : NetworkManager
    {
       
        Location.BattleField battleField;
        List<Player> players = new List<Player>();

      
        public override void Start()
        {           
            ConnectionStatus status = ConnectionSettings.GetConnectionStatus();

            switch (status)
            {
                case ConnectionStatus.Host: { StartHost(); } break;
                case ConnectionStatus.Client:
                    {
                        networkAddress = ConnectionSettings.GetIPMatchAdress();
                        StartClient();
                    } break;
                default: { Debug.LogError("There is no behaviour for this connection status"); } break;
            }

        }

        public override void OnStartServer()
        {
            LoadBattleField();
        }

        public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
        {
            Player player = LevelFactory.CreatePlayer(playerPrefab.GetComponent<Player>(), battleField);
            NetworkServer.AddPlayerForConnection(conn, player.gameObject);
        }

        private void LoadBattleField()
        {
            battleField = LevelFactory.CreateLocation(spawnPrefabs[0].GetComponent<Location.BattleField>());
        }
        void StopGame()
        {
            if (NetworkServer.active) StopHost();
            else StopClient();
            GoToMenu();
        }

        void GoToMenu()
        {
            SceneManager.LoadSceneAsync(0);
        }

        private void OnGUI()
        {
            int areaWidth = 150;
            int areaHeight = 150;
            int topPadding = 20;
            
            GUILayout.BeginArea(new Rect((Screen.width - areaWidth) / 2, topPadding, areaWidth, areaHeight));

            if (NetworkServer.active)
            {
                if (GUILayout.Button("Stop Game"))
                {
                    StopGame();
                }

            }
            else
            {
                if (GUILayout.Button("Back to menu"))
                {
                    GoToMenu();
                }
            }
            GUILayout.EndArea();
        }

    }

}
