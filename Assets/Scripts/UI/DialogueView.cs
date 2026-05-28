using TMPro;
using UnityEngine;

namespace CyberpixelOk.UI
{
    [DisallowMultipleComponent]
    public class DialogueView : MonoBehaviour
    {
        [SerializeField] private TMP_Text speakerLabel;
        [SerializeField] private TMP_Text dialogueLabel;
        [SerializeField] private GameObject root;

        public void SetVisible(bool visible)
        {
            if (root != null)
            {
                root.SetActive(visible);
            }
        }

        public void SetSpeaker(string speakerName)
        {
            if (speakerLabel != null)
            {
                speakerLabel.text = speakerName;
            }
        }

        public void SetDialogue(string dialogueLine)
        {
            if (dialogueLabel != null)
            {
                dialogueLabel.text = dialogueLine;
            }
        }
    }
}
