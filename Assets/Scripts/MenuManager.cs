using UnityEngine;
using UnityEngine.SceneManagement;
using CyberpixelOk.Managers;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void PlayGame()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.StartGameplay2D();
            return;
        }

        SceneManager.LoadScene("Gameplay2d");
    }
}
