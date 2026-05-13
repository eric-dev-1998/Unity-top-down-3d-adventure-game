using Assets.Scripts.Player;
using Assets.Scripts.Systems.Spell;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.World.Enemy
{
    [RequireComponent(typeof(Entity))]
    public class EnemyCore : SpellInteraction
    { 
        // Motion properties such as move speed are stored in the Entity class.

        public enum Behavior { Static, Neutral, Hostile };
        public enum AttackBehavior { Melee, Ranged };

        [Header("Enemy properties:")]
        public Behavior behavior;
        public AttackBehavior attackBehavior;
        public float DistanceForMeleeAttack = 0.34f;
        public float MinDistanceForRangedAttack = 2.5f;
        public float CooldownAfterAttack = 1f;
        public GameObject ProjectilePrefab;
        public Transform ProjectileSpawnPoint;
        private float _attackCooldownTimer = 0f;
        private bool _HasAttacked = false;

        [Header("Damage recieved:")]
        public int NeutralDamage = 1;
        public int FireDamage = 1;
        public int WaterDamage = 1;
        public int WindDamage = 1;
        public int EarthDamage = 1;

        [Header("Wandering properties:")]
        public float WanderingAreaRadius = 4f;
        public float WanderingDelay = 3f;
        private float _wanderingDelayTimer = 0f;
        private float _elapsedTimeWithoutMoving = 0f;
        private Vector3 _startPosition;
        private Vector3 _currentWalkPoint = Vector3.zero;
        private Vector3 _currentDirection = Vector3.zero;
        private Vector3 _lastPosition = Vector3.zero;

        [Header("Chase properties:")]
        public float MaxDistanceToChase = 3f;
        public float ChaseCooldownAfterHit = 1.10f;
        private bool _IsChasing = false;

        private Entity _entity;

        private float _ChaseCooldownCounter = 0f;
        private Vector3 _velocity = Vector3.zero;
        private PlayerCore _player;

        private BoxCollider _triggerCollider;

        public void Reset()
        { 
            GameObject projectileSpawnPointObject = transform.Find("ProjectileSpawnPoint").gameObject;
            if (projectileSpawnPointObject == null)
            {
                projectileSpawnPointObject = new GameObject("ProjectileSpawnPoint");
                projectileSpawnPointObject.transform.parent = transform;

                ProjectileSpawnPoint = projectileSpawnPointObject.transform;
            }
            else
            {
                ProjectileSpawnPoint = projectileSpawnPointObject.transform;
            }
        }

        public virtual void Ready() { }

        private void Start()
        {
            _attackCooldownTimer = CooldownAfterAttack;
            _entity = GetComponent<Entity>();
            _startPosition = transform.position;

            _triggerCollider = GetComponent<BoxCollider>();

            Ready();
        }

        public virtual void UpdateEnemy() { }

        public override void UpdateExternal()
        {
            if (Health >= 1)
            {
                // Constantly check player's distance.
                CheckDistanceFromPlayer();

                UpdateBasedOnBehavior();
            }

            UpdateEnemy();
        }

        private void UpdateBasedOnBehavior()
        {
            if (behavior == Behavior.Neutral)
                UpdateNeutral();
            else if (behavior == Behavior.Hostile)
                UpdateHostile();
        }

        private void UpdateNeutral()
        {
            // A neutral enemy will just wander around and can inflict damage if touch the player.
            WanderAround();
        }

        private void UpdateHostile()
        {
            // A hostile enemy will wander around but can chase after the player if it gets too close.

            if (_HasAttacked)
            {
                _attackCooldownTimer += Time.deltaTime;
                if (_attackCooldownTimer >= CooldownAfterAttack)
                {
                    // Stop attack animation:
                    _entity.entityAnimator.animator.SetBool("Attack", false);
                    _HasAttacked = false;
                }

                return;
            }

            if (!_IsChasing)
            {
                WanderAround();
            }
            else
            {
                // This enemy will chase after the player and will only stop if:
                // 1. It hits the player, in this case it will only stop momentarily to give the player a chance to react.
                // 2. The player gets far enough, in this case this enemy will go back to its start position.

                ChasePlayer();
            }
        }

        public virtual void Resurrect()
        {
            transform.position = _startPosition;
            Health = MaxHealth;
        }

        private void CheckDistanceFromPlayer()
        {
            if (_player == null)
                _player = FindAnyObjectByType<PlayerCore>();

            float currentDistance = Vector3.Distance(transform.position, _player.transform.position);

            if (currentDistance <= MaxDistanceToChase)
                _IsChasing = true;
            else
                _IsChasing = false;
        }

        private void ChasePlayer()
        {
            // Using this variable will tell this enemy to stop moving if its attacking the player, more specificly for ranged enemies.
            bool walk = true;

            Vector3 playerPosition = _player.transform.position;

            Vector3 directionVector = playerPosition - transform.position;
            directionVector.Normalize();

            float distanceFromPlayer = Vector3.Distance(playerPosition, transform.position);
            float realDistance = distanceFromPlayer - (_player.GetEntity().GetCharacterController().radius + _entity.GetCharacterController().radius);

            // Attack if the player is close enough.
            switch (attackBehavior)
            {
                case AttackBehavior.Melee:
                    if (realDistance <= DistanceForMeleeAttack)
                    {
                        Attack();
                        walk = false;
                    }
                    break;

                case AttackBehavior.Ranged:

                    // Wait until player is in front to shoot.

                    RaycastHit hit;
                    Vector3 origin = transform.position + transform.forward * 0.5f;
                    origin.y = 0.5f;

                    Debug.DrawRay(origin, directionVector, Color.aliceBlue, MinDistanceForRangedAttack);

                    int layerMask = 1 << LayerMask.NameToLayer("Enemy target");
                    if (Physics.Raycast(origin, directionVector, out hit, MinDistanceForRangedAttack, layerMask))
                    {
                        AttackRanged();
                        walk = false;
                    }
                    break;
            }

            if (walk)
            {
                // Play walk animation:
                _entity.entityAnimator.animator.SetBool("Walk", true);

                // Rotate towards move direction:
                Quaternion targetRotation = Quaternion.LookRotation(directionVector, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _entity.rotationSpeed * Time.deltaTime);

                // Move:
                _entity.GetCharacterController().Move(directionVector * _entity.runSpeed * Time.deltaTime);
            }
            else
                // Stop walk animation:
                _entity.entityAnimator.animator.SetBool("Walk", false);
        }

        private void WanderAround()
        {
            // The loop:
            // 1. Decide the next point.
            // 2. Walk to that point.
            // 3. Wait a little.

            if (_currentWalkPoint != Vector3.zero)
            {
                float distance = Vector3.Distance(transform.position, _currentWalkPoint);
                if (distance <= 0.15f)
                {
                    // Stop for a while and move to another point inside the walk radius.
                    if (_wanderingDelayTimer >= WanderingDelay)
                    {
                        // Decide next point.
                        _wanderingDelayTimer = 0f;
                        DecideNextWanderingPoint();
                    }
                    else
                    {
                        // Stop walk animation:
                        _entity.entityAnimator.animator.SetBool("Walk", false);

                        // Kepp wating.
                        _wanderingDelayTimer += Time.deltaTime;
                    }
                }
                else
                {
                    // Play walk animation.
                    _entity.entityAnimator.animator.SetBool("Walk", true);

                    // Keep walking towards the next point.
                    Vector3 directionVector = _currentWalkPoint - transform.position;
                    directionVector.Normalize();
                    directionVector.y = 0;

                    // Rotate towrads move direction.
                    Quaternion targetRotation = Quaternion.LookRotation(directionVector, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _entity.rotationSpeed * Time.deltaTime);

                    _lastPosition = transform.position;

                    // Move.
                    _entity.GetCharacterController().Move(directionVector * _entity.walkSpeed * Time.deltaTime);

                    if (transform.position == _lastPosition)
                    {
                        if (_elapsedTimeWithoutMoving >= 0.5f)
                        {
                            // Change direction.
                            _elapsedTimeWithoutMoving = 0f;
                            DecideNextWanderingPoint();
                        }
                        else
                            _elapsedTimeWithoutMoving += Time.deltaTime;
                    }
                    else
                        _elapsedTimeWithoutMoving = 0f;
                }
            }
            else
            {
                DecideNextWanderingPoint();
            }
        }

        private void DecideNextWanderingPoint()
        {
            float angle = Random.Range(0, 361);

            Vector3 directionPoint = new Vector3(
                WanderingAreaRadius * Mathf.Cos(angle), 
                0, 
                WanderingAreaRadius * Mathf.Sin(angle)
            );

            _currentWalkPoint = _startPosition + directionPoint;
        }

        public override void OnPlayerCollision()
        {
            if (_player == null)
                _player = FindAnyObjectByType<PlayerCore>();

            Vector3 knockbackDirection = _player.transform.position - transform.position;
            knockbackDirection.Normalize();

            _player.GetEntity().Knockback(knockbackDirection, 3f);
            _player.GetEntity().RecieveDamage(1);
            _ChaseCooldownCounter = ChaseCooldownAfterHit;
        }

        public override void OnLowHealth()
        {
            _triggerCollider.enabled = false;
            _entity.GetCharacterController().enabled = false;

            GameObject lifeCrystal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Collectables/Life crystal.prefab");
            lifeCrystal.name = $"Life crystal - {FindObjectsByType<Collectible>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length}";
            Instantiate(lifeCrystal, transform.position + transform.forward * 0.5f, Quaternion.identity);

            Kill();
        }

        public virtual void Kill() 
        {
            Destroy(gameObject);
        }

        public virtual void Attack() 
        {
            if (_HasAttacked)
                return;

            // Play attack animation:
            _entity.entityAnimator.animator.SetBool("Attack", true);

            _HasAttacked = true;
            _attackCooldownTimer = 0f;
            Debug.Log("Melee attack.");
        }

        public virtual void AttackRanged() 
        {
            if (_HasAttacked)
                return;

            // Play attack animation:
            _entity.entityAnimator.animator.SetBool("Attack", true);

            _HasAttacked = true;
            _attackCooldownTimer = 0f;
            Debug.Log("Ranged attack.");
        }

        public virtual void RecieveDamage(int ammount, Vector3 direction, float force)
        {
            if (IsDamaged)
                return;

            DamageTimer = 0f;
            IsDamaged = true;

            Health -= ammount;
            if (Health <= 0)
                Health = 0;

            if (behavior == Behavior.Hostile)
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            GetEntity().Knockback(direction, force);
        }

        public void SpawnProjectile()
        {
            // This method is called within the attack animation.

            GameObject newProjectile = Instantiate(ProjectilePrefab);
            newProjectile.transform.position = ProjectileSpawnPoint.position;
            newProjectile.transform.rotation = transform.rotation;

            // Leaving the 'Attack' bool as true untill the last frame causes the animation to loop, so it is set to false here.
            _entity.entityAnimator.animator.SetBool("Attack", false);
        }

        public void RestoreHit() { GetComponent<Animator>().SetBool("Hit", false); }

        public PlayerCore GetPlayer() { return _player; }
        public Entity GetEntity() { return _entity; }
        public BoxCollider GetTriggerCollider() { return _triggerCollider; }
    }
}
