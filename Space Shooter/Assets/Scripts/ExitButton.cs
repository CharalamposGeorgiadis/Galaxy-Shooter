using UnityEngine;

public class ExitButton : MonoBehaviour
{
    /// <summary>
    /// Called when the exit button is pressed
    /// </summary>
    public void OnButtonClick()
    {
        Application.Quit();
    }
}
