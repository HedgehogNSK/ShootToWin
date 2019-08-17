using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Shooter
{
    public class GameManager : NetworkBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] Camera cam;
        [SerializeField] Vector3 camOffset;
        [SerializeField] Location.BattleField battleFieldPrefab;
        [SerializeField] Player playerPrefab;
#pragma warning restore CS0649

        Location.BattleField battleField;
        Player myPlayer;
        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main;
            LoadGame();
        }

        // Update is called once per frame
        void Update()
        {
            MoveCamera();
        }
        private void LoadGame()
        {
            battleField = Instantiate(battleFieldPrefab);
            battleField.Generate(20,20);
            //myPlayer = Instantiate(playerPrefab);
            //myPlayer.SetPosition(battleField.GetRandomPositionFree2Walk);
        }

        private void UnloadGame()
        {
            Destroy(battleField);
            Destroy(myPlayer);
        }

        void MoveCamera()
        {
            if(myPlayer)
            cam.transform.position = myPlayer.transform.position +camOffset;
        }
    }
}