using System;
using System.Collections;
using CyberpixelOk.Core;
using CyberpixelOk.Interactions;
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

        [Header("Safety")]
        [SerializeField] private bool keepNpcInteractablesActive = true;
        [SerializeField, Min(0.1f)] private float npcSafetyCheckInterval = 0.5f;

        public event Action<GameMode> ModeChanged;

        public GameMode CurrentMode { get; private set; } = GameMode.Menu;

        private float npcSafetyTimer;

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

        private void Update()
        {
            if (!keepNpcInteractablesActive)
            {
                return;
            }

            npcSafetyTimer -= Time.deltaTime;
            if (npcSafetyTimer > 0f)
            {
                return;
            }

            npcSafetyTimer = npcSafetyCheckInterval;
            EnsureNpcInteractablesActive();
        }

        public void LoadMenu()
        {
            StartCoroutine(LoadMenuRoutine());
        }

        public void StartGameplay2D()
        {
            Debug.Log($"{nameof(GameFlowManager)}: loading 2D scene '{gameplay2DSceneName}'.", this);
            StartCoroutine(SwitchToGameplayRoutine(gameplay2DSceneName, GameMode.Gameplay2D));
        }

        public void StartGameplay3D()
        {
            Debug.Log($"{nameof(GameFlowManager)}: loading 3D scene '{gameplay3DSceneName}'.", this);
            StartCoroutine(SwitchToGameplayRoutine(gameplay3DSceneName, GameMode.Gameplay3D));
        }

        public void ReturnTo2DFrom3D()
        {
            StartCoroutine(SwitchToGameplayRoutine(gameplay2DSceneName, GameMode.Gameplay2D));
        }

        private void EnsureNpcInteractablesActive()
        {
            QuestGatedNpcInteractable[] questNpcs = FindObjectsByType<QuestGatedNpcInteractable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < questNpcs.Length; index++)
            {
                QuestGatedNpcInteractable npc = questNpcs[index];
                if (npc != null && npc.gameObject.scene.IsValid() && npc.gameObject.scene.isLoaded && !npc.gameObject.activeSelf)
                {
                    npc.gameObject.SetActive(true);
                }
            }

            NpcInteractable[] simpleNpcs = FindObjectsByType<NpcInteractable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < simpleNpcs.Length; index++)
            {
                NpcInteractable npc = simpleNpcs[index];
                if (npc != null && npc.gameObject.scene.IsValid() && npc.gameObject.scene.isLoaded && !npc.gameObject.activeSelf)
                {
                    npc.gameObject.SetActive(true);
                }
            }
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

            string targetScenePath = $"Assets/Scenes/{targetSceneName}.unity";
            int buildIndex = SceneUtility.GetBuildIndexByScenePath(targetScenePath);
            if (buildIndex < 0)
            {
                Debug.LogError($"{nameof(GameFlowManager)} cannot load scene '{targetSceneName}'. Add '{targetScenePath}' to Build Settings / Build Profile.", this);
                yield break;
            }

            Scene previousActiveScene = SceneManager.GetActiveScene();

            if (!SceneManager.GetSceneByName(targetSceneName).isLoaded)
            {
                Debug.Log($"{nameof(GameFlowManager)}: scene '{targetSceneName}' not loaded, loading additively.", this);
                yield return SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
            }

            Scene targetScene = SceneManager.GetSceneByName(targetSceneName);
            if (targetScene.IsValid())
            {
                Debug.Log($"{nameof(GameFlowManager)}: setting active scene to '{targetSceneName}'.", this);
                SceneManager.SetActiveScene(targetScene);
            }

            if (previousActiveScene.IsValid() && previousActiveScene.isLoaded && previousActiveScene.name != targetSceneName)
            {
                yield return SceneManager.UnloadSceneAsync(previousActiveScene);
            }

            CurrentMode = targetMode;
            ModeChanged?.Invoke(CurrentMode);

            Debug.Log($"{nameof(GameFlowManager)}: mode changed to {CurrentMode}.", this);

            if (screenFader != null)
            {
                yield return screenFader.FadeIn();
            }
        }
    }
}
