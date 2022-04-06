using UnityEngine;
using UnityEngine.UI;

public class AudioOnSliderClick : MonoBehaviour
{
    // Audio type
    [Tooltip("Audio type(Examples: ExplosionSounds, LaserSounds, BackgroundMusic)")]
    [SerializeField]
    private string audioObjectName;

    // Audio slider
    [Tooltip("Audio slider")]
    [SerializeField]
    private Slider audioSlider;

    // All Audio Sources of the current Audio type
    private AudioSource[] audioSources;

    public void PlayAudioOnSliderClick()
    {
        audioSources = GameObject.Find(audioObjectName).GetComponentsInChildren<AudioSource>();
        int audioSample = Random.Range(0, audioSources.Length);
        audioSources[audioSample].volume = audioSlider.value;
        audioSources[audioSample].Play();
    }
}
