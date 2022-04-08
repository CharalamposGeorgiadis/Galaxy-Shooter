using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class for controlling the player
/// </summary>
public class Player : MonoBehaviour
{
    // Player starting position
    [Tooltip("Player starting position")]
    [SerializeField]
    private Vector3 playerStartingPosition;

    // Player lives
    [Tooltip("Player Lives")]
    [SerializeField]
    private int lives;

    // Player speed
    [Tooltip("Player speed")]
    [SerializeField]
    private float speed;

    // Player fire rate
    [Tooltip("Player fire rate")]
    [SerializeField]
    private float fireRate;

    // List containing all Laser Projectile Prefabs
    [Tooltip("List containing all Laser Projectile Prefabs")]
    [SerializeField]
    private List<GameObject> laserProjectiles;

    // Laser position offset from the player object when fired
    [Tooltip("Laser position offset from the player object when fired")]
    [SerializeField]
    private Vector3 laserOffset;

    // Amount of speed increase/decrease when the player collects a speed increase power-up
    [Tooltip("Amount of speed increase/decrease when the player collects a speed increase power-up")]
    [SerializeField]
    private float extraSpeed;

    // Player shield
    [Tooltip("Player shield")]
    [SerializeField]
    private GameObject shield;

    // Amount of score required for the player to gain one extra life
    [Tooltip("Amount of score required for the player to gain one extra life")]
    [SerializeField]
    private int scoreForExtraLife;

    // Enemy Generator object
    [Tooltip("Enemy Generator object")]
    [SerializeField]
    private GameObject enemySpawner;

    // Asteroid Generator object
    [Tooltip("Asteroid Generator object")]
    [SerializeField]
    private GameObject asteroidSpawner;

    // Game Object respnsible for saving the Player's highscore
    [Tooltip("Game Object respnsible for saving the Player's highscore")]
    [SerializeField]
    private GameObject scoreManager;

    // Player input actions
    [Tooltip("Player input actions")]
    [SerializeField]
    private PlayerInputActions playerControls;

    // Immunity time for when the player gets hit
    [Tooltip("Immunity time for when the player gets hit")]
    [SerializeField]
    private float immunityTime;

    // Frequency of player blinking while they are immune
    [Tooltip("Frequency of player blinking while they are immune")]
    [SerializeField]
    private float blinkingFrequency;

    // Player score
    private int score;

    // Original amount of score required for the player to gain one extra life
    private int originalScoreForExtraLife;

    // Time passed after the last bullet was fired by the player
    private float lastFired;

    // Width of the player
    private float playerWidth;

    // Height of the player
    private float playerHeight;

    // Boundaries of the screen for any resolution
    private Vector2 screenBounds;

    // List of booleans for the player's weapon power-up. ONLY ONE CAN BE TRUE
    readonly List<bool> weaponUpgrade = new();

    // Current length of time that the player has been immune for
    private float currentImmuneTime;

    // Current length of time that the player has been blinking for
    private float currentBlinkTime = 0.1f;

    // Thruster and Damage renderers
    private List<SpriteRenderer> playerRenderers;

    // Movement input action
    private InputAction move;

    // Movement direction of the Player
    private Vector3 moveDirection;

    // Boolean containing whether the Player is pressing/holding down the fire button
    public bool isShooting;

    /// <summary>
    /// Called when the script instance is loaded
    /// </summary>
    private void Awake()
    {
        playerControls = InputManager.inputActions;

        move = playerControls.Player.Move;

        playerControls.Player.Fire.performed += x => ShootPressed();
        playerControls.Player.HoldToFire.performed += x => ShootHeld();
    }

    /// <summary>
    /// Called when the Player object becomes active
    /// </summary>
    private void OnEnable()
    {
        playerControls.Enable();
    }

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    void Start()
    {
        weaponUpgrade.Add(true);
        for (int i = 1; i < laserProjectiles.Count; i++)
            weaponUpgrade.Add(false);
        transform.position = playerStartingPosition;
        transform.Find("Shield").gameObject.SetActive(false);
        transform.Find("LeftWingDamage").gameObject.SetActive(false);
        transform.Find("RightWingDamage").gameObject.SetActive(false);
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        playerWidth = transform.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        playerHeight = transform.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        playerRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
        originalScoreForExtraLife = scoreForExtraLife;
    }

    /// <summary>
    /// Called once per frame
    /// </summary>
    void Update()
    {
        // Moving the player if a movement key is pressed
        PlayerMovement();

        // Increasing the Player's lifes by 1 if a certain score has been reached
        IncreasePlayerLives();

        // Player Shooting
        Shooting();

        // Player immunity when they get hit
        Immunity();
    }

    /// <summary>
    /// Controls the player's movement
    /// </summary>
    void PlayerMovement()
    {
        // Moving the player
        moveDirection = move.ReadValue<Vector2>();
        transform.Translate(speed * Time.deltaTime * moveDirection);

        // Preventing the player from going outside the screen
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, screenBounds.x + playerWidth, -screenBounds.x - playerWidth),
                  Mathf.Clamp(transform.position.y, screenBounds.y + playerHeight, -screenBounds.y + playerHeight / 2), 0f);  
    }

    /// <summary>
    /// Sets the isShooting value to True if the Player is pressing/holding down the Fire button
    /// </summary>
    private void ShootPressed() {
        isShooting = true;
    }

    /// <summary>
    /// Sets the isShooting value to False if the Player ahs stopped pressing/holding down the Fire button
    /// </summary>
    private void ShootHeld()
    {
        isShooting = false;
    }

    /// <summary>
    /// Shooting when a Fire button is pressed/held down
    /// </summary>
    private void Shooting()
    {
        // Shooting if the game is not paused
        if (!PauseMenu.gameIsPaused) {
            if (isShooting)
            {
                if (Time.time > lastFired + fireRate)
                {
                    for (int i = 0; i < weaponUpgrade.Count; i++)
                    {
                        if (weaponUpgrade[i])
                        {
                            Instantiate(laserProjectiles[i], transform.position + laserOffset, Quaternion.identity);
                            GameObject.Find("LaserSounds").GetComponentsInChildren<AudioSource>()[i].Play();
                            lastFired = Time.time;
                            break;
                        }
                    }
                }
            }
    }
}

    /// <summary>
    /// Called when the attached object collides with another collider
    /// </summary>
    /// <param name="other">Collider of other object</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Damaging or destroying player if hit by an enemy
        if (currentImmuneTime <= 0)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Asteroid") || other.CompareTag("EnemyLaser"))
            {
                if (other.CompareTag("EnemyLaser"))
                {
                    GameObject hitSound;
                    if (transform.Find("Shield").gameObject.activeInHierarchy)
                        hitSound = GameObject.Find("ShieldHitSound");
                    else
                        hitSound = GameObject.Find("PlayerHitSound");
                    hitSound.GetComponent<AudioSource>().Play();
                    Destroy(other.gameObject);
                }
                if (transform.Find("Shield").gameObject.activeInHierarchy)
                    transform.Find("Shield").gameObject.SetActive(false);
                else
                {
                    lives--;
                    DowngradeWeapon();
                    DecreasePlayerSpeed(extraSpeed);
                    DecreasePlayerFireRate(0.05f);
                    switch (lives)
                    {
                        case 0:
                            if (enemySpawner != null)
                                enemySpawner.GetComponent<EnemyGenerator>().PlayerDied();
                            if (asteroidSpawner != null)
                                asteroidSpawner.GetComponent<AsteroidGenerator>().PlayerDied();
                            StartCoroutine(scoreManager.GetComponent<ScoreSaveManager>().SubmitScore(score));
                            Destroy(gameObject);
                            break;
                        case 1:
                            transform.Find("RightWingDamage").gameObject.SetActive(true);
                            playerRenderers.Add(transform.Find("RightWingDamage").gameObject.GetComponent<SpriteRenderer>());
                            currentImmuneTime = immunityTime;
                            foreach (SpriteRenderer r in playerRenderers)
                                r.enabled = false;
                            currentBlinkTime = blinkingFrequency;
                            break;
                        case 2:
                            transform.Find("LeftWingDamage").gameObject.SetActive(true);
                            playerRenderers.Add(transform.Find("LeftWingDamage").gameObject.GetComponent<SpriteRenderer>());
                            currentImmuneTime = immunityTime;
                            foreach (SpriteRenderer r in playerRenderers)
                                r.enabled = false;
                            currentBlinkTime = blinkingFrequency;
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Activates the collected power-up
    /// </summary>
    /// <param name="powerUpName">Name of the collected power-up</param>
    public void ActivatePowerUp(string powerUpName)
    {
        IncreasePlayerScore(100);
        switch (powerUpName)
        {
            case "WeaponPowerUp":
                UpgradeWeapon();
                break;
            case "SpeedPowerUp":
                IncreasePlayerSpeed(extraSpeed);
                break;
            case "ShieldPowerUp":
                AddPlayerShield();
                break;
        }
    }

    /// <summary>
    /// Upgrades player's weapon
    /// </summary>
    public void UpgradeWeapon()
    {
        for (int i = 0; i < laserProjectiles.Count; i++)
        {
            if (weaponUpgrade[i])
            {
                if (i != weaponUpgrade.Count - 1)
                {
                    weaponUpgrade[i] = false;
                    weaponUpgrade[i + 1] = true;
                }
                else
                    IncreasePlayerFireRate(0.1f);
                break;
            }
        }
    }

    /// <summary>
    /// Downgrades player's weapon
    /// </summary>
    public void DowngradeWeapon()
    {
        for (int i = laserProjectiles.Count - 1; i > 0; i--)
            if (weaponUpgrade[i])
                if (i != 0)
                {
                    weaponUpgrade[i] = false;
                    weaponUpgrade[i - 1] = true;
                }
    }

    /// <summary>
    /// Increases player's speed
    /// </summary>
    /// <param name="speedIncrease">Amount of speed increase</param>
    private void IncreasePlayerSpeed(float speedIncrease)
    {
        speed += speedIncrease;
    }

    /// <summary>
    /// Decreases player's speed
    /// </summary>
    /// <param name="speedDecrease">Amount of speed decrease</param>
    private void DecreasePlayerSpeed(float speedDecrease)
    {
        speed -= speedDecrease;
    }

    /// <summary>
    /// Increases the player's fire rate. The value of fireRateIncrease is subtracted since we want to decrease the amount of time between shots in the Shooting() function
    /// </summary>
    /// <param name="fireRateIncrease">Amount of fire rate increase</param>
    private void IncreasePlayerFireRate(float fireRateIncrease)
    {
        fireRate -= fireRateIncrease;
        if (fireRate < 0.15f)
        {
            fireRate = 0.15f;
            IncreasePlayerScore(100);
        }
    }

    /// <summary>
    /// Decreases the player's fire rate. The value of fireRateIncrease is added since we want to decrease the amount of time between shots in the Shooting() function
    /// </summary>
    /// <param name="fireRateDecrease">Amount of fire rate Decrease</param>
    private void DecreasePlayerFireRate(float fireRateDecrease)
    {
        fireRate += fireRateDecrease;
    }

    /// <summary>
    /// Adds a shield around the player
    /// </summary>
    private void AddPlayerShield()
    {
        if (transform.Find("Shield").gameObject.activeInHierarchy)
            IncreasePlayerScore(100);
        else
            transform.Find("Shield").gameObject.SetActive(true);
    }

    /// <summary>
    /// Gets the player's score
    /// </summary>
    /// <returns>Player score</returns>
    public int GetPlayerScore()
    {
        return score;
    } 

    /// <summary>
    /// Increases the player's core
    /// </summary>
    /// <param name="scoreIncrease">Amount of score increase</param>
    public void IncreasePlayerScore(int scoreIncrease)
    {
        score += scoreIncrease;
    }

    /// <summary>
    /// Gets the Player's lives
    /// </summary>
    /// <returns>Player lives</returns>
    public int GetPlayerLives()
    {
        return lives;
    }

    /// <summary>
    /// Increases the player's lives by 1 if they reach a certain score
    /// </summary>
    private void IncreasePlayerLives()
    {
        if (score >= scoreForExtraLife)
        {
            scoreForExtraLife += originalScoreForExtraLife;
            if (lives < 3)
            {
                lives++;
                AudioSource extraLifeSound = GameObject.Find("ExtraLifePowerUpSound").GetComponent<AudioSource>();
                extraLifeSound.Play();              
                switch (lives)
                {
                    case 2:
                        playerRenderers.Remove(transform.Find("RightWingDamage").gameObject.GetComponent<SpriteRenderer>());
                        transform.Find("RightWingDamage").gameObject.SetActive(false);
                        break;
                    case 3:
                        playerRenderers.Remove(transform.Find("LeftWingDamage").gameObject.GetComponent<SpriteRenderer>());
                        transform.Find("LeftWingDamage").gameObject.SetActive(false);
                        break;
                }
            }
            else
            {
                IncreasePlayerFireRate(0.1f);
                IncreasePlayerSpeed(1f);
                IncreasePlayerScore(100);
            }
        }      
    }

    /// <summary>
    /// Player immunity when they get hit
    /// </summary>
    private void Immunity()
    {
        if (currentImmuneTime > 0)
        {
            currentImmuneTime -= Time.deltaTime;
            currentBlinkTime -= Time.deltaTime;

            if (currentBlinkTime <= 0)
            {
                foreach(SpriteRenderer r in playerRenderers)
                    r.enabled = !r.enabled;
                currentBlinkTime = blinkingFrequency;
            }

            if (currentImmuneTime <= 0)
                foreach (SpriteRenderer r in playerRenderers)
                    r.enabled = true;  
        }
    }
}
