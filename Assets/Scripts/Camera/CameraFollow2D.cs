using CyberpixelOk.Player;
using UnityEngine;

namespace CyberpixelOk.Camera2D
{
    [DisallowMultipleComponent]
    public class CameraFollow2D : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Camera followCamera;

        [Header("Follow")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
        [SerializeField, Min(0f)] private float smoothTime = 0.15f;
        [SerializeField] private bool followX = true;
        [SerializeField] private bool followY = true;

        [Header("Dead Zone")]
        [SerializeField, Range(0f, 0.49f)] private float horizontalDeadZone = 0.25f;
        [SerializeField, Range(0f, 0.49f)] private float verticalDeadZone = 0.2f;

        private Vector3 velocity;
        private Transform drivenTransform;

        private void Awake()
        {
            if (followCamera == null)
            {
                followCamera = GetComponent<Camera>();
            }

            if (followCamera == null)
            {
                followCamera = GetComponentInChildren<Camera>();
            }

            if (followCamera == null)
            {
                followCamera = Camera.main;
            }

            if (target == null)
            {
                PlayerController2D player = FindFirstObjectByType<PlayerController2D>();
                if (player != null)
                {
                    target = player.transform;
                }
            }

            drivenTransform = followCamera != null ? followCamera.transform : null;
        }

        private void LateUpdate()
        {
            if (target == null || followCamera == null || drivenTransform == null)
            {
                return;
            }

            Vector3 desiredPosition = drivenTransform.position;
            Vector3 targetViewportPosition = followCamera.WorldToViewportPoint(target.position);

            if (followX)
            {
                float leftLimit = 0.5f - horizontalDeadZone;
                float rightLimit = 0.5f + horizontalDeadZone;

                if (targetViewportPosition.x < leftLimit)
                {
                    desiredPosition.x += GetCameraDeltaX(targetViewportPosition.x - leftLimit);
                }
                else if (targetViewportPosition.x > rightLimit)
                {
                    desiredPosition.x += GetCameraDeltaX(targetViewportPosition.x - rightLimit);
                }
            }

            if (followY)
            {
                float bottomLimit = 0.5f - verticalDeadZone;
                float topLimit = 0.5f + verticalDeadZone;

                if (targetViewportPosition.y < bottomLimit)
                {
                    desiredPosition.y += GetCameraDeltaY(targetViewportPosition.y - bottomLimit);
                }
                else if (targetViewportPosition.y > topLimit)
                {
                    desiredPosition.y += GetCameraDeltaY(targetViewportPosition.y - topLimit);
                }
            }

            desiredPosition.z = offset.z != 0f ? offset.z : drivenTransform.position.z;

            if (desiredPosition == drivenTransform.position)
            {
                velocity = Vector3.zero;
                return;
            }

            if (smoothTime <= 0f)
            {
                drivenTransform.position = desiredPosition;
                return;
            }

            drivenTransform.position = Vector3.SmoothDamp(drivenTransform.position, desiredPosition, ref velocity, smoothTime);
        }

        private float GetCameraDeltaX(float viewportDelta)
        {
            if (!followCamera.orthographic)
            {
                return viewportDelta * 2f;
            }

            return viewportDelta * 2f * followCamera.orthographicSize * followCamera.aspect;
        }

        private float GetCameraDeltaY(float viewportDelta)
        {
            if (!followCamera.orthographic)
            {
                return viewportDelta * 2f;
            }

            return viewportDelta * 2f * followCamera.orthographicSize;
        }
    }
}
