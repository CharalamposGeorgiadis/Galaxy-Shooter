using System.Collections.Generic;
using UnityEngine;

public class TrackShuffle : MonoBehaviour
{
    // Background music game objects
    [Tooltip("Background music game objects")]
    [SerializeField]
    private List<GameObject> musicObjects = new();

    // Background music Audio Sources
    private List<AudioSource> musicTracks = new();

    // Current track number
    private int currentTrack = 0;

    // Track history
    private List<int> trackHistory = new();

    // Boolean containing whether a music track is playing or not
    private bool isPlaying;

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    void Start()
    {
        foreach (GameObject m in musicObjects)
            musicTracks.Add(m.GetComponent<AudioSource>());

        currentTrack = Random.Range(0, musicTracks.Count);
        musicTracks[currentTrack].Play();
        trackHistory.Add(currentTrack);
        InvokeRepeating(nameof(ChangeSong), 0, 1.0f);
    }

    /// <summary>
    /// Changing song once the current song has ended
    /// </summary>
    void ChangeSong()
    {
        if (Application.isFocused)
            isPlaying = false;
        foreach (AudioSource t in musicTracks)
            if (t.isPlaying)
            {
                isPlaying = true;
                break;
            }
        if (!isPlaying)
        {
            currentTrack = Random.Range(0, musicTracks.Count - 1);
            if (!trackHistory.Contains(currentTrack))
            {
                musicTracks[currentTrack].Play();
                trackHistory.Add(currentTrack);
            }
            else if (trackHistory.Count == musicTracks.Count)
            {
                if (currentTrack == trackHistory[^1])
                    currentTrack = trackHistory[^2];
                trackHistory.Clear();
                musicTracks[currentTrack].Play();
                trackHistory.Add(currentTrack);
            }
        }
    }
}
