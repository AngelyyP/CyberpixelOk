using UnityEngine;

public class CreditsScroll : MonoBehaviour
{
    public float speed = 50f;

    public float startY = -600f;
    public float endY = 700f;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        rectTransform.anchoredPosition = new Vector2(
            rectTransform.anchoredPosition.x,
            startY
        );
    }

    void Update()
    {
        rectTransform.anchoredPosition += Vector2.up * speed * Time.deltaTime;

        if (rectTransform.anchoredPosition.y > endY)
        {
            rectTransform.anchoredPosition = new Vector2(
                rectTransform.anchoredPosition.x,
                startY
            );
        }
    }
}