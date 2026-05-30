using UnityEngine;
using UnityEngine.Events;
using CyberpixelOk.Player; // CollectionSystem vive aquí

namespace CyberpixelOk.Systems
{
    public class GameManager : MonoBehaviour
    {
        [Header("Condiciones")]
        [SerializeField] private int objectivesRequired = 5;
        [SerializeField] private float timeLimit = 120f; // 2 minutos

        [Header("Referencias")]
        [SerializeField] private CollectionSystem collectionSystem;

        [Header("Eventos")]
        public UnityEvent OnGameWin;
        public UnityEvent OnGameLose;
        public UnityEvent<float> OnTimerTick;   // tiempo restante cada frame
        public UnityEvent<int> OnCollectedTick; // cuántos lleva

        private float timeRemaining;
        private bool gameActive = false;

        private void Start()
        {
            /*if (collectionSystem != null)
                collectionSystem.OnCollected.AddListener(HandleCollected);
+*/
            StartGame();
        }

        private void OnDestroy()
        {
            /*if (collectionSystem != null)
                collectionSystem.OnCollected.RemoveListener(HandleCollected);*/
        }

        private void Update()
        {
            if (!gameActive) return;

            timeRemaining -= Time.deltaTime;
            OnTimerTick?.Invoke(timeRemaining);

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                Lose();
            }
        }

        private void StartGame()
        {
            timeRemaining = timeLimit;
            gameActive = true;
        }

        public void HandleCollected(int value)
        {
            if (!gameActive) return;

            OnCollectedTick?.Invoke(collectionSystem.Count);

            if (collectionSystem.Count >= objectivesRequired)
                Win();
        }

        private void Win()
        {
            gameActive = false;
            OnGameWin?.Invoke();
        }

        private void Lose()
        {
            gameActive = false;
            OnGameLose?.Invoke();
        }
    }
}
