using System;
using UnityEngine;

namespace CyberpixelOk.Core
{
    [DisallowMultipleComponent]
    public class HealthComponent : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;

        public event Action<float, float> HealthChanged;
        public event Action<DamageInfo> Damaged;
        public event Action Died;

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public bool IsDead { get; private set; }

        private void Awake()
        {
            if (currentHealth <= 0f)
            {
                currentHealth = maxHealth;
            }
        }

        public void SetMaxHealth(float newMaxHealth, bool refill = true)
        {
            maxHealth = Mathf.Max(1f, newMaxHealth);

            if (refill || currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            IsDead = currentHealth <= 0f;
            HealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void SetHealth(float value)
        {
            currentHealth = Mathf.Clamp(value, 0f, maxHealth);
            IsDead = currentHealth <= 0f;
            HealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void RestoreFullHealth()
        {
            currentHealth = maxHealth;
            IsDead = false;
            HealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f)
            {
                return;
            }

            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            HealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (IsDead || damageInfo.Amount <= 0f)
            {
                return;
            }

            currentHealth = Mathf.Max(0f, currentHealth - damageInfo.Amount);
            Damaged?.Invoke(damageInfo);
            HealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0f)
            {
                IsDead = true;
                Died?.Invoke();
            }
        }
    }
}
