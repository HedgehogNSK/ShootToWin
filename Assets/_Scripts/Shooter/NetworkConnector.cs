using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

namespace Shooter
{
    [RequireComponent(typeof(NetworkManager))]
    public class NetworkConnector : MonoBehaviour
    {
        NetworkManager manager;
        private void Awake()
        {
            manager = NetworkManager.singleton;
        }

       

    }
}