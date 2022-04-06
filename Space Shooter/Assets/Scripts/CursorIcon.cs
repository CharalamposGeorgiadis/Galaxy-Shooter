using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorIcon : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursorImage;

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    void Start()
    {
        Cursor.SetCursor(cursorImage, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
