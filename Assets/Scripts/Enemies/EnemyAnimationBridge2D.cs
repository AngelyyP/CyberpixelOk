using CyberpixelOk.Core;
using UnityEngine;

namespace CyberpixelOk.Enemies
{
    [DisallowMultipleComponent]
    public class EnemyAnimationBridge2D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RangedEnemy rangedEnemy;
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private Animator animator;

        [Header("Animator Parameters")]
        [SerializeField] private string idleTrigger = "Idle";
        [SerializeField] private string shootTrigger = "Shoot";
        [SerializeField] private string hurtTrigger = "Hurt";
        [SerializeField] private string deathTrigger = "Death";

        private void Reset()
        {
            rangedEnemy = GetComponentInParent<RangedEnemy>();
            healthComponent = GetComponentInParent<HealthComponent>();
            animator = GetComponent<Animator>();

            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>(true);
            }
        }

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>(true);
            }

            if (animator != null)
            {
                animator.applyRootMotion = false;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }
        }

        private void OnEnable()
        {
            if (rangedEnemy != null)
            {
                rangedEnemy.ShotFired += HandleShotFired;
            }

            if (healthComponent != null)
            {
                healthComponent.Damaged += HandleDamaged;
                healthComponent.Died += HandleDied;
            }

            PlayIdle();
        }

        private void OnDisable()
        {
            if (rangedEnemy != null)
            {
                rangedEnemy.ShotFired -= HandleShotFired;
            }

            if (healthComponent != null)
            {
                healthComponent.Damaged -= HandleDamaged;
                healthComponent.Died -= HandleDied;
            }
        }

        private void HandleShotFired()
        {
            if (animator != null)
            {
                animator.SetTrigger(shootTrigger);
            }
        }

        private void HandleDamaged(DamageInfo damageInfo)
        {
            if (animator != null)
            {
                animator.SetTrigger(hurtTrigger);
            }
        }

        private void HandleDied()
        {
            if (animator != null)
            {
                animator.SetTrigger(deathTrigger);
            }
        }

        private void PlayIdle()
        {
            if (animator != null)
            {
                animator.SetTrigger(idleTrigger);
            }
        }
    }
}