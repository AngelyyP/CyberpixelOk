using CyberpixelOk.Core;
using CyberpixelOk.Weapons;
using UnityEngine;

namespace CyberpixelOk.Player
{
    [DisallowMultipleComponent]
    public class PlayerAnimationBridge2D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController2D playerController;
        [SerializeField] private PlayerWeaponController2D weaponController;
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem jetpackParticles;

        [Header("Animator Parameters")]
        [SerializeField] private string moveSpeedParameter = "MoveSpeed";
        [SerializeField] private string verticalSpeedParameter = "VerticalSpeed";
        [SerializeField] private string groundedParameter = "IsGrounded";
        [SerializeField] private string jetpackParameter = "IsJetpacking";
        [SerializeField] private string deadParameter = "IsDead";
        [SerializeField] private string shootTrigger = "Shoot";
        [SerializeField] private string hurtTrigger = "Hurt";
        [SerializeField] private string deathTrigger = "Death";

        private void Reset()
        {
            playerController = GetComponentInParent<PlayerController2D>();
            weaponController = GetComponentInParent<PlayerWeaponController2D>();
            healthComponent = GetComponentInParent<HealthComponent>();
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            if (weaponController != null)
            {
                weaponController.WeaponFired += HandleWeaponFired;
            }

            if (healthComponent != null)
            {
                healthComponent.Damaged += HandleDamaged;
                healthComponent.Died += HandleDied;
            }
        }

        private void OnDisable()
        {
            if (weaponController != null)
            {
                weaponController.WeaponFired -= HandleWeaponFired;
            }

            if (healthComponent != null)
            {
                healthComponent.Damaged -= HandleDamaged;
                healthComponent.Died -= HandleDied;
            }
        }

        private void LateUpdate()
        {
            if (playerController == null || animator == null)
            {
                return;
            }

            PlayerStateSnapshot snapshot = playerController.CurrentSnapshot;
            animator.SetFloat(moveSpeedParameter, Mathf.Abs(snapshot.HorizontalSpeed));
            animator.SetFloat(verticalSpeedParameter, snapshot.VerticalSpeed);
            animator.SetBool(groundedParameter, snapshot.IsGrounded);
            animator.SetBool(jetpackParameter, snapshot.IsJetpacking);
            animator.SetBool(deadParameter, snapshot.IsDead);

            if (jetpackParticles != null)
            {
                if (snapshot.IsJetpacking && !jetpackParticles.isPlaying)
                {
                    jetpackParticles.Play();
                }
                else if (!snapshot.IsJetpacking && jetpackParticles.isPlaying)
                {
                    jetpackParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
        }

        private void HandleWeaponFired(WeaponContext context)
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
    }
}
