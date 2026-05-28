using CyberpixelOk.Core;
using UnityEngine;

namespace CyberpixelOk.Weapons
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class ProjectileBase : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private Collider2D projectileCollider;

        private GameObject owner;
        private ProjectilePool pool;
        private ProjectileBase poolKey;
        private Vector2 travelDirection;
        private float lifetimeTimer;
        private int damage;

        public ProjectileBase PoolKey => poolKey;

        private void Reset()
        {
            body = GetComponent<Rigidbody2D>();
            projectileCollider = GetComponent<Collider2D>();
        }

        private void Awake()
        {
            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
            }

            if (projectileCollider == null)
            {
                projectileCollider = GetComponent<Collider2D>();
            }
        }

        private void Update()
        {
            if (lifetimeTimer <= 0f)
            {
                return;
            }

            lifetimeTimer -= Time.deltaTime;
            if (lifetimeTimer <= 0f)
            {
                Despawn();
            }
        }

        public void Initialize(GameObject ownerObject, int projectileDamage, Vector2 direction, float speed, float lifetime, ProjectilePool projectilePool)
        {
            owner = ownerObject;
            damage = projectileDamage;
            travelDirection = direction.normalized;
            lifetimeTimer = lifetime;
            pool = projectilePool;

            if (body != null)
            {
                body.linearVelocity = travelDirection * speed;
            }

            gameObject.SetActive(true);
        }

        internal void SetPoolKey(ProjectileBase sourcePrefab)
        {
            poolKey = sourcePrefab;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == null || other.gameObject == owner)
            {
                return;
            }

            HealthComponent health = other.GetComponentInParent<HealthComponent>();
            if (health != null)
            {
                health.TakeDamage(new DamageInfo
                {
                    Amount = damage,
                    Source = owner,
                    HitPoint = transform.position,
                    HitDirection = travelDirection,
                    DamageType = "Projectile"
                });
            }

            Despawn();
        }

        public void Despawn()
        {
            if (pool != null)
            {
                pool.Despawn(this);
                return;
            }

            Destroy(gameObject);
        }

        private void OnDisable()
        {
            owner = null;
            pool = null;
            lifetimeTimer = 0f;
            travelDirection = Vector2.zero;
        }
    }
}
