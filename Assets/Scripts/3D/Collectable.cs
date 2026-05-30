using UnityEngine;

namespace CyberpixelOk.Player
{
    public class Collectable : MonoBehaviour
    {
        [SerializeField] private int value = 1;

        public int Value => value;
    }
}
