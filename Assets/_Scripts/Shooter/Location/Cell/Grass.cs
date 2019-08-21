using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace Shooter.Location
{
    public class Grass : Cell
    {
        [SerializeField]float visibleDistance = 3;
        List<Collider> colliders = new List<Collider>();
       
        
        private void OnTriggerStay(Collider other)
        {
            Player player = other.GetComponent<Player>();
            if ( (!player || !player.isLocalPlayer) && !colliders.Contains(other))
            {
                colliders.Add(other);
            }

            if (colliders.Count > 0)
            {
                string s = "Colliders: ";
                foreach (var collider in colliders) s += name + ";";
                Debug.Log(s);
            }
           
        }


        private void OnTriggerExit(Collider other)
        {         
            colliders.Remove(other);
        }


        private void OnEnable()
        {
            Camera.onPreRender += CheckAndHide;
            Camera.onPostRender += MakeObjectsVisible;
        }

        private void OnDisable()
        {            
            Camera.onPreRender -= CheckAndHide;
            Camera.onPostRender -= MakeObjectsVisible;
        }



        private void CheckAndHide(Camera cam)
        {
            PlayerFollower follower = cam.GetComponent<PlayerFollower>();
            if (!follower) return;

            Player player = follower.ObservablePlayer;
           
            if (player && player.isLocalPlayer)
            {
                foreach (var collider in colliders)
                {
                    if ((player.transform.position - collider.transform.position).sqrMagnitude > visibleDistance * visibleDistance) 
                    SwitchComponentsActivity<Renderer>(collider, false);
                }

            }
        }

        private void MakeObjectsVisible(Camera cam)
        {
            PlayerFollower follower = cam.GetComponent<PlayerFollower>();
            if (!follower) return;

            foreach (var collider in colliders)
                {                    
                        SwitchComponentsActivity<Renderer>(collider, true);
                }

            
        }

        private static void SwitchComponentsActivity<T>(Collider collider, bool on) where T: Renderer
        {
            IEnumerable<T> renderers = collider.GetComponentsInChildren<T>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = on;
                
            }
        }
    }
}

