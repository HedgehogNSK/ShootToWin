using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

namespace Shooter
{
    public class GameManager : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] Camera cam;
        [SerializeField] Vector3 camOffset;
#pragma warning restore CS0649

        Player myPlayer;
        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main;            
        }

        // Update is called once per frame
        void Update()
        {
            MoveCamera();
        }

        private void UnloadGame()
        {
            Destroy(myPlayer);
        }

        void MoveCamera()
        {
            if(myPlayer)
            cam.transform.position = myPlayer.transform.position +camOffset;
            else
            {
                myPlayer = FindObjectsOfType<Player>().SingleOrDefault(pl => pl.isLocalPlayer);
            }
        }
    }
}