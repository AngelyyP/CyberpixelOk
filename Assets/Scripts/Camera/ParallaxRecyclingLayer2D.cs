using System.Collections.Generic;
using UnityEngine;

namespace CyberpixelOk.Camera2D
{
    [DisallowMultipleComponent]
    public class ParallaxRecyclingLayer2D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform parallaxCamera;

        [Header("Parallax")]
        [SerializeField, Range(0f, 1f)] private float parallaxX = 0.2f;
        [SerializeField, Range(0f, 1f)] private float parallaxY = 0f;

        [Header("Grid (recycling)")]
        [Tooltip("Size in world units of a single tile (width, height). If zero, will try to auto-detect from first child's renderer bounds.")]
        [SerializeField] private Vector2 tileSize = Vector2.zero;
        [Tooltip("Number of tiles in X (columns) and Y (rows) used in the repeating grid.")]
        [SerializeField] private Vector2Int gridSize = new Vector2Int(3, 1);

        private Vector3 startPosition;
        private Vector3 startCameraPosition;

        private List<Transform> tiles = new List<Transform>();
        private List<Vector3> initialLocalPositions = new List<Vector3>();
        private float totalWidth;
        private float totalHeight;

        private void Awake()
        {
            if (parallaxCamera == null && Camera.main != null)
            {
                parallaxCamera = Camera.main.transform;
            }

            // gather children as tiles
            tiles.Clear();
            initialLocalPositions.Clear();

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                tiles.Add(child);
                initialLocalPositions.Add(child.localPosition);
            }

            if (tiles.Count == 0)
            {
                Debug.LogWarning($"{nameof(ParallaxRecyclingLayer2D)} on '{name}' has no child tiles.");
            }

            // auto-detect tile size if not provided
            if (tileSize.sqrMagnitude <= 0.0001f && tiles.Count > 0)
            {
                Renderer r = tiles[0].GetComponent<Renderer>();
                if (r != null)
                {
                    tileSize = new Vector2(r.bounds.size.x, r.bounds.size.y);
                }
            }

            // ensure sensible grid
            gridSize.x = Mathf.Max(1, gridSize.x);
            gridSize.y = Mathf.Max(1, gridSize.y);

            totalWidth = tileSize.x * gridSize.x;
            totalHeight = tileSize.y * gridSize.y;

            CacheStartPositions();
        }

        private void OnEnable()
        {
            CacheStartPositions();
        }

        private void LateUpdate()
        {
            if (parallaxCamera == null || tiles.Count == 0)
            {
                return;
            }

            Vector3 cameraDelta = parallaxCamera.position - startCameraPosition;
            Vector3 basePos = startPosition + new Vector3(cameraDelta.x * parallaxX, cameraDelta.y * parallaxY, 0f);

            Vector3 camPos = parallaxCamera.position;

            for (int i = 0; i < tiles.Count; i++)
            {
                Vector3 local = initialLocalPositions[i];
                Vector3 worldPos = basePos + transform.TransformVector(local);

                // wrap horizontally
                float diffX = worldPos.x - camPos.x;
                if (totalWidth > 0f)
                {
                    while (diffX > totalWidth * 0.5f)
                    {
                        worldPos.x -= totalWidth;
                        diffX -= totalWidth;
                    }

                    while (diffX < -totalWidth * 0.5f)
                    {
                        worldPos.x += totalWidth;
                        diffX += totalWidth;
                    }
                }

                // wrap vertically
                float diffY = worldPos.y - camPos.y;
                if (totalHeight > 0f)
                {
                    while (diffY > totalHeight * 0.5f)
                    {
                        worldPos.y -= totalHeight;
                        diffY -= totalHeight;
                    }

                    while (diffY < -totalHeight * 0.5f)
                    {
                        worldPos.y += totalHeight;
                        diffY += totalHeight;
                    }
                }

                tiles[i].position = worldPos;
            }
        }

        private void CacheStartPositions()
        {
            startPosition = transform.position;
            if (parallaxCamera != null)
            {
                startCameraPosition = parallaxCamera.position;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            gridSize.x = Mathf.Max(1, gridSize.x);
            gridSize.y = Mathf.Max(1, gridSize.y);
        }
#endif
    }
}
