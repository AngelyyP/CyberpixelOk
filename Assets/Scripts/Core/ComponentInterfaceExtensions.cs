using UnityEngine;

namespace CyberpixelOk.Core
{
    public static class ComponentInterfaceExtensions
    {
        public static bool TryGetInterfaceInSelfOrParents<TInterface>(this Component component, out TInterface result)
            where TInterface : class
        {
            if (component == null)
            {
                result = null;
                return false;
            }

            return component.gameObject.TryGetInterfaceInSelfOrParents(out result);
        }

        public static bool TryGetInterfaceInSelfOrParents<TInterface>(this GameObject gameObject, out TInterface result)
            where TInterface : class
        {
            if (gameObject == null)
            {
                result = null;
                return false;
            }

            MonoBehaviour[] behaviours = gameObject.GetComponentsInParent<MonoBehaviour>(true);
            for (int index = 0; index < behaviours.Length; index++)
            {
                if (behaviours[index] is TInterface typedInterface)
                {
                    result = typedInterface;
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}
