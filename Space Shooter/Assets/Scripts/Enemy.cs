using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Probability for spawning a power-up
    [Tooltip("Total probability for spawning a power-up (1-100).")]
    [SerializeField]
    private int powerUpProbability;

    // List of all available power-ups
    [Tooltip("List of all available power-ups")]
    [SerializeField]
    private List<GameObject> powerUps = new();

    // Enemy Laser
    [Tooltip("Enemy laser")]
    [SerializeField]
    private GameObject laser;

    // Points awared to the player if they hit an enemy with a laser
    [Tooltip("Points awared to the player if they hit an enemy with a laser")]
    [SerializeField]
    private int pointsForLaserHit;

    // Points awared to the player if they hit an enemy with their ship
    [Tooltip("Points awared to the player if they hit an enemy with their ship")]
    [SerializeField]
    private int pointsForShipHit;

    // Enemy fire rate
    private float fireRate;

    // Time passed after the last bullet was fired by the enemy
    private float lastFired;

    // Explosion Sounds
    private GameObject explosionAudio;

    // Player Object
    private Player player;

    // Width of the enemy
    [HideInInspector]
    public float enemyWidth;

    // Height of the enemy
    [HideInInspector]
    public float enemyHeight;

    // Boundaries of the screen for any resolution
    private Vector2 screenBounds;

    // Enemy speed
    private float speed;

    // Enemy Animator
    private Animator animator;

    // Boolean containing whether the enemy is destroyed or not
    [HideInInspector]
    public bool destroyed = false;

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        player = GameObject.Find("Player").GetComponent<Player>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Called when an enemy is generated
    /// </summary>
    private void OnEnable()
    {
        enemyWidth = transform.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        enemyHeight = transform.GetComponent<SpriteRenderer>().bounds.size.y / 2;
    }

    /// <summary>
    /// Called once per frame
    /// </summary>
    void Update()
    {
        // Enemy movement
        Movement();

        // Enemy shooting
        StartCoroutine(Shooting());
    }

    /// <summary>
    /// Enemy movement
    /// </summary>
    private void Movement()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.down);

        if (transform.position.y < screenBounds.y - enemyHeight)
        {
            // Finding all active enemy positions
            List<float> activeEnemyPositionX = new();
            foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
                activeEnemyPositionX.Add(e.transform.position.x);

            bool canResetEnemy = true;

            float xResetPosition = Random.Range(screenBounds.x + enemyWidth, -screenBounds.x - enemyWidth);
            // Resetting an enemy who has goen off screen to their default spawn position on the y axis if there is enough space
            restart:
            for (int i = 0; i < activeEnemyPositionX.Count; i++)
                if (xResetPosition >= activeEnemyPositionX[i] - enemyWidth && xResetPosition <= activeEnemyPositionX[i] + enemyWidth)
                {
                    xResetPosition = Random.Range(screenBounds.x + enemyWidth, -screenBounds.x - enemyWidth);
                    if (i == activeEnemyPositionX.Count - 1)
                        canResetEnemy = false;
                    else
                        goto restart;
                }

            if (canResetEnemy)
                transform.position = new Vector3(xResetPosition, -screenBounds.y * enemyHeight, 0f);
            else
                Destroy(gameObject);
        }
    }

    /// <summary>
    /// Spawns a power-up based on a given probability
    /// </summary>
    private void SpawnPowerUp()
    {
        if (gameObject.scene.isLoaded)
        {
            int randomNumber = Random.Range(1, 100);
            if (randomNumber <= powerUpProbability) {
                int powerUpInedx = Random.Range(0, powerUps.Count);
                Instantiate(powerUps[powerUpInedx], transform.position, Quaternion.identity);
            }          
        }
    }

    /// <summary>
    /// Coroutine for enemy shooting
    /// </summary>
    /// <returns></returns>
    IEnumerator Shooting()
    {
        while (true)
        {
            if (Time.time > lastFired + fireRate)
            {
                GameObject currentLaser = Instantiate(laser, transform.position, Quaternion.identity);
                currentLaser.GetComponent<EnemyLaser>().speed += speed;

                
                AudioSource currentLaserSound = GameObject.Find("EnemyLaserSound").GetComponent<AudioSource>();
                currentLaserSound.Play();
                lastFired = Time.time;
            }
            yield return null;
        }
    }

        /// <summary>
        /// Called when the attached object collides with another collider
        /// </summary>
        /// <param name="other">Collider of other object</param>
        private void OnTriggerEnter2D(Collider2D other)
    {
        // Destroying enemy if hit by a bullet
        if (other.CompareTag("Laser"))
        {
            fireRate = 100;
            GameObject.Find("Enemy Spawner").GetComponent<EnemyGenerator>().EnemyDestroyed();
            explosionAudio = GameObject.Find("ExplosionSounds");
            int explosionSoundNumber = Random.Range(1, explosionAudio.transform.childCount + 1);
            AudioSource explosionSound = GameObject.Find("Explosion" + explosionSoundNumber.ToString()).GetComponent<AudioSource>();
            explosionSound.Play();
            GetComponent<BoxCollider2D>().enabled = false;
            Destroy(other.gameObject);
            Destroy(GetComponent<Transform>().GetChild(0).gameObject, 0.1f);
            SpawnPowerUp();
            animator.SetTrigger("OnDestroyed");
            speed = 1f;
            Destroy(gameObject, 2.4f);
            player.IncreasePlayerScore(pointsForLaserHit);
        }
        // Destroying enemy if hit by the player
        else if (other.CompareTag("Player"))
        {
            fireRate = 100;
            GameObject.Find("Enemy Spawner").GetComponent<EnemyGenerator>().EnemyDestroyed();
            explosionAudio = GameObject.Find("ExplosionSounds");
            int explosionSoundNumber = Random.Range(1, explosionAudio.transform.childCount + 1);
            AudioSource explosionSound = GameObject.Find("Explosion" + explosionSoundNumber.ToString()).GetComponent<AudioSource>();
            explosionSound.Play();
            GetComponent<BoxCollider2D>().enabled = false;
            Destroy(GetComponent<Transform>().GetChild(0).gameObject, 0.1f);
            SpawnPowerUp();
            animator.SetTrigger("OnDestroyed");
            speed = 1;
            Destroy(gameObject, 2.4f);
            player.IncreasePlayerScore(pointsForShipHit);
        }
        // Destroying asteroid and enemy if they collide
        else if (other.CompareTag("Asteroid"))
        {
            if (other.gameObject.GetComponent<Asteroid>().canDestroyEnemy)
            {
                fireRate = 100;
                GameObject.Find("Enemy Spawner").GetComponent<EnemyGenerator>().EnemyDestroyed();
                explosionAudio = GameObject.Find("ExplosionSounds");
                int explosionSoundNumber = Random.Range(1, explosionAudio.transform.childCount + 1);
                AudioSource explosionSound = GameObject.Find("Explosion" + explosionSoundNumber.ToString()).GetComponent<AudioSource>();
                explosionSound.Play();
                GetComponent<BoxCollider2D>().enabled = false;
                Destroy(GetComponent<Transform>().GetChild(0).gameObject, 0.1f);
                SpawnPowerUp();
                animator.SetTrigger("OnDestroyed");
                speed = 1;
                Destroy(gameObject, 2.4f);
            }
        }
    }

    /// <summary>
    /// Sets the enemy's speed
    /// </summary>
    /// <param name="speed">Speed value</param>
    public void SetEnemySpeed(float speed)
    {
        this.speed = speed;
    }

    /// <summary>
    /// Sets the enemy's fire rate
    /// </summary>
    /// <param name="speed">Fire rate value</param>
    public void SetEnemyFireRate(float fireRate)
    {
        this.fireRate = fireRate;
    }
}
