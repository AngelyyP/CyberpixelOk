using System.Collections.Generic;
using UnityEngine;

namespace CyberpixelOk.Camera2D
{
    [DisallowMultipleComponent]
    public class ParallaxAutoTiler2D : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Prefab containing a SpriteRenderer (or any renderer). The script will instantiate copies as children.")]
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform parallaxCamera;

        [Header("Tiling")]
        [SerializeField, Min(2)] private int columns = 3;
        [SerializeField] private float spacing = 0f; // extra gap between tiles
        [SerializeField, Min(0.01f)] private float fallbackTileWidth = 8f;

        [Header("Parallax")]
        [SerializeField, Range(0f, 1f)] private float parallaxX = 0.2f;
        [SerializeField, Range(0f, 1f)] private float parallaxY = 0f;

        private List<Transform> tiles = new List<Transform>();
        private float tileWidth = 0f;
        private Vector3 startCameraPos;
        private Vector3 startPosition;

        private void Awake()
        {
            if (parallaxCamera == null && Camera.main != null)
            {
                parallaxCamera = Camera.main.transform;
            }

            if (tilePrefab == null)
            {
                Debug.LogWarning($"{nameof(ParallaxAutoTiler2D)} requires a tilePrefab to auto-generate tiles.", this);
                return;
            }

            if (tilePrefab.GetComponentInChildren<ParallaxAutoTiler2D>() != null)
            {
                Debug.LogError($"{nameof(ParallaxAutoTiler2D)} detected on tilePrefab '{tilePrefab.name}'. Remove it from the prefab to avoid recursive instantiation.", this);
                enabled = false;
                return;
            }

            GenerateTiles();
            CacheStart();
        }

        private void GenerateTiles()
        {
            columns = Mathf.Clamp(columns, 2, 8);

            // Clear existing list and preserve scene children as-is.
            tiles.Clear();

            SpriteRenderer spriteRenderer = tilePrefab.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                tileWidth = spriteRenderer.sprite.bounds.size.x * Mathf.Abs(spriteRenderer.transform.localScale.x);
            }

            if (tileWidth <= 0f)
            {
                Renderer renderer = tilePrefab.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    tileWidth = renderer.bounds.size.x;
                }
            }

            if (tileWidth <= 0f)
            {
                tileWidth = fallbackTileWidth;
                Debug.LogWarning($"{nameof(ParallaxAutoTiler2D)} could not determine tile width. Using fallback width {fallbackTileWidth}.", this);
            }

            float cellWidth = tileWidth + spacing;

            float centerOffset = (columns - 1) * 0.5f * cellWidth;

            for (int i = 0; i < columns; i++)
            {
                Vector3 localPos = new Vector3((i * cellWidth) - centerOffset, 0f, 0f);
                GameObject go = Instantiate(tilePrefab, transform);
                go.transform.localPosition = localPos;
                go.transform.localRotation = Quaternion.identity;
                tiles.Add(go.transform);
            }
        }

        private void CacheStart()
        {
            startPosition = transform.position;
            if (parallaxCamera != null)
            {
                startCameraPos = parallaxCamera.position;
            }
        }

        private void LateUpdate()
        {
            if (parallaxCamera == null || tiles.Count == 0)
            {
                return;
            }

            Vector3 cameraDelta = parallaxCamera.position - startCameraPos;
            Vector3 basePos = startPosition + new Vector3(cameraDelta.x * parallaxX, cameraDelta.y * parallaxY, 0f);

            float totalWidth = tileWidth * columns + spacing * (columns - 1);
            Vector3 camPos = parallaxCamera.position;

            for (int i = 0; i < tiles.Count; i++)
            {
                // initial slot position relative to base
                float cellWidth = tileWidth + spacing;
                float centerOffset = (columns - 1) * 0.5f * cellWidth;
                Vector3 local = new Vector3((i * cellWidth) - centerOffset, 0f, 0f);
                Vector3 worldPos = basePos + transform.TransformVector(local);

                // wrap horizontally so tiles recycle around the camera
                float diffX = worldPos.x - camPos.x;

                if (totalWidth > 0f)
                {
                    if (diffX > totalWidth * 0.5f)
                    {
                        worldPos.x -= totalWidth * Mathf.Ceil((diffX - totalWidth * 0.5f) / totalWidth);
                    }
                    else if (diffX < -totalWidth * 0.5f)
                    {
                        worldPos.x += totalWidth * Mathf.Ceil((-totalWidth * 0.5f - diffX) / totalWidth);
                    }
                }

                tiles[i].position = worldPos;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            columns = Mathf.Clamp(columns, 2, 8);
            spacing = Mathf.Max(0f, spacing);
            fallbackTileWidth = Mathf.Max(0.01f, fallbackTileWidth);
            parallaxX = Mathf.Clamp01(parallaxX);
            parallaxY = Mathf.Clamp01(parallaxY);
        }
#endif
    }
}
