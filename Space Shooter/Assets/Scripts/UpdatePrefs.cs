using UnityEngine;
using System.Collections.Generic;

public class UpdatePrefs : MonoBehaviour
{   
    private static readonly string firstPlay = "FirstPlay";

    // Audio types (Examples: Music, Explosion)
    [Tooltip("Audio types (Examples: Music, Explosion)")]
    [SerializeField]
    private List<string> audioTypes = new();

    // Audio Game Objects
    [Tooltip("Audio Game Objects")]
    [SerializeField]
    private List<GameObject> audioGameObjects = new();

    // Integer containing whether thsi session is the first time the user has started the game on their device
    private int firstPlayInt;

    // Audio Sources
    private readonly List<AudioSource> audioSources = new();

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    void Start()
    {
        // Finding all the AudioSources of the game
        foreach (GameObject g in audioGameObjects)
        {
            AudioSource[] tempAudio = g.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource a in tempAudio)
                audioSources.Add(a.GetComponent<AudioSource>());
        }
        
        // Initializing or loading the Player's audio preferences
        for (int i = 0; i < audioTypes.Count; i++)
        {
            firstPlayInt = PlayerPrefs.GetInt(firstPlay + audioTypes[i]);
            if (firstPlayInt == 0)
            {
                foreach (AudioSource a in audioSources)
                {
                    if (a.name.Contains(audioTypes[i]))
                    {
                        PlayerPrefs.SetFloat(audioTypes[i] + "Pref", a.volume);
                        PlayerPrefs.SetInt(firstPlay + audioTypes[i], -1);
                    }
                }
            }
            else
                foreach (AudioSource a in audioSources)
                {
                    if (a.name.Contains(audioTypes[i]))
                        a.volume = PlayerPrefs.GetFloat(audioTypes[i] + "Pref");
                } 
        }    
    }
}
