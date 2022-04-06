using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    // Loading porgress bar (Slider object)
    [Tooltip("Loading progress bar (Slider object)")]
    [SerializeField]
    private Slider progressBar;

    // Name of the scene that is loading
    [Tooltip("Name of the scene that is loading")]
    [SerializeField]
    private string sceneName;

    // Loading screen percentage
    [Tooltip("Loading screen percentage")]
    [SerializeField]
    private TextMeshProUGUI loadingPercentage;

    /// <summary>
    /// Loads a level asynchronously
    /// </summary>
    public void Start()
    {
        // Loading the game scene asynchronously
        StartCoroutine(LoadSceneAsynchronously());
    }

    /// <summary>
    /// Coroutine for loading the game scene asynchronously
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadSceneAsynchronously()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            progressBar.value = progress;
            loadingPercentage.text = (progress * 100f).ToString("0") + "%";
            yield return null;
        }
    }
}
