using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGenerator : MonoBehaviour
{
    // Asteroid Prefab
    [Tooltip("Asteroid Prefab")]
    [SerializeField]
    private GameObject asteroid;

    // Minimum seconds passed before generating a new asteroid
    [Tooltip("Minimum seconds passed before generating a new asteroid")]
    [SerializeField]
    private float minSecondsToSpawnNewAsteroid;

    // Maximum seconds passed before generating a new asteroid
    [Tooltip("Maximum seconds passed before generating a new asteroid")]
    [SerializeField]
    private float maxSecondsToSpawnNewAsteroid;


    // The minimum limit of Maximum seconds passed before generating a new asteroid
    [Tooltip("The minimum limit of Maximum seconds passed before generating a new asteroid")]
    [SerializeField]
    private float maxSecondsToSpawnNewAsteroidLimit;

    // The number of asteroids that must be destoryed in order to begin spawning asteroids faster
    [Tooltip("The number of asteroids that must be destoryed in order to begin spawning asteroids faster")]
    [SerializeField]
    private int asteroidsPerLevel;

    // Minimum asteroid speed
    [Tooltip("Minimum asteroid speed")]
    [SerializeField]
    private float minSpeed;

    // Maximum asteroid speed
    [Tooltip("Maximum asteroid speed")]
    [SerializeField]
    private float maxSpeed;

    // The maximum limit of Maximum asteroid speed
    [Tooltip("The maximum limit of Maximum asteroid speed")]
    [SerializeField]
    private float maxSpeedLimit;

    // Minimum asteroid rotation speed
    [Tooltip("Minimum asteroid rotation speed")]
    [SerializeField]
    private float minRotationSpeed;

    // Maximum asteroid rotation speed
    [Tooltip("Maximum asteroid rotation speed")]
    [SerializeField]
    private float maxRotationSpeed;

    // Minimum asteroid size
    [Tooltip("Minimum asteroid size")]
    [SerializeField]
    private float minSize;

    // Maximum asteroid size
    [Tooltip("Maximum asteroid size")]
    [SerializeField]
    private float maxSize;

    // Initial asteroidsPerLevel value
    private int initialAsteroidsPerLevel;

    // If player is alive or not
    private bool isPlayerAlive = true;

    // Boundaries of the screen for any resolution
    private Vector2 screenBounds;

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        initialAsteroidsPerLevel = asteroidsPerLevel;

        // Generating asteroids at random time intervals
        StartCoroutine(AsteroidGeneratorRoutine());
    }

    /// <summary>
    /// Coroutine for generating asteroids at random time intervals
    /// </summary>
    /// <returns></returns>
    IEnumerator AsteroidGeneratorRoutine()
    {
        while (isPlayerAlive)
        {
            // List of all positions of every active enemy
            List<float> activeEnemyPositionX = new();
            foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
                activeEnemyPositionX.Add(e.transform.position.x);

            // List of all positions of every active asteroid
            List<float> activeAsteroidPositionX = new();
            foreach (GameObject e in GameObject.FindGameObjectsWithTag("Asteroid"))
                activeAsteroidPositionX.Add(e.transform.position.x);

            // New asteroid
            GameObject newAsteroid = Instantiate(asteroid, new Vector3(1000, 1000, 1000), Quaternion.identity);

            // Boolean for controlling wther a new asteroid can be spawned
            bool canCreateAsteroid = true;

            // Randomizing the new asteroid's size
            float asteroidSize = Random.Range(minSize, maxSize);
            newAsteroid.transform.localScale = new Vector3(asteroidSize, asteroidSize, asteroidSize);

            // Dimensions of the new asteroid
            float asteroidWidth = newAsteroid.GetComponent<Asteroid>().asteroidWidth;
            float asteroidHeight = newAsteroid.GetComponent<Asteroid>().asteroidHeight;

            // Randomizing the position of the new asteroid on the horizontal axis
            Vector3 newAsteroidPosition = new(Random.Range(screenBounds.x + asteroidWidth, -screenBounds.x - asteroidWidth), -screenBounds.y * (asteroidHeight / 1.1f), 0f);

            // Checking whether there is enough room for a new asteroid to spawn
            restart:
                // Checking all active enemy positions
                for (int i = 0; i < activeEnemyPositionX.Count; i++)
                    if (newAsteroidPosition.x >= activeEnemyPositionX[i] - asteroidWidth && newAsteroidPosition.x <= activeEnemyPositionX[i] + asteroidWidth)
                    {
                        newAsteroidPosition = new Vector3(Random.Range(screenBounds.x + asteroidWidth, -screenBounds.x - asteroidWidth), -screenBounds.y * (asteroidHeight / 1.1f), 0f);
                        if (i == activeEnemyPositionX.Count - 1)
                            canCreateAsteroid = false;
                        else
                            goto restart;
                    }
                if (canCreateAsteroid)
                    // Checking all active asteroid positions
                    for (int i = 0; i < activeAsteroidPositionX.Count; i++)
                        if (newAsteroidPosition.x >= activeAsteroidPositionX[i] - asteroidWidth && newAsteroidPosition.x <= activeAsteroidPositionX[i] + asteroidWidth)
                        {
                            newAsteroidPosition = new Vector3(Random.Range(screenBounds.x + asteroidWidth, -screenBounds.x - asteroidWidth), -screenBounds.y * (asteroidHeight / 1.1f), 0f);
                            if (i == activeAsteroidPositionX.Count - 1)
                                canCreateAsteroid = false;
                            else
                                goto restart;
                        }

            if (canCreateAsteroid)
            {
                newAsteroid.transform.position = newAsteroidPosition;

                activeEnemyPositionX.Add(newAsteroidPosition.x);

                float speed = Random.Range(minSpeed, maxSpeed);
                newAsteroid.GetComponent<Asteroid>().SetAsteroidSpeed(speed);

                float rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
                newAsteroid.GetComponent<Asteroid>().SetAsteroidRotationSpeed(rotationSpeed);

            }
            else
                Destroy(newAsteroid);

            yield return new WaitForSeconds(Random.Range(minSecondsToSpawnNewAsteroid, maxSecondsToSpawnNewAsteroid));
        }
    }



    /// <summary>
    /// Called when the player dies
    /// </summary>
    public void PlayerDied()
    {
        isPlayerAlive = false;
        // Destroying all enemies once the player has died
        foreach (GameObject e in GameObject.FindGameObjectsWithTag("Asteroid"))
            Destroy(e);
    }

    /// <summary>
    /// Decreases the minimum and Maximum limits of spawining a new asteroid every time x asteroids die
    /// </summary>
    public void AsteroidDestroyed()
    {
        asteroidsPerLevel--;
        if (asteroidsPerLevel == 0)
        {
            asteroidsPerLevel = initialAsteroidsPerLevel;
            if (maxSecondsToSpawnNewAsteroid > maxSecondsToSpawnNewAsteroidLimit)
            {
                minSecondsToSpawnNewAsteroid -= 0.1f;
                minSecondsToSpawnNewAsteroid = Mathf.Round(minSecondsToSpawnNewAsteroid * 10) / 10;
                maxSecondsToSpawnNewAsteroid -= 0.1f;
                maxSecondsToSpawnNewAsteroid = Mathf.Round(maxSecondsToSpawnNewAsteroid * 10) / 10;
            }
            if (maxSpeed < maxSpeedLimit)
            {
                minSpeed += 0.1f;
                minSpeed = Mathf.Round(minSpeed * 10) / 10;
                maxSpeed += 0.1f;
                maxSpeed = Mathf.Round(maxSpeed * 10) / 10;
            }
        }
    }
}
