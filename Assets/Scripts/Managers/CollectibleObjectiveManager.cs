using CyberpixelOk.Interactions;
using UnityEngine;

namespace CyberpixelOk.Managers
{
    [DisallowMultipleComponent]
    public class CollectibleObjectiveManager : MonoBehaviour
    {
        [SerializeField] private bool autoCountSceneCollectibles = true;
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

            int targetRequirement = requiredCollectibles;

            if (autoCountSceneCollectibles)
            {
                int sceneCollectibleCount = FindObjectsByType<CollectibleInteractable>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length
                    + FindObjectsByType<CollectibleInteractable3D>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;

                if (sceneCollectibleCount > 0)
                {
                    targetRequirement = sceneCollectibleCount;
                }
            }

            sessionManager.SetCollectibleRequirement(targetRequirement);

            if (resetProgressOnAwake)
            {
                sessionManager.ResetCollectibleProgress();
            }

            appliedRequirement = true;
        }
    }
}