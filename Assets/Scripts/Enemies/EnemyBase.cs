using CyberpixelOk.Core;
using UnityEngine;

namespace CyberpixelOk.Enemies
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(HealthComponent))]
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EnemySettings settings;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private Transform target;
        [SerializeField] private Transform[] patrolPoints;

        public EnemyState CurrentState { get; private set; } = EnemyState.Idle;

        private int patrolIndex;
        private float patrolWaitTimer;
        private float nextAttackTime;

        protected Transform Target => target;
        protected HealthComponent TargetHealth => target != null ? target.GetComponentInParent<HealthComponent>() : null;
        protected EnemySettings Settings => settings;

        private void Reset()
        {
            body = GetComponent<Rigidbody2D>();
            healthComponent = GetComponent<HealthComponent>();
        }

        private void Awake()
        {
            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
            }

            if (healthComponent == null)
            {
                healthComponent = GetComponent<HealthComponent>();
            }

            if (settings != null)
            {
                healthComponent.SetMaxHealth(settings.MaxHealth);
            }

            healthComponent.Died += HandleDeath;
        }

        private void OnDestroy()
        {
            if (healthComponent != null)
            {
                healthComponent.Died -= HandleDeath;
            }
        }

        private void Update()
        {
            if (CurrentState == EnemyState.Dead || settings == null)
            {
                return;
            }

            if (target == null)
            {
                AcquireTarget();
            }

            if (target == null)
            {
                PatrolIdle();
                return;
            }

            float distance = Vector2.Distance(transform.position, target.position);
            if (distance <= settings.AttackRange)
            {
                SetState(EnemyState.Attack);
                TryAttack();
                return;
            }

            if (distance <= settings.DetectionRange)
            {
                SetState(EnemyState.Chase);
                ChaseTarget();
                return;
            }

            if (distance > settings.TargetLostRange)
            {
                target = null;
            }

            PatrolIdle();
        }

        protected abstract void TryAttack();

        protected virtual void PatrolIdle()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                SetState(EnemyState.Idle);
                body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
                return;
            }

            SetState(EnemyState.Patrol);
            Transform patrolPoint = patrolPoints[patrolIndex];
            if (patrolPoint == null)
            {
                return;
            }

            if (patrolWaitTimer > 0f)
            {
                patrolWaitTimer -= Time.deltaTime;
                body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
                return;
            }

            MoveTowards(patrolPoint.position, settings.PatrolSpeed);
            if (Vector2.Distance(transform.position, patrolPoint.position) <= 0.25f)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                patrolWaitTimer = settings.PatrolWaitTime;
            }
        }

        protected virtual void ChaseTarget()
        {
            if (target == null)
            {
                return;
            }

            MoveTowards(target.position, settings.ChaseSpeed);
        }

        protected void ApplyDamage(DamageInfo damageInfo)
        {
            healthComponent.TakeDamage(damageInfo);
        }

        protected bool CanAttackNow()
        {
            return Time.time >= nextAttackTime;
        }

        protected void RegisterAttackCooldown()
        {
            nextAttackTime = Time.time + settings.AttackCooldown;
        }

        protected void FaceMovementDirection(float horizontalVelocity)
        {
            if (Mathf.Abs(horizontalVelocity) <= 0.01f)
            {
                return;
            }

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (horizontalVelocity >= 0f ? 1f : -1f);
            transform.localScale = scale;
        }

        private void AcquireTarget()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }

        private void MoveTowards(Vector3 destination, float speed)
        {
            float direction = Mathf.Sign(destination.x - transform.position.x);
            body.linearVelocity = new Vector2(direction * speed, body.linearVelocity.y);
            FaceMovementDirection(body.linearVelocity.x);
        }

        private void SetState(EnemyState newState)
        {
            CurrentState = newState;
        }

        private void HandleDeath()
        {
            SetState(EnemyState.Dead);
            body.linearVelocity = Vector2.zero;
        }
    }
}
