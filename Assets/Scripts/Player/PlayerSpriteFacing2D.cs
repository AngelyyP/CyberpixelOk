using UnityEngine;

namespace CyberpixelOk.Player
{
    [DisallowMultipleComponent]
    public class PlayerSpriteFacing2D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController2D playerController;
        [SerializeField] private SpriteRenderer[] spriteRenderers;

        [Header("Settings")]
        [SerializeField] private bool invertFlip;

        private void Reset()
        {
            playerController = GetComponentInParent<PlayerController2D>();
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        }

        private void LateUpdate()
        {
            if (playerController == null || spriteRenderers == null || spriteRenderers.Length == 0)
            {
                return;
            }

            bool facingRight = playerController.CurrentSnapshot.FacingRight;
            bool flipX = invertFlip ? facingRight : !facingRight;

            for (int index = 0; index < spriteRenderers.Length; index++)
            {
                if (spriteRenderers[index] != null)
                {
                    spriteRenderers[index].flipX = flipX;
                }
            }
        }
    }
}
