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

            GenerateTiles();
            CacheStart();
        }

        private void GenerateTiles()
        {
            // Clear existing children we created earlier
            tiles.Clear();

            // detect width from prefab renderer
            Renderer r = tilePrefab.GetComponentInChildren<Renderer>();
            if (r != null)
            {
                tileWidth = r.bounds.size.x;
            }

            if (tileWidth <= 0f)
            {
                // try to detect after instantiate one
                GameObject temp = Instantiate(tilePrefab, transform.position, Quaternion.identity);
                Renderer rt = temp.GetComponentInChildren<Renderer>();
                if (rt != null)
                {
                    tileWidth = rt.bounds.size.x;
                }
                DestroyImmediate(temp);
            }

            if (tileWidth <= 0f)
            {
                Debug.LogWarning($"{nameof(ParallaxAutoTiler2D)} could not determine tile width. Set a SpriteRenderer with correct size on the prefab.");
                tileWidth = 1f;
            }

            float cellWidth = tileWidth + spacing;

            float centerOffset = (columns - 1) * 0.5f * cellWidth;

            for (int i = 0; i < columns; i++)
            {
                Vector3 localPos = new Vector3((i * cellWidth) - centerOffset, 0f, 0f);
                GameObject go = Instantiate(tilePrefab, transform);
                go.transform.localPosition = localPos;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
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
            columns = Mathf.Max(2, columns);
            spacing = Mathf.Max(0f, spacing);
            parallaxX = Mathf.Clamp01(parallaxX);
            parallaxY = Mathf.Clamp01(parallaxY);
        }
#endif
    }
}
