using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    // Enemy Prefab
    [Tooltip("Enemy Prefab")]
    [SerializeField]
    private GameObject enemy;

    // Minimum seconds passed before generating a new enemy
    [Tooltip("Minimum seconds passed before generating a new enemy")]
    [SerializeField]
    private float minSecondsToSpawnNewEnemy;

    // Maximum seconds passed before generating a new enemy
    [Tooltip("Maximum seconds passed before generating a new enemy")]
    [SerializeField]
    private float maxSecondsToSpawnNewEnemy;

    // The minimum limit of Maximum seconds passed before generating a new enemy
    [Tooltip("The minimum limit of Maximum seconds passed before generating a new enemy")]
    [SerializeField]
    private float maxSecondsToSpawnNewEnemyLimit;

    // Minimum enemy speed
    [Tooltip("Minimum enemy speed")]
    [SerializeField]
    private float minSpeed;

    // The minimum limit of Maximum enemy speed
    [Tooltip("Maximum enemy speed")]
    [SerializeField]
    private float maxSpeed;

    // The maximum limit of Maximum enemy speed
    [Tooltip("The maximum limit of Maximum enemy speed")]
    [SerializeField]
    private float maxSpeedLimit;

    // The number of enemies that must be destoryed in order to begin spawning enemies faster
    [Tooltip("The number of enemies that must be destoryed in order to begin spawning enemies faster")]
    [SerializeField]
    private int enemiesPerLevel;

    // Minimum enemy fire rate
    [Tooltip("Minimum enemy fire rate")]
    [SerializeField]
    private float minFireRate;

    // Maximum enemy fire rate
    [Tooltip("Maximum enemy fire rate")]
    [SerializeField]
    private float maxFireRate;

    // Maximum enemy fire rate limit
    [Tooltip("Maximum enemy fire rate limit")]
    [SerializeField]
    private float maxFireRateLimit;

    // Initial enemiesPerLevel value
    private int initialEnemiesPerLevel;

    // If player is alive or not
    private bool isPlayerAlive = true;

    // Boundaries of the screen for any resolution
    private Vector2 screenBounds;

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    void Start()
    {
        initialEnemiesPerLevel = enemiesPerLevel;
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        // Generating enemies at random time intervals
        StartCoroutine(EnemyGeneratorRoutine());
    }

    /// <summary>
    /// Coroutine for generating enemies at random time intervals
    /// </summary>
    /// <returns></returns>
    IEnumerator EnemyGeneratorRoutine()
    {
        while (isPlayerAlive)
        {
            // List of all positions of every active enemy
            List<float> activeEnemyPositionX = new();
            foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
                activeEnemyPositionX.Add(e.transform.position.x);

            // New Enemy
            GameObject newEnemy = Instantiate(enemy, new Vector3(1000, 1000, 1000), Quaternion.identity);

            // Boolean for controlling wther a new enemy can be spawned
            bool canCreateEnemy = true;

            // Dimensions of the new enemy
            float enemyWidth = newEnemy.GetComponent<Enemy>().enemyWidth;
            float enemyHeight = newEnemy.GetComponent<Enemy>().enemyHeight;
            // Randomizing the position of the new enemy on the horizontal axis
            Vector3 newEnemyPosition = new(Random.Range(screenBounds.x + enemyWidth, -screenBounds.x - enemyWidth), -screenBounds.y * (enemyHeight / 1.08f), 0f);

            // Checking whether there is enough room for a new enemy to spawn
            restart:
                // Checking all active enemy positions
                for (int i = 0; i < activeEnemyPositionX.Count; i++)
                    if (newEnemyPosition.x >= activeEnemyPositionX[i] - enemyWidth && newEnemyPosition.x <= activeEnemyPositionX[i] + enemyWidth)
                    {
                        newEnemyPosition = new Vector3(Random.Range(screenBounds.x + enemyWidth, -screenBounds.x - enemyWidth), -screenBounds.y * enemyHeight, 0f);
                        if (i == activeEnemyPositionX.Count - 1)
                            canCreateEnemy = false;
                        else
                            goto restart;
                    }

            // Creating the new enemy if there was enough room
            if (canCreateEnemy)
            {
                newEnemy.transform.position = newEnemyPosition;
                float speed = Random.Range(minSpeed, maxSpeed);
                newEnemy.GetComponent<Enemy>().SetEnemySpeed(speed);
                float fireRate = Random.Range(minFireRate, maxFireRate);
                newEnemy.GetComponent<Enemy>().SetEnemyFireRate(fireRate);
            }
            else
                Destroy(newEnemy);

            yield return new WaitForSeconds(Random.Range(minSecondsToSpawnNewEnemy, maxSecondsToSpawnNewEnemy));
        }
    }

    /// <summary>
    /// Called when the player dies
    /// </summary>
    public void PlayerDied()
    {
        isPlayerAlive = false;
        // Destroying all enemies once the player has died
        foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
           Destroy(e);
    }

    /// <summary>
    /// Decreases the minimum and Maximum limits of spawining a new enemy and increases their fire rate every time x enemies die
    /// </summary>
    public void EnemyDestroyed()
    {
        enemiesPerLevel--;
        if (enemiesPerLevel == 0)
        {
            enemiesPerLevel = initialEnemiesPerLevel;
            if (maxSecondsToSpawnNewEnemy > maxSecondsToSpawnNewEnemyLimit)
            {
                minSecondsToSpawnNewEnemy -= 0.1f;
                minSecondsToSpawnNewEnemy = Mathf.Round(minSecondsToSpawnNewEnemy * 10) / 10;
                maxSecondsToSpawnNewEnemy -= 0.1f;
                maxSecondsToSpawnNewEnemy = Mathf.Round(maxSecondsToSpawnNewEnemy * 10) / 10;
            }
            if (maxSpeed < maxSpeedLimit)
            {
                minSpeed += 0.1f;
                minSpeed = Mathf.Round(minSpeed * 10) / 10;
                maxSpeed += 0.1f;
                maxSpeed = Mathf.Round(maxSpeed * 10) / 10;
            }
            if (maxFireRate > maxFireRateLimit)
            {
                minFireRate -= 0.1f;
                minFireRate = Mathf.Round(minFireRate * 10) / 10;
                maxFireRate -= 0.1f;
                maxFireRate = Mathf.Round(maxFireRate * 10) / 10;
            }
        }
    }
}
