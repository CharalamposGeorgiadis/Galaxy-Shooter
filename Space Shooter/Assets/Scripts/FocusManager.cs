using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusManager : MonoBehaviour
{
    /// <summary>
    /// Called before the first frame update
    /// </summary>
    private void Start()
    {
        Application.runInBackground = false;
    }
}
