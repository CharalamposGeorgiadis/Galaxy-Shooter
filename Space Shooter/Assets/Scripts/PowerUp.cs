using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Speed of the power-up
    [Tooltip("Speed of the power-up.")]
    [SerializeField]
    private float speed;

    // Power-up sounds
    private AudioSource[] powerUpSounds;

    // Height of the power-up
    private float powerUpHeight;

    // Boundaries of the screen for any resolution
    private Vector2 screenBounds;

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    /// <summary>
    /// Called when a power-up is generated
    /// </summary>
    private void OnEnable()
    {
        powerUpHeight = transform.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        powerUpSounds = GameObject.Find("PowerUpSounds").GetComponentsInChildren<AudioSource>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Power-up movement
        Movement();

        // Destroy the power-up if the player is destroyed
        DestroyPowerUp();
    }

    /// <summary>
    /// Moves the power-up
    /// </summary>
    private void Movement()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.down);

        if (transform.position.y < screenBounds.y - powerUpHeight)
            Destroy(gameObject);
    }

    /// <summary>
    /// Destroys the power-up if the player is destroyed
    /// </summary>
    private void DestroyPowerUp()
    {
        if (!GameObject.FindGameObjectWithTag("Player"))
            Destroy(gameObject);
    }

    /// <summary>
    /// Called when the attached object collides with another collider
    /// </summary>
    /// <param name="other">Collider of other object</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Activating power-up if it collides with the player
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().ActivatePowerUp(gameObject.name.Split("(")[0]);

            foreach (AudioSource a in powerUpSounds)
            {
                string name = a.clip.name;
                if (gameObject.name.Contains(name))
                    a.Play();
            }
            Destroy(gameObject);
        }
    }
}
