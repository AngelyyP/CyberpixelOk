using UnityEngine;

namespace CyberpixelOk.Managers
{
    [DisallowMultipleComponent]
    public class CollectibleObjectiveManager : MonoBehaviour
    {
        [SerializeField, Min(0)] private int requiredCollectibles = 3;
        [SerializeField] private bool resetProgressOnAwake = true;

        private bool appliedRequirement;

        private void Awake()
        {
            TryApplyRequirement();
        }

        private void Start()
        {
            TryApplyRequirement();
        }

        private void Update()
        {
            if (appliedRequirement)
            {
                return;
            }

            TryApplyRequirement();
        }

        private void TryApplyRequirement()
        {
            if (appliedRequirement)
            {
                return;
            }

            GameSessionManager sessionManager = GameSessionManager.Instance != null ? GameSessionManager.Instance : FindFirstObjectByType<GameSessionManager>();
            if (sessionManager == null)
            {
                return;
            }

            sessionManager.SetCollectibleRequirement(requiredCollectibles);

            if (resetProgressOnAwake)
            {
                sessionManager.ResetCollectibleProgress();
            }

            appliedRequirement = true;
        }
    }
}