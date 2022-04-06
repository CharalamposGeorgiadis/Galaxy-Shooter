using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    // Pause Menu UI
    [Tooltip("Pause Menu UI")]
    [SerializeField]
    private GameObject pauseMenu;

    // Main screnn of the Pause Menu
    [Tooltip("Main screnn of the Pause Menu")]
    [SerializeField]
    private GameObject mainPauseMenu;

    // Options Menu UI
    [Tooltip("Options Menu UI")]
    [SerializeField]
    private GameObject optionsMenu;

    // Controls Menu UI
    [Tooltip("Controls Menu UI")]
    [SerializeField]
    private GameObject controlsMenu;

    // Game Over screen
    [Tooltip("Game Over screen")]
    [SerializeField]
    private GameObject gameOverScreen;

    // Player input actions
    private PlayerInputActions playerControls;

    // Parent of the current active submenu back button
    private GameObject parentMenu;

    // Boolean containing whether the game is paused or not
    public static bool gameIsPaused = false;

    // Pause key
    private InputAction pause;

    /// <summary>
    /// Called when the script instance is loaded
    /// </summary>
    private void Awake()
    {
        playerControls = InputManager.inputActions;

        pause = playerControls.UI.Pause;

        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }


    /// <summary>
    /// Called when the Player object becomes active
    /// </summary>
    private void OnEnable()
    {
        playerControls.Enable();
    }

    /// <summary>
    /// Called once per frame
    /// </summary>
    void Update()
    {
        if (!gameOverScreen.activeInHierarchy)
            if (pause.triggered)
                if (gameIsPaused)
                    Resume();
                else
                    Pause();
    }

    /// <summary>
    /// Resumes the game
    /// </summary>
    public void Resume()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        gameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Pauses the game
    /// </summary>
    private void Pause()
    {
        pauseMenu.SetActive(true);
        mainPauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        gameIsPaused = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0f;      
    }

    /// <summary>
    /// Returns to the main pause menu
    /// </summary>
    public void BackToMainPauseMenu()
    {
        parentMenu.SetActive(false);
        mainPauseMenu.SetActive(true);
    }

    /// <summary>
    ///  Opens the options menu
    /// </summary>
    public void OptionsMenu()
    {
        mainPauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
        parentMenu = optionsMenu;
    }

    /// <summary>
    ///  Opens the controls menu
    /// </summary>
    public void ControlsMenu()
    {
        mainPauseMenu.SetActive(false);
        controlsMenu.SetActive(true);
        parentMenu = controlsMenu;
    }

    /// <summary>
    ///  Returns to Main Meny
    /// </summary>
    public void MainMenu()
    {
        pauseMenu.SetActive(false);
        gameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
