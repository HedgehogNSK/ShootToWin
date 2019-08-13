using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Hedge.Tools;
namespace Shooter
{
    public class Gun : MonoBehaviour, IWeapon
    {
        [SerializeField] float baseDamage = 20;
        [SerializeField] float baseReloadTime = 3;
        [SerializeField] float baseRange = 5;
        [SerializeField] float baseShotSpread = 60;
        [SerializeField]LayerMask layerMask;
        public float ReloadTime => baseReloadTime;
        public float Damage => baseDamage;
        public float AttackDispersion => baseShotSpread;
        public float Range => baseRange;

        //Step in degrees for Raycasting area in front of weapon
        float deltaAngle = 5;
        float lastShot;
        private void Awake()
        {
            lastShot = -ReloadTime;
        }
        public void Attack(Vector3 direction)
        {
            if (!IsAvailableToShoot) return;

            foreach(RaycastHit hit in GetAllHits(direction))
            {
                Debug.Log("Hit [GameObject]"+hit.collider.name);
                Destroy(hit.collider.gameObject);
            }

            Debug.Log("Bang Bang");
            lastShot = Time.time;
        }

        private IEnumerable<RaycastHit> GetAllHits(Vector3 direction)
        {
            Vector3 leftDirection, rightDirection;
           
            float currentAngle = deltaAngle;

            IEnumerable<RaycastHit> hits=  Physics.RaycastAll(transform.position, direction, Range, layerMask);
            if (AttackDispersion == 0) return hits;

            while (currentAngle < AttackDispersion)
            {
                leftDirection = direction.RotateAroundY(currentAngle);
                rightDirection = direction.RotateAroundY(-currentAngle);

                hits = hits.Union(Physics.RaycastAll(transform.position, leftDirection, Range, layerMask));
                hits = hits.Union(Physics.RaycastAll(transform.position, rightDirection, Range, layerMask));

                currentAngle += deltaAngle;
            }

            leftDirection = direction.RotateAroundY(AttackDispersion);
            rightDirection = direction.RotateAroundY(-AttackDispersion);
            hits = hits.Union(Physics.RaycastAll(transform.position, leftDirection, Range, layerMask));
            hits = hits.Union(Physics.RaycastAll(transform.position, rightDirection, Range, layerMask));
            
            return hits.Distinct();
            
        }
        

        public bool IsAvailableToShoot
        {
            get
            {
                if (Time.time > lastShot + ReloadTime)
                {
                    return true;
                }
                else
                {
                    Debug.LogWarning("Not ready");
                    return false;
                }
            }
        }


    }
}

