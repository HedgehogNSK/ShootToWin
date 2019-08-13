using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]Camera cam;
        [SerializeField]Player player;
        [SerializeField] Vector3 camOffset;
        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main;
            player = FindObjectOfType<Player>();
        }

        // Update is called once per frame
        void Update()
        {
            MoveCamera();
        }
        private void Init()
        {
            
        }

        void MoveCamera()
        {
            cam.transform.position = player.transform.position +camOffset;
        }
    }
}