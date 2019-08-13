using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Hedge.Tools;
namespace Shooter
{
    public class Gun : MonoBehaviour, IWeapon
    {
        [SerializeField] int baseDamage = 20;
        [SerializeField] float baseReloadTime = 3;
        [SerializeField] float baseRange = 5;
        [SerializeField] float baseShotSpread = 60;
        [SerializeField] LayerMask layerMask;
        public int Damage => baseDamage;
        public float ReloadTime => baseReloadTime;
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

            foreach (RaycastHit hit in GetAllHits(direction))
            {
                IHitable target = hit.collider.gameObject.GetComponent<IHitable>();
                if (target != null)
                {
                    HitInfo hitInfo = new HitInfo(Damage);
                    target.Strike(hitInfo);
                }

            }

            Debug.Log("Bang Bang");
            lastShot = Time.time;
        }

        private IEnumerable<RaycastHit> GetHitsBeforeWall(Vector3 direction)
        {
            IEnumerable<RaycastHit> hits = Physics.RaycastAll(transform.position, direction, Range, layerMask);
            RaycastHit blockHit = hits.FirstOrDefault(other => other.collider.tag.Equals("Wall"));
            if (blockHit.collider != null)
            {
                hits = hits.Where(hit => hit.distance < blockHit.distance);
            }
            return hits;
        }
        private IEnumerable<RaycastHit> GetAllHits(Vector3 direction)
        {
            Vector3 leftDirection, rightDirection;

            float currentAngle = deltaAngle;

            IEnumerable<RaycastHit> hits = GetHitsBeforeWall(direction);

            if (AttackDispersion == 0) return hits;

            while (currentAngle < AttackDispersion)
            {
                leftDirection = direction.RotateAroundY(currentAngle);
                rightDirection = direction.RotateAroundY(-currentAngle);


                hits = hits.Union(GetHitsBeforeWall(leftDirection));
                hits = hits.Union(GetHitsBeforeWall(rightDirection));

                currentAngle += deltaAngle;
            }

            leftDirection = direction.RotateAroundY(AttackDispersion);
            rightDirection = direction.RotateAroundY(-AttackDispersion);

            hits = hits.Union(GetHitsBeforeWall(leftDirection));
            hits = hits.Union(GetHitsBeforeWall(rightDirection));

          
            return hits.Distinct(new ShotRaycastComparer());

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

