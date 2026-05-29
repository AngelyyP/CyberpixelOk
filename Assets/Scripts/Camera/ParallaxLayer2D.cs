using UnityEngine;

namespace CyberpixelOk.Camera2D
{
    [DisallowMultipleComponent]
    public class ParallaxLayer2D : MonoBehaviour
    {
        private enum LayerOrderSource
        {
            SortingOrder,
            SiblingIndex
        }

        [Header("References")]
        [SerializeField] private Transform parallaxCamera;
        [SerializeField] private Renderer layerRenderer;

        [Header("Parallax")]
        [SerializeField, Range(0f, 1f)] private float parallaxX = 0.2f;
        [SerializeField, Range(0f, 1f)] private float parallaxY = 0f;
        [SerializeField] private bool useX = true;
        [SerializeField] private bool useY = false;

        [Header("Layer Depth")]
        [SerializeField] private bool useLayerDepthScaling = true;
        [SerializeField] private LayerOrderSource layerOrderSource = LayerOrderSource.SortingOrder;
        [SerializeField] private int referenceLayerOrder;
        [SerializeField, Range(-1f, 1f)] private float layerStepX = 0.06f;
        [SerializeField, Range(-1f, 1f)] private float layerStepY = 0.03f;

        private Vector3 startPosition;
        private Vector3 startCameraPosition;

        private void Awake()
        {
            if (parallaxCamera == null && Camera.main != null)
            {
                parallaxCamera = Camera.main.transform;
            }

            if (layerRenderer == null)
            {
                layerRenderer = GetComponent<Renderer>();
            }

            CacheStartPositions();
        }

        private void OnEnable()
        {
            CacheStartPositions();
        }

        private void LateUpdate()
        {
            if (parallaxCamera == null)
            {
                return;
            }

            Vector3 cameraDelta = parallaxCamera.position - startCameraPosition;
            Vector3 newPosition = startPosition;
            float effectiveParallaxX = GetEffectiveParallaxX();
            float effectiveParallaxY = GetEffectiveParallaxY();

            if (useX)
            {
                newPosition.x += cameraDelta.x * effectiveParallaxX;
            }

            if (useY)
            {
                newPosition.y += cameraDelta.y * effectiveParallaxY;
            }

            transform.position = newPosition;
        }

        private float GetEffectiveParallaxX()
        {
            if (!useLayerDepthScaling)
            {
                return parallaxX;
            }

            int layerOrder = GetLayerOrder();
            return Mathf.Clamp01(parallaxX + (layerOrder - referenceLayerOrder) * layerStepX);
        }

        private float GetEffectiveParallaxY()
        {
            if (!useLayerDepthScaling)
            {
                return parallaxY;
            }

            int layerOrder = GetLayerOrder();
            return Mathf.Clamp01(parallaxY + (layerOrder - referenceLayerOrder) * layerStepY);
        }

        private int GetLayerOrder()
        {
            if (layerOrderSource == LayerOrderSource.SiblingIndex)
            {
                return transform.GetSiblingIndex();
            }

            if (layerRenderer != null)
            {
                return layerRenderer.sortingOrder;
            }

            return transform.GetSiblingIndex();
        }

        private void CacheStartPositions()
        {
            startPosition = transform.position;

            if (parallaxCamera != null)
            {
                startCameraPosition = parallaxCamera.position;
            }
        }
    }
}
