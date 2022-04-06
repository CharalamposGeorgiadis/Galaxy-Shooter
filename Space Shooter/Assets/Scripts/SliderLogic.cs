using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SliderLogic : MonoBehaviour
{
    // Audio type
    [Tooltip("Audio type(Examples: music, laser)")]
    [SerializeField]
    private string audioType;

    // Audio slider
    [Tooltip("Audio slider")]
    [SerializeField]
    private Slider audioSlider;

    // Current audio volume
    private float audioVolume;

    // Audio Sources of the current Audio type
    private List<AudioSource> audioSources = new();

    // Start is called before the first frame update
    void Start()
    {
        audioVolume = PlayerPrefs.GetFloat(audioType + "Pref");
        audioSlider.value = audioVolume;
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource a in allAudioSources)
            if (a.name.Contains(audioType))
            {
                a.volume = audioVolume;
                audioSources.Add(a);
            }
    }

    public void OnValueChanged()
    {
        foreach (AudioSource a in audioSources)
        {
            a.volume = audioSlider.value;
            PlayerPrefs.SetFloat(audioType + "Pref", a.volume);
        }
    }
}
