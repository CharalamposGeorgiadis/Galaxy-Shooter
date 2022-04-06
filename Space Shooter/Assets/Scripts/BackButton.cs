using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    // Name of the scene that this back button leads to
    [Tooltip("Name of the scene that this back button leads to")]
    [SerializeField]
    private string scene;

    public void Back()
    {
        SceneManager.LoadScene(scene);
    }
}
