using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Player lives sprites
    [Tooltip("Player lives sprites")]
    [SerializeField]
    private List<Sprite> playerLivesSprites;

    // Background images
    [Tooltip("Background images (stored as materials)")]
    [SerializeField]
    private List<Material> backgroundImages;

    // In-Game UI
    [Tooltip("In-Game UI")]
    [SerializeField]
    private GameObject inGameUi;

    // Game Over screen
    [Tooltip("Game Over screen")]
    [SerializeField]
    private GameObject gameOverScreen;

    // Player Object
    private Player player;

    // Score text
    private TextMeshProUGUI scoreText;

    // Player Lives image
    private Image playerLivesImage;

    // Current player life index
    private int currentPlayerLifeIndex;

    // Current Player score
    private string playerScore;

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Adding a random background
        GameObject.Find("Background").GetComponent<Image>().material = backgroundImages[Random.Range(0, backgroundImages.Count)];

        player = GameObject.Find("Player").GetComponent<Player>();

        // Initializing score text
        scoreText = inGameUi.transform.Find("Score").GetComponent<TextMeshProUGUI>();

        // Initializing Player lives image
        playerLivesImage = inGameUi.transform.Find("Lives").GetComponent<Image>();
        playerLivesImage.sprite = playerLivesSprites[^1];
        currentPlayerLifeIndex = playerLivesSprites.Count - 1;
    }

    /// <summary>
    /// Called once per frame
    /// </summary>
    void Update()
    {
        // Updating UI elements
        UpdateUI();
    }

    /// <summary>
    /// Updates UI elements
    /// </summary>
    private void UpdateUI()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            // Changing the score text if the Player's score changes
            if (!scoreText.text.Equals(player.GetPlayerScore().ToString()))
            {
                playerScore = player.GetComponent<Player>().GetPlayerScore().ToString();
                scoreText.SetText(playerScore);
            }

            // Changing the Player lives image if the Player's lives change
            if (player.GetPlayerLives() != currentPlayerLifeIndex)
            {
                playerLivesImage.sprite = playerLivesSprites[player.GetPlayerLives()];
                currentPlayerLifeIndex = player.GetPlayerLives();
            }
        }
        // Modifying UI elements once the player dies
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            inGameUi.SetActive(false);
            gameOverScreen.SetActive(true);
            gameOverScreen.transform.Find("GameOverScore").GetComponent<TextMeshProUGUI>().SetText("Score: " + playerScore);
        }
    }
}
