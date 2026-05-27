using System.Collections.Generic;
using UnityEngine;

namespace CyberpixelOk.Interactions
{
    [CreateAssetMenu(menuName = "CyberpixelOk/Dialogue/Dialogue Data", fileName = "DialogueData")]
    public class DialogueData : ScriptableObject
    {
        [SerializeField] private string speakerName = "NPC";
        [TextArea(2, 4)]
        [SerializeField] private List<string> lines = new List<string>();

        public string SpeakerName => speakerName;
        public IReadOnlyList<string> Lines => lines;
    }
}
