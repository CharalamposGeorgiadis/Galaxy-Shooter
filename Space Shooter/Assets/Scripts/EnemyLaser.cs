using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    // Speed of each laser projectile
    [Tooltip("Speed of each laser projectile")]
    public float speed;

    // Height of the screen for any resolution
    private float screenHeight;

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    void Start()
    {
        screenHeight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)).y;
    }

    /// <summary>
    /// Called once per frame
    /// </summary>
    void Update()
    {
        // Laser projectile movement
        Movement();
    }

    /// <summary>
    /// Laser projectile movement
    /// </summary>
    private void Movement()
    {
        // Laser projectile movement
        transform.Translate(speed * Time.deltaTime * Vector3.down);

        // Destroy laser projectile onject if it has gone off screen
        if (transform.position.y < screenHeight * 1.5f)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            else
                Destroy(gameObject);
        }
    }
}
