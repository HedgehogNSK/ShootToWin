using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

namespace Shooter
{
    
    public class CustomNetworkManager : NetworkManager
    {
        private const string menuSceneName = "Menu";
#pragma warning disable CS0649
        [SerializeField] GameSettings.LevelSettings lvlSettings;
#pragma warning restore CS0649


        Location.BattleField battleField;
        List<Player> players = new List<Player>();
        public GameSettings.LevelSettings @LevelSettings => lvlSettings;
        public Location.BattleField @BattleField => battleField;
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
            Player player = LevelFactory.CreatePlayer(lvlSettings, battleField);
            NetworkServer.AddPlayerForConnection(conn, player.gameObject);
            
        }

        private void LoadBattleField()
        {
            battleField = LevelFactory.CreateLocation(lvlSettings);
            NetworkServer.Spawn(battleField.gameObject);
        }
        void StopGame()
        {
           StopHost();
           GoToMenu();
        }

        void GoToMenu()
        {
            SceneManager.LoadSceneAsync(menuSceneName);
        }

        private void OnGUI()
        {
            int areaWidth = 150;
            int areaHeight = 150;
            int topPadding = 20;
            
            GUILayout.BeginArea(new Rect((Screen.width - areaWidth) / 2, topPadding, areaWidth, areaHeight));

            if (NetworkServer.active || NetworkClient.isConnected)
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
