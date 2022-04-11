using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // Asteroid Prefab
    [Tooltip("Asteroid Prefab")]
    [SerializeField]
    private GameObject asteroid;

    // Probability to spawn smaller asteroids after one is destroyed by a laser
    [Tooltip("Probability to spawn smaller asteroids after one is destroyed by a laser (1 - 100)")]
    public int smallerAsteroidsProbability;

    // Maximum number of smaller asteroids that can spawn after one is created
    [Tooltip("Maximum number of smaller asteroids that can spawn after one is created")]
    [SerializeField]
    private int maxNumberOfSmallerAsteroids;

    // Minimum velocity of the smaller asteroids on the horizontal axis
    [Tooltip("Minimum velocity of the smaller asteroids on the horizontal axis (can be negative)")]
    [SerializeField]
    private float minXVelocity;

    // Maximum velocity of the smaller asteroids on the horizontal axis
    [Tooltip("Maximum velocity of the smaller asteroids on the horizontal axis (can be negative)")]
    [SerializeField]
    private float maxXVelocity;

    // Minimum velocity of the smaller asteroids on the vertical axis
    [Tooltip("Minimum velocity of the smaller asteroids on the vertical axis (can be negative)")]
    [SerializeField]
    private float minYVelocity;

    // Minimum velocity of the smaller asteroids on the vertical axis
    [Tooltip("Minimum velocity of the smaller asteroids on the vertical axis (can be negative)")]
    [SerializeField]
    private float maxYVelocity;

    // Minimum rotation speed for the smaller asteroids
    [Tooltip("Minimum rotation speed for the smaller asteroids")]
    [SerializeField]
    private float minRotationSpeed;

    // Maximum rotation speed for the smaller asteroids
    [Tooltip("Maximum rotation speed for the smaller asteroids")]
    [SerializeField]
    private float maxRotationSpeed;

    // Points awarded to the player if they hit an asteroid with a laser
    [Tooltip("Points awared to the player if they hit an asteroid with a laser")]
    [SerializeField]
    private int pointsForLaserHit;

    // Points awarded to the player if they hit an asteroid with their ship
    [Tooltip("Points awared to the player if they hit an asteroid with their ship")]
    [SerializeField]
    private int pointsForShipHit;

    // Points awared to the player if an asteroid spawned by another asteroid hits an enemy
    [Tooltip("Points awared to the player if an asteroid spawned by another asteroid hits an enemy")]
    [SerializeField]
    private int pointsForEnemyHit;

    // Explosion Sounds
    private GameObject explosionAudio;

    // Asteroid speed
    private float speed;

    // Asteroid Rotation Speed
    private float rotationSpeed;

    // Player Object
    private Player player;

    // Width of the asteroid
    [HideInInspector]
    public float asteroidWidth;

    // Height of the asteroid
    [HideInInspector]
    public float asteroidHeight;

    // Boundaries of the screen for any resolution
    private Vector2 screenBounds;

    // Asteroid Animator
    private Animator animator;

    // Boolean containing whether the asteroid can collide and destroyed enemy ships
    [HideInInspector]
    public bool canDestroyEnemy = false;

    /// <summary>
    /// Called when an enemy is generated
    /// </summary>
    private void OnEnable()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        player = GameObject.Find("Player").GetComponent<Player>();
        animator = GetComponent<Animator>();
        asteroidWidth = transform.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        asteroidHeight = transform.GetComponent<SpriteRenderer>().bounds.size.y / 2;
    }

    /// <summary>
    /// Called once per frame
    /// </summary>
    void Update()
    {
        // Asteroid movement
        Movement();

        // Asteroid Rotation
        Rotation();
    }

    /// <summary>
    /// Asteroid movement
    /// </summary>
    private void Movement()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.down, Space.World);
        if ((transform.position.y < screenBounds.y - asteroidHeight) || transform.position.y > -screenBounds.y * GetComponent<Asteroid>().asteroidHeight)
            Destroy(gameObject);
    }

    /// <summary>
    /// Asteroid rotation
    /// </summary>
    private void Rotation()
    {
       transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.forward);
    }

    /// <summary>
    /// Called when the attached object collides with another collider
    /// </summary>
    /// <param name="other">Collider of other object</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Destroying asteroid if hit by a bullet
        if (other.CompareTag("Laser"))
        {
            GameObject.Find("Asteroid Spawner").GetComponent<AsteroidGenerator>().AsteroidDestroyed();
            explosionAudio = GameObject.Find("ExplosionSounds");
            int explosionSoundNumber = Random.Range(1, explosionAudio.transform.childCount + 1);
            AudioSource explosionSound = GameObject.Find("Explosion" + explosionSoundNumber.ToString()).GetComponent<AudioSource>();
            explosionSound.Play();
            Vector3 destroyedAsteroidScale = gameObject.transform.localScale;
            GetComponent<CircleCollider2D>().enabled = false;
            Destroy(other.gameObject);
            animator.SetTrigger("OnDestroyed");
            speed = 1f;
            Destroy(gameObject, 2.2f);
            player.IncreasePlayerScore(pointsForLaserHit);
            // Random chance to spawn smaller asteroids when one is destroyed by a laser
            if (Random.Range(1, 100) <= smallerAsteroidsProbability)
            {
                int numberOfNewAsteroids = Random.Range(1, maxNumberOfSmallerAsteroids + 1);
                for (int i = 0; i < numberOfNewAsteroids; i++)
                {
                    GameObject newAsteroid = Instantiate(asteroid, transform.position, Quaternion.identity);
                    Asteroid newAsteroidObject = newAsteroid.GetComponent<Asteroid>();
                    newAsteroid.GetComponent<CircleCollider2D>().enabled = true;
                    newAsteroidObject.smallerAsteroidsProbability = 0;

                    float xVelocity = Random.Range(minXVelocity, maxXVelocity);
                    float yVelocity = Random.Range(minYVelocity, maxYVelocity);

                    newAsteroid.transform.localScale = new Vector3(Random.Range(0.1f, destroyedAsteroidScale.x / 2), Random.Range(0.1f, destroyedAsteroidScale.y / 2), 1f);

                    newAsteroidObject.rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
                    newAsteroid.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity, yVelocity);

                    newAsteroid.GetComponent<Asteroid>().canDestroyEnemy = true;
                }
            }
        }
        // Destroying asteroid if hit by the player
        else if (other.CompareTag("Player"))
        {
            GameObject.Find("Asteroid Spawner").GetComponent<AsteroidGenerator>().AsteroidDestroyed();
            explosionAudio = GameObject.Find("ExplosionSounds");
            int explosionSoundNumber = Random.Range(1, explosionAudio.transform.childCount + 1);
            AudioSource explosionSound = GameObject.Find("Explosion" + explosionSoundNumber.ToString()).GetComponent<AudioSource>();
            explosionSound.Play();
            GetComponent<CircleCollider2D>().enabled = false;
            animator.SetTrigger("OnDestroyed");
            speed = 1;
            Destroy(gameObject, 2.2f);
            player.IncreasePlayerScore(pointsForShipHit);
        }
        // Destroying asteroid and enemy if they collide
        else if (other.CompareTag("Enemy") && canDestroyEnemy)
        {
            GameObject.Find("Asteroid Spawner").GetComponent<AsteroidGenerator>().AsteroidDestroyed();
            GetComponent<CircleCollider2D>().enabled = false;
            //animator.SetTrigger("OnDestroyed");
            speed = 1;
            Destroy(gameObject);
            player.IncreasePlayerScore(pointsForEnemyHit);
        }
        // Destroying asteroid if an enemy hits it with a laser
        else if (other.CompareTag("EnemyLaser"))
        {
            GameObject.Find("Asteroid Spawner").GetComponent<AsteroidGenerator>().AsteroidDestroyed();
            explosionAudio = GameObject.Find("ExplosionSounds");
            int explosionSoundNumber = Random.Range(1, explosionAudio.transform.childCount + 1);
            AudioSource explosionSound = GameObject.Find("Explosion" + explosionSoundNumber.ToString()).GetComponent<AudioSource>();
            explosionSound.Play();
            GetComponent<CircleCollider2D>().enabled = false;
            animator.SetTrigger("OnDestroyed");
            Destroy(other.gameObject);
            speed = 1;
            Destroy(gameObject, 2.2f);
        }
    }

    /// <summary>
    /// Sets the asteroids's speed
    /// </summary>
    /// <param name="speed">Speed value</param>
    public void SetAsteroidSpeed(float speed)
    {
        this.speed = speed;
    }

    /// <summary>
    /// Sets the asteroids's rotation speed
    /// </summary>
    /// <param name="rotationSpeed">Rotation speed value</param>
    public void SetAsteroidRotationSpeed(float rotationSpeed)
    {
        this.rotationSpeed = rotationSpeed;
    }
}
