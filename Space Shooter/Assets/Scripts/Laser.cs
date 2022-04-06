using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Speed of each laser projectile
    [Tooltip("Speed of each laser projectile")]
    [SerializeField]
    private float speed;

    /// <summary>
    /// Called once per frame
    /// </summary>
    void Update()
    {
        // Laser projectile movement
        Movement();
    }

    /// <summary>
    /// // Laser projectile movement
    /// </summary>
    private void Movement()
    {
        // Laser projectile movement
        transform.Translate(speed * Time.deltaTime * Vector3.up);

        // Destroy laser projectile onject if it is no longer visible
        if (!transform.GetComponent<SpriteRenderer>().isVisible)
        {
            if (transform.parent != null)
                if (transform.parent.childCount == 1)
                    Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }
    }
}
