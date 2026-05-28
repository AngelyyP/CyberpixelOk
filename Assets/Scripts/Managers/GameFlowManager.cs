using System;
using System.Collections;
using CyberpixelOk.Core;
using CyberpixelOk.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CyberpixelOk.Managers
{
    [DisallowMultipleComponent]
    public class GameFlowManager : MonoBehaviour
    {
        public static GameFlowManager Instance { get; private set; }

        [Header("Scene Names")]
        [SerializeField] private string menuSceneName = "MainMenu";
        [SerializeField] private string gameplay2DSceneName = "Gameplay2d";
        [SerializeField] private string gameplay3DSceneName = "Gameplay3d";

        [Header("Transition")]
        [SerializeField] private ScreenFader screenFader;

        public event Action<GameMode> ModeChanged;

        public GameMode CurrentMode { get; private set; } = GameMode.Menu;

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

        public void LoadMenu()
        {
            StartCoroutine(LoadMenuRoutine());
        }

        public void StartGameplay2D()
        {
            StartCoroutine(SwitchToGameplayRoutine(gameplay2DSceneName, GameMode.Gameplay2D));
        }

        public void StartGameplay3D()
        {
            StartCoroutine(SwitchToGameplayRoutine(gameplay3DSceneName, GameMode.Gameplay3D));
        }

        public void ReturnTo2DFrom3D()
        {
            StartCoroutine(SwitchToGameplayRoutine(gameplay2DSceneName, GameMode.Gameplay2D));
        }

        private IEnumerator LoadMenuRoutine()
        {
            if (screenFader != null)
            {
                yield return screenFader.FadeOut();
            }

            CurrentMode = GameMode.Menu;
            ModeChanged?.Invoke(CurrentMode);

            yield return SceneManager.LoadSceneAsync(menuSceneName, LoadSceneMode.Single);

            if (screenFader != null)
            {
                yield return screenFader.FadeIn();
            }
        }

        private IEnumerator SwitchToGameplayRoutine(string targetSceneName, GameMode targetMode)
        {
            if (screenFader != null)
            {
                yield return screenFader.FadeOut();
            }

            Scene previousActiveScene = SceneManager.GetActiveScene();

            if (!SceneManager.GetSceneByName(targetSceneName).isLoaded)
            {
                yield return SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
            }

            Scene targetScene = SceneManager.GetSceneByName(targetSceneName);
            if (targetScene.IsValid())
            {
                SceneManager.SetActiveScene(targetScene);
            }

            if (previousActiveScene.IsValid() && previousActiveScene.isLoaded && previousActiveScene.name != targetSceneName)
            {
                yield return SceneManager.UnloadSceneAsync(previousActiveScene);
            }

            CurrentMode = targetMode;
            ModeChanged?.Invoke(CurrentMode);

            if (screenFader != null)
            {
                yield return screenFader.FadeIn();
            }
        }
    }
}
