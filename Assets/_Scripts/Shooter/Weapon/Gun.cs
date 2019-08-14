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
        [SerializeField] LayerMask affectedLayers;
        [SerializeField] LayerMask shieldLayers;
        [Space]
        [SerializeField] ParticleSystem particlePrefab;
        ParticleSystem particle;
        [SerializeField] Transform muzzle;
        public int Damage => baseDamage;
        public float ReloadTime => baseReloadTime;
        public float AttackDispersion => baseShotSpread;
        public float Range => baseRange;

        //Step in degrees for Raycasting area in front of weapon
        float deltaAngle = 5;
        float lastShot;
        private void Awake()
        {
#if DEBUG
            if ((affectedLayers | shieldLayers) != affectedLayers) Debug.LogError("Shield Layers must be add in affected layers at first");
#endif
            lastShot = -ReloadTime;
        }
        public void Attack(Vector3 origin,Vector3 direction)
        {
            if (!IsAvailableToShoot) return;

            foreach (RaycastHit hit in GetAllHits(origin, direction))
            {
                IHitable target = hit.collider.gameObject.GetComponent<IHitable>();
                if (target != null)
                {
                    HitInfo hitInfo = new HitInfo(Damage);
                    target.Strike(hitInfo);
                }

            }
            Animation();
            Debug.Log("Bang Bang");
            lastShot = Time.time;
        }

        void Animation()
        {
            particle = Instantiate(particlePrefab, muzzle);
            particle.transform.localPosition = Vector3.zero;
            Destroy(particle.gameObject, particle.main.duration);
        }

        //Get all targets by direction of 1 ray except targets behind the Shield
        private IEnumerable<RaycastHit> GetHitsBeforeWall(Vector3 origin,Vector3 direction)
        {
            
            IEnumerable<RaycastHit> hits = Physics.RaycastAll(origin, direction, Range, affectedLayers, QueryTriggerInteraction.Ignore);
            
            RaycastHit blockHit = hits.FirstOrDefault(other => ((1<<other.transform.gameObject.layer) & shieldLayers.value) != 0);
            if (blockHit.collider != null)
            {
                hits = hits.Where(hit => hit.distance < blockHit.distance);
            }
            return hits;
        }
        private IEnumerable<RaycastHit> GetAllHits(Vector3 origin, Vector3 direction)
        {
            Vector3 leftDirection, rightDirection;

            float currentAngle = deltaAngle;

            IEnumerable<RaycastHit> hits = GetHitsBeforeWall(origin, direction);

            if (AttackDispersion == 0) return hits;

            while (currentAngle < AttackDispersion)
            {
                leftDirection = direction.RotateAroundY(currentAngle);
                rightDirection = direction.RotateAroundY(-currentAngle);


                hits = hits.Union(GetHitsBeforeWall(origin, leftDirection));
                hits = hits.Union(GetHitsBeforeWall(origin, rightDirection));

                currentAngle += deltaAngle;
            }

            leftDirection = direction.RotateAroundY(AttackDispersion);
            rightDirection = direction.RotateAroundY(-AttackDispersion);

            hits = hits.Union(GetHitsBeforeWall(origin, leftDirection));
            hits = hits.Union(GetHitsBeforeWall(origin, rightDirection));

          
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

