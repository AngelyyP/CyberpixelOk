using CyberpixelOk.Core;
using UnityEngine;

namespace CyberpixelOk.Managers
{
    [DisallowMultipleComponent]
    public class GameplayModeBinder : MonoBehaviour
    {
        [Header("Roots")]
        [SerializeField] private GameObject player2DRoot;
        [SerializeField] private GameObject player3DRoot;

        [Header("Cameras")]
        [SerializeField] private GameObject camera2DRoot;
        [SerializeField] private GameObject camera3DRoot;

        [Header("Cursor")]
        [SerializeField] private bool lockCursorDuringGameplay = true;

        private void OnEnable()
        {
            if (GameFlowManager.Instance != null)
            {
                GameFlowManager.Instance.ModeChanged += HandleModeChanged;
                HandleModeChanged(GameFlowManager.Instance.CurrentMode);
            }
        }

        private void OnDisable()
        {
            if (GameFlowManager.Instance != null)
            {
                GameFlowManager.Instance.ModeChanged -= HandleModeChanged;
            }
        }

        private void HandleModeChanged(GameMode mode)
        {
            bool is2DGameplay = mode == GameMode.Gameplay2D;
            bool is3DGameplay = mode == GameMode.Gameplay3D;

            if (player2DRoot != null)
            {
                player2DRoot.SetActive(is2DGameplay);
            }

            if (player3DRoot != null)
            {
                player3DRoot.SetActive(is3DGameplay);
            }

            if (camera2DRoot != null)
            {
                camera2DRoot.SetActive(is2DGameplay);
            }

            if (camera3DRoot != null)
            {
                camera3DRoot.SetActive(is3DGameplay);
            }

            if (lockCursorDuringGameplay)
            {
                bool lockGameplayCursor = is2DGameplay || is3DGameplay;
                Cursor.lockState = lockGameplayCursor ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !lockGameplayCursor;
            }
        }
    }
}
