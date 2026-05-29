using UnityEngine;

namespace CyberpixelOk.Managers
{
    [DisallowMultipleComponent]
    public class CollectibleObjectiveManager : MonoBehaviour
    {
        [SerializeField, Min(0)] private int requiredCollectibles = 3;
        [SerializeField] private bool resetProgressOnAwake = true;

        private void Awake()
        {
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
        }
    }
}