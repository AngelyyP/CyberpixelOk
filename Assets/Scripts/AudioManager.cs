using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public Slider volumeSlider;

    void Start()
    {
        volumeSlider.value = 1f;
    }

    void Update()
    {
        musicSource.volume = volumeSlider.value;
    }
}