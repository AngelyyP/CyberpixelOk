using UnityEngine;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    public Image brightnessPanel;
    public Slider brightnessSlider;

    void Start()
    {
        brightnessSlider.value = 1f;
    }

    void Update()
    {
        Color color = brightnessPanel.color;

        color.a = 1 - brightnessSlider.value;

        brightnessPanel.color = color;
    }
}