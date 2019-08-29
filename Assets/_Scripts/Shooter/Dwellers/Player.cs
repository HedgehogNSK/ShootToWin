using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers.Mobile;
using Hedge.UI;
using Mirror;
using UnityEngine.SceneManagement;
using System.Linq;
using Hedge.Tools;
using Shooter.GameSettings;

namespace Shooter
{
    [RequireComponent(typeof(Rigidbody), typeof(NetworkIdentity))]
    sealed public class Player : Dweller, IHitable, IAttacker
    {
        const float RESPAWN_TIME = 3;
#pragma warning disable CS0649
        [SerializeField] Transform hand;
       [SerializeField] Material aimVizualizationMaterial;
#pragma warning restore CS0649

        public Weapon weapon { get; private set; }

        Rigidbody rigid;
        new Collider collider;
        Vector3 movementDirection = Vector3.zero;
        Quaternion lookRotation = Quaternion.identity;

        [SyncVar] int frags = 10;
        public int Frags
        {
            get { return frags; }
            set
            {
                if (value >= 0)
                {
                    frags = value;
                    DataSpreader.OnUpdate?.Invoke(DataType.Points, frags);

                }
                else
                    Debug.LogError("Amount of frags can't be negative");

            }
        }

        public override int Health
        {
            get => base.Health;
            protected set
            {
                base.Health = value;
                if (isLocalPlayer)
                {
                    DataSpreader.OnUpdate?.Invoke(DataType.Health, Health);
                }
            }
        }

        public event Action<Player> OnDead;

        void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            weapon = GetComponentInChildren<Gun>();
            collider = GetComponent<Collider>();
            Frags = 0;
        }

        

        public override void BasicSetup(DwellerSettings settings)
        {
            base.BasicSetup(settings);
            if (settings is PlayerSettings)
            {
                PlayerSettings playerSettings = (PlayerSettings)settings;
                SetWeapon(playerSettings.BaseWeaponPrefab);
            }
        }

        [Command]
        public void CmdOnStartInitialize()
        {
            RpcInitialize();

        }

        [ClientRpc]
        public void RpcInitialize()
        {
            Initialize();
        }

        public override void Initialize()
        {
            Speed = baseSpeed;
            Health = baseHealth;
            lookRotation = transform.rotation;
            movementDirection = Vector3.zero;

        }

        private void SetWeapon(Weapon prefab)
        {
            if (!weapon)
            {
                weapon = Instantiate(prefab, hand);
                Camera.onPostRender += FiringZoneHilighter;
            }
        }
        private void Start()
        {
            BasicSetup(((CustomNetworkManager)NetworkManager.singleton).LevelSettings.PlayerSettings);
            ConnectControllers(true);

            if (isLocalPlayer)
            {
                CmdOnStartInitialize();
            }

        }

        private void FixedUpdate()
        {
            Move();
            Rotate();
#if MOUSE
            if (Input.GetKey(KeyCode.Mouse0))
                CmdAttack();            
#endif 
        }

        private void Move()
        {

#if KEYBOARD
            if (!isLocalPlayer) return;
            movementDirection = Vector3.zero;
            if (Input.GetKey(KeyCode.UpArrow)) movementDirection += Vector3.forward;
            if (Input.GetKey(KeyCode.DownArrow)) movementDirection += Vector3.back;
            if (Input.GetKey(KeyCode.LeftArrow)) movementDirection += Vector3.left;
            if (Input.GetKey(KeyCode.RightArrow)) movementDirection += Vector3.right;

             
#endif

            rigid.MovePosition(Speed * movementDirection.normalized * Time.fixedDeltaTime + rigid.position);

        }

#if MOUSE
        public LayerMask layerMask;
#endif
        private void Rotate()
        {

#if MOUSE
              if (!isLocalPlayer) return;
            Camera cam = Camera.main;            

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out RaycastHit raycastHit,100,layerMask,QueryTriggerInteraction.Ignore))
            {
                lookRotation = Quaternion.LookRotation(raycastHit.point.XZ() - transform.position.XZ(), Vector3.up);
               
            }
#endif
            rigid.MoveRotation(lookRotation);
        }
        public void SetMoveDirection(Joystick joystick, Vector2 direction)
        {
            movementDirection = new Vector3(direction.x, 0, direction.y).normalized;
        }

        bool highlight = false;
        public void TakeAim(Joystick joystick, Vector2 forward, bool fire)
        {
            //highlight = false;
            if (fire)
            {
                CmdAttack();
                
            }
            else
            {
                SetRotation(forward);
                highlight = true;
            }
        }


        private void FiringZoneHilighter(Camera cam)
        {
            if (!highlight) return;
            if (!cam.GetComponent<PlayerFollower>()) return;

            if (!aimVizualizationMaterial)
            {
                Debug.LogError("Material for aim vizualization isn't set");
                return;
            }
            //Debug.Log("Drawing");
            Vector3 weaponPos = weapon.transform.position;
            float spread = weapon.AttackSpread;
            float range = weapon.Range;
            Vector3 direction = transform.forward;

            aimVizualizationMaterial.SetPass(0);


            GL.Begin(GL.TRIANGLES);
            //GL.Color(new Color(0.1f, 1f, 0.2f, 0.3f));


            Vector3 nextPoint = weaponPos + direction.RotateAroundY(-spread / 2) * range;
            Vector3 rightFarthestPoint;
            int points = 1000;

            float spreadPart = spread / points;
            for (int i = 0; i != points; i++)
            {
                GL.Vertex3(weaponPos.x, weaponPos.y, weaponPos.z);

                rightFarthestPoint = nextPoint;
                nextPoint = weaponPos + direction.RotateAroundY(i * spreadPart - spread / 2) * range;
                GL.Vertex3(rightFarthestPoint.x, rightFarthestPoint.y, rightFarthestPoint.z);
                GL.Vertex3(nextPoint.x, nextPoint.y, nextPoint.z);
            }

            GL.End();


        }

        void SetRotation(Vector2 forward)
        {
            lookRotation = Quaternion.LookRotation(new Vector3(forward.x, 0, forward.y), Vector3.up);

        }

        [Command]
        void CmdAttack()
        {
            RpcAttack();
        }

        [ClientRpc]
        void RpcAttack()
        {
            if (weapon)
            {
                weapon.Attack(this, transform.forward);
            }
            else
            {
                Debug.LogWarning("Player have no weapon");
            }
        }

        public void GetStrike(HitArgs hit)
        {
            int healthBefore = Health;
            Health -= hit._Weapon.Damage;
            if (healthBefore > 0 && Health <= 0 && hit.Attacker != null)
            {
                hit.Attacker.AddKill(this);
            }
            else
            {
                HitAnimation(hit);
            }
        }

        [ClientRpc]
        public void RpcRespawnPlayer(Vector3 position, float respawnTime)
        {
            SetPosition(position);
            NetworkManager.singleton.StartCoroutine(RespawnPlayerCoroutine(respawnTime));
        }

        IEnumerator RespawnPlayerCoroutine(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            CmdOnStartInitialize();
            gameObject.SetActive(true);
            ConnectControllers(true);

        }

        protected override void Die()
        {
            ConnectControllers(false);
            DieAnimation();
            OnDead?.Invoke(this);
        }

        public void AddKill(IHitable target)
        {
            Frags++;
        }

        void ConnectControllers(bool connect)
        {
            if (!isLocalPlayer) return;
            if (connect)
            {
                SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
                MoveJoystick.OnMove += SetMoveDirection;
                AttackJoystick.OnAim += TakeAim;


            }
            else
            {
                MoveJoystick.OnMove -= SetMoveDirection;
                AttackJoystick.OnAim -= TakeAim;
                SceneManager.UnloadSceneAsync(1);
            }
        }

        private void HitAnimation(HitArgs hit)
        {
            if (hit._Weapon.HitParticles != null)
            {
                ParticleSystem particle = Instantiate(hit._Weapon.HitParticles);
                particle.transform.localPosition = transform.position - hit.Direction * 0.1f;
                Destroy(particle.gameObject, particle.main.duration);
            }
        }
        void DieAnimation()
        {
            gameObject.SetActive(false);
        }

        public void OnDisable()
        {
            ConnectControllers(false);
        }
    }
}

