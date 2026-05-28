using System.Collections;
using UnityEngine;

namespace CyberpixelOk.UI
{
    [DisallowMultipleComponent]
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.35f;

        private void Reset()
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>(true);
        }

        private void Awake()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponentInChildren<CanvasGroup>(true);
            }
        }

        public IEnumerator FadeIn()
        {
            yield return FadeTo(0f);
        }

        public IEnumerator FadeOut()
        {
            yield return FadeTo(1f);
        }

        private IEnumerator FadeTo(float targetAlpha)
        {
            if (canvasGroup == null)
            {
                yield break;
            }

            canvasGroup.gameObject.SetActive(true);
            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }
    }
}
