using UnityEngine;
using TMPro;

namespace CyberpixelOk.Systems
{
    public class GameUI : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private TMP_Text timerText;    // era Text
        [SerializeField] private TMP_Text counterText;  // era Text
        [SerializeField] private int objectivesRequired = 5;

        [Header("Paneles")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;

        private void Start()
        {
            winPanel?.SetActive(false);
            losePanel?.SetActive(false);
        }

        public void UpdateTimer(float seconds)
        {
            if (timerText == null) return;

            int m = Mathf.FloorToInt(seconds / 60f);
            int s = Mathf.FloorToInt(seconds % 60f);
            timerText.text = string.Format("{0}:{1:00}", m, s);

            // Se pone rojo cuando quedan menos de 30 segundos
            timerText.color = seconds <= 30f ? Color.red : Color.white;
        }

        public void UpdateCounter(int count)
        {
            if (counterText != null)
                counterText.text = $"{count} / {objectivesRequired}";
        }

        public void ShowWin()
        {
            winPanel?.SetActive(true);
            UnlockCursor();
        }

        public void ShowLose()
        {
            losePanel?.SetActive(true);
            UnlockCursor();
        }

        private void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}