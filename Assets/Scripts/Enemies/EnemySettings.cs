using UnityEngine;

namespace CyberpixelOk.Enemies
{
    [CreateAssetMenu(menuName = "CyberpixelOk/Enemies/Enemy Settings", fileName = "EnemySettings")]
    public class EnemySettings : ScriptableObject
    {
        [SerializeField] private float maxHealth = 3f;
        [SerializeField] private float patrolSpeed = 2f;
        [SerializeField] private float chaseSpeed = 4f;
        [SerializeField] private float detectionRange = 6f;
        [SerializeField] private float attackRange = 1.2f;
        [SerializeField] private float attackCooldown = 1.2f;
        [SerializeField] private float patrolWaitTime = 1f;
        [SerializeField] private float targetLostRange = 8f;

        public float MaxHealth => maxHealth;
        public float PatrolSpeed => patrolSpeed;
        public float ChaseSpeed => chaseSpeed;
        public float DetectionRange => detectionRange;
        public float AttackRange => attackRange;
        public float AttackCooldown => attackCooldown;
        public float PatrolWaitTime => patrolWaitTime;
        public float TargetLostRange => targetLostRange;
    }
}
