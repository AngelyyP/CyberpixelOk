using CyberpixelOk.Systems;
using UnityEngine;

namespace CyberpixelOk.Interactions
{
    public readonly struct InteractorContext
    {
        public readonly GameObject Interactor;
        public readonly Transform Transform;
        public readonly GameInputReader Input;

        public InteractorContext(GameObject interactor, Transform transform, GameInputReader input)
        {
            Interactor = interactor;
            Transform = transform;
            Input = input;
        }
    }
}
