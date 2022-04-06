using UnityEngine;

public class UiSounds : MonoBehaviour
{
    /// <summary>
    /// Plays audio when the player navigates the main menu
    /// </summary>
    public void UINavigation()
    {
         AudioSource audio = GameObject.Find("UINavigate").GetComponent<AudioSource>();
         audio.Play();
    }

    /// <summary>
    /// Plays audio when the player clicks a button of the main menu
    /// </summary>
    public void UISelect()
    {
        AudioSource audio = GameObject.Find("UISelect").GetComponent<AudioSource>();
        audio.Play();
    }
}
