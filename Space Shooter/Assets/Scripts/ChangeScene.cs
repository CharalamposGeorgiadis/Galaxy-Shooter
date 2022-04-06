using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Name of the scene that will be loaded
    [Tooltip("Name of the scene that will be loaded")]
    [SerializeField]
    private string scene;

    /// <summary>
    /// Called when a button that loads another scene is clicked
    /// </summary>
    public void OnButtonClick()
    {
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// Sets the scene's name
    /// </summary>
    /// <param name="sceneName">Scene name</param>
    public void setSceneName(string sceneName)
    {
        scene = sceneName;
    }
}
