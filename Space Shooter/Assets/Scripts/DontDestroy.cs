using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    // Tag that will not be destroyed
    [Tooltip("Tag that will not be destroyed")]
    [SerializeField]
    private string tagName;

    /// <summary>
    /// Called when the script instance is laoded
    /// </summary>
    void Awake()
    {
        GameObject[] objectList = GameObject.FindGameObjectsWithTag(tagName);
        if (objectList.Length > 1)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
