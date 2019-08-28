﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

namespace Shooter
{
    public class PlayerFollower : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] Vector3 camOffset;
#pragma warning restore CS0649
        Camera cam;
        Player myPlayer;
        public Player ObservablePlayer => myPlayer;
        void Start()
        {
            cam = Camera.main;            
        }
        void LateUpdate()
        {
            MoveCamera();
        }

        void MoveCamera()
        {           
            if(!myPlayer)
            {
                myPlayer = FindObjectsOfType<Player>().SingleOrDefault(pl => pl.isLocalPlayer);                  
            }
            if (myPlayer)
                cam.transform.position = myPlayer.transform.position + camOffset;
        }


    }
}