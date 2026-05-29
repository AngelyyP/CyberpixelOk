using System;
using CyberpixelOk.Systems;
using CyberpixelOk.UI;
using UnityEngine;

namespace CyberpixelOk.Interactions
{
    [DisallowMultipleComponent]
    public class DialogueController : MonoBehaviour
    {
        public static DialogueController Instance { get; private set; }

        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private DialogueView dialogueView;

        public event Action<DialogueData, string> DialogueLineChanged;
        public event Action<DialogueData> DialogueStarted;
        public event Action DialogueEnded;

        public bool IsDialogueActive => currentDialogue != null;

        private DialogueData currentDialogue;
        private int currentLineIndex;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            if (inputReader == null)
            {
                inputReader = FindFirstObjectByType<GameInputReader>();
            }

            if (dialogueView == null)
            {
                dialogueView = FindFirstObjectByType<DialogueView>(FindObjectsInactive.Include);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            if (inputReader == null)
            {
                inputReader = FindFirstObjectByType<GameInputReader>();
            }

            if (inputReader != null)
            {
                inputReader.InteractPressed += HandleAdvancePressed;
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.InteractPressed -= HandleAdvancePressed;
            }
        }

        public void StartDialogue(DialogueData dialogueData)
        {
            if (dialogueData == null || dialogueData.Lines.Count == 0)
            {
                return;
            }

            currentDialogue = dialogueData;
            currentLineIndex = 0;

            if (dialogueView != null)
            {
                dialogueView.SetVisible(true);
                dialogueView.SetSpeaker(currentDialogue.SpeakerName);
                dialogueView.SetDialogue(currentDialogue.Lines[currentLineIndex]);
            }

            DialogueStarted?.Invoke(currentDialogue);
            DialogueLineChanged?.Invoke(currentDialogue, currentDialogue.Lines[currentLineIndex]);
        }

        public void EndDialogue()
        {
            if (currentDialogue == null)
            {
                return;
            }

            currentDialogue = null;
            currentLineIndex = 0;

            if (dialogueView != null)
            {
                dialogueView.SetVisible(false);
            }

            DialogueEnded?.Invoke();
        }

        private void HandleAdvancePressed()
        {
            if (currentDialogue == null)
            {
                return;
            }

            currentLineIndex++;
            if (currentLineIndex >= currentDialogue.Lines.Count)
            {
                EndDialogue();
                return;
            }

            if (dialogueView != null)
            {
                dialogueView.SetSpeaker(currentDialogue.SpeakerName);
                dialogueView.SetDialogue(currentDialogue.Lines[currentLineIndex]);
            }

            DialogueLineChanged?.Invoke(currentDialogue, currentDialogue.Lines[currentLineIndex]);
        }
    }
}
