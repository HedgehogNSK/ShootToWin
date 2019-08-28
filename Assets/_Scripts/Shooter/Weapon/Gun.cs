using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Hedge.Tools;
using Mirror;

namespace Shooter
{
    public class Gun : Weapon
    {
#pragma warning disable CS0649
        [SerializeField] int baseDamage = 20;
        [SerializeField] float baseReloadTime = 3;
        [SerializeField] float baseRange = 5;
        [SerializeField] float baseShotSpread = 60;
        [SerializeField] LayerMask affectedLayers;
        [SerializeField] LayerMask shieldLayers;
        [Space]
        [SerializeField] ParticleSystem shotParticlePrefab;
        [SerializeField] ParticleSystem hitParticlePrefab;
        [SerializeField] Transform muzzle;
        [SerializeField] AudioClip shotSound;
#pragma warning restore CS0649

        ParticleSystem particle;
        public override int Damage => baseDamage;
        public override float ReloadTime => baseReloadTime;
        public override float AttackSpread => baseShotSpread;
        public override float Range => baseRange;
        public override ParticleSystem HitParticles => hitParticlePrefab;

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
       
        public override void Attack(IAttacker attacker,Vector3 direction)
        {
            if (!IsAvailableToShoot) return;
            IEnumerable<RaycastHit> allhits = GetAllHits(transform.position, direction);
            allhits = allhits.Except(allhits.Where(hit => attacker.Equals(hit.transform.GetComponent<IAttacker>())));
            foreach (RaycastHit hit in allhits)
            {
            
                IHitable target = hit.collider.gameObject.GetComponent<IHitable>();
                if (target != null)
                {
                    HitArgs hitInfo = HitArgs.CreateBuilder().SetDirection(direction).SetAttacker(attacker).SetDamage(this);
                    target.GetStrike(hitInfo);
                }
                else
                {
                    HitAnimation(hit.point);
                }
            }
            ShotAnimation();
            ShotSound();
            lastShot = Time.time;
        }
       
       
        void ShotAnimation()
        {
            if (shotParticlePrefab != null)
            {            
                particle = Instantiate(shotParticlePrefab, muzzle);
                particle.transform.localPosition = Vector3.zero;
                Destroy(particle.gameObject, particle.main.duration);
                
            }
            
        }

        void ShotSound()
        {
            if (shotSound != null)
                AudioSource.PlayClipAtPoint(shotSound,transform.position);
        }
       
        void HitAnimation(Vector3 target)
        {
            if (HitParticles != null)
            {
                ParticleSystem particle = Instantiate(HitParticles);
                particle.transform.position = target;                
                Destroy(particle.gameObject, particle.main.duration);
                
            }

        }
        
        //Get all targets by direction of 1 ray except targets behind the Shield
        private IEnumerable<RaycastHit> GetHitsBeforeWall(Vector3 origin,Vector3 direction)
        {
            
            IEnumerable<RaycastHit> hits = Physics.RaycastAll(origin, direction, Range, affectedLayers, QueryTriggerInteraction.Ignore);
            
            RaycastHit blockHit = hits.FirstOrDefault(other => ((1<<other.transform.gameObject.layer) & shieldLayers.value) != 0);
            if (blockHit.collider != null)
            {
                hits = hits.Where(hit => hit.distance <= blockHit.distance);
            }
            return hits;
        }
        private IEnumerable<RaycastHit> GetAllHits(Vector3 origin, Vector3 direction)
        {
            Vector3 leftDirection, rightDirection;

            float currentAngle = deltaAngle;

            IEnumerable<RaycastHit> hits = GetHitsBeforeWall(origin, direction);

            if (AttackSpread == 0) return hits;

            while (currentAngle < AttackSpread/2)
            {
                leftDirection = direction.RotateAroundY(currentAngle);
                rightDirection = direction.RotateAroundY(-currentAngle);


                hits = hits.Union(GetHitsBeforeWall(origin, leftDirection));
                hits = hits.Union(GetHitsBeforeWall(origin, rightDirection));

                currentAngle += deltaAngle;
            }

            leftDirection = direction.RotateAroundY(AttackSpread/2);
            rightDirection = direction.RotateAroundY(-AttackSpread/2);

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

