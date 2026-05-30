using UnityEngine;
using UnityEngine.Events;

namespace CyberpixelOk.Player
{
    [DisallowMultipleComponent]
    public class CollectionSystem : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float collectRadius = 1.5f;
        [SerializeField] private LayerMask collectableLayer;

        [Header("Events")]
        public UnityEvent<int> OnCollected;
        public UnityEvent<int> OnCountChanged;

        private int count = 0;
        public int Count => count;

        private readonly Collider[] hits = new Collider[16];
private void Update()
{
    int found = Physics.OverlapSphereNonAlloc(
        transform.position, collectRadius, hits, collectableLayer);

    // DEBUG - borra esto después
    Debug.Log($"Objetos encontrados: {found} | Layer mask: {collectableLayer.value}");

    for (int i = 0; i < found; i++)
    {
        Collectable item = hits[i].GetComponent<Collectable>();
        if (item == null) continue;

        count += item.Value;
        Destroy(hits[i].gameObject);

        OnCollected?.Invoke(item.Value);
        OnCountChanged?.Invoke(count);
    }
}

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, collectRadius);
        }
    }
}