using System.Collections.Generic;
using UnityEngine;

namespace CyberpixelOk.Weapons
{
    [DisallowMultipleComponent]
    public class ProjectilePool : MonoBehaviour
    {
        public static ProjectilePool Instance { get; private set; }

        private readonly Dictionary<ProjectileBase, Queue<ProjectileBase>> pooledProjectiles = new Dictionary<ProjectileBase, Queue<ProjectileBase>>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public ProjectileBase Spawn(ProjectileBase prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
            {
                return null;
            }

            if (!pooledProjectiles.TryGetValue(prefab, out Queue<ProjectileBase> queue))
            {
                queue = new Queue<ProjectileBase>();
                pooledProjectiles.Add(prefab, queue);
            }

            ProjectileBase projectile = queue.Count > 0 ? queue.Dequeue() : Instantiate(prefab);
            projectile.SetPoolKey(prefab);
            projectile.transform.SetPositionAndRotation(position, rotation);
            projectile.gameObject.SetActive(true);
            return projectile;
        }

        public void Despawn(ProjectileBase projectile)
        {
            if (projectile == null)
            {
                return;
            }

            ProjectileBase prefab = projectile.PoolKey;
            if (prefab == null)
            {
                Destroy(projectile.gameObject);
                return;
            }

            if (!pooledProjectiles.TryGetValue(prefab, out Queue<ProjectileBase> queue))
            {
                queue = new Queue<ProjectileBase>();
                pooledProjectiles.Add(prefab, queue);
            }

            projectile.gameObject.SetActive(false);
            queue.Enqueue(projectile);
        }
    }
}
