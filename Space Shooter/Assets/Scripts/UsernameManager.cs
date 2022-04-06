using UnityEngine;
using TMPro;
using LootLocker.Requests;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using LootLocker;

public class UsernameManager : MonoBehaviour
{
    // Username Input Field
    [Tooltip("Username Input Field")]
    [SerializeField]
    private TMP_InputField usernameField;

    // Password Input Field
    [Tooltip("Password Input Field")]
    [SerializeField]
    private TMP_InputField passwordField;

    // View/Hide password Button text
    [Tooltip("View/Hide password Button text")]
    [SerializeField]
    private TextMeshProUGUI showPasswordButtonText;

    // Inappropriate username error message
    [Tooltip("Inappropriate username error message")]
    [SerializeField]
    private GameObject errorMessage;

    // Player input actions
    private PlayerInputActions playerControls;

    // Entered username
    public static string username = "";

    // Enter password
    private string password = "";

    // Enter key
    private InputAction enter;

    // Signing method
    private string signingMethod;

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    private void Start()
    {
        usernameField.GetComponent<EventTrigger>().enabled = false;
        passwordField.GetComponent<EventTrigger>().enabled = false;

        signingMethod = SigningMethod.signingMethod;
    }


    /// <summary>
    /// Called when the script instance is loaded
    /// </summary>
    private void Awake()
    {
        playerControls = InputManager.inputActions;
        enter = playerControls.UI.Enter;
    }

    /// <summary>
    /// Called when the Player object becomes active
    /// </summary>
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Update()
    {
        if (enter.IsPressed())
            Sign();
    }

    /// <summary>
    /// Disables the "Enter Username... when the Player clicks inside the Input Text Field
    /// </summary>
    public void OnUsernameSelect()
    {
        if (usernameField.placeholder.GetComponent<TextMeshProUGUI>().text == "Username...")
            usernameField.placeholder.GetComponent<TextMeshProUGUI>().text = "";
    }

    /// <summary>
    /// Enables the "Enter Username... when the Player clicks outside the Input Text Field if it is empty
    /// </summary>
    public void OnUsernameDeselect()
    {
        usernameField.GetComponent<EventTrigger>().enabled = true;
        if (usernameField.placeholder.GetComponent<TextMeshProUGUI>().text == "")
            usernameField.placeholder.GetComponent<TextMeshProUGUI>().text = "Username...";
    }

    /// <summary>
    /// Disables the "Enter Password... when the Player clicks inside the Input Text Field
    /// </summary>
    public void OnPasswordSelect()
    {
        if (passwordField.placeholder.GetComponent<TextMeshProUGUI>().text == "Password...")
            passwordField.placeholder.GetComponent<TextMeshProUGUI>().text = "";
    }

    /// <summary>
    /// Enables the "Enter Username... when the Player clicks outside the Input Text Field if it is empty
    /// </summary>
    public void OnPasswordDeselect()
    {
        usernameField.GetComponent<EventTrigger>().enabled = true;
        if (passwordField.placeholder.GetComponent<TextMeshProUGUI>().text == "")
            passwordField.placeholder.GetComponent<TextMeshProUGUI>().text = "Password...";
    }

    /// <summary>
    /// Show/Hide password
    /// </summary>
    public void ShowPassword()
    {
        if (passwordField.contentType == TMP_InputField.ContentType.Password)
        {
            passwordField.contentType = TMP_InputField.ContentType.Standard;
            passwordField.ForceLabelUpdate();
            showPasswordButtonText.SetText("SHOW");
        }
        else if (passwordField.contentType == TMP_InputField.ContentType.Standard)
        {
            passwordField.contentType = TMP_InputField.ContentType.Password;
            passwordField.ForceLabelUpdate();
            showPasswordButtonText.SetText("HIDE");
        }
    }

    /// <summary>
    /// Validating the Player's entered username and/or password
    /// </summary>
    /// <returns>Boolean containing whether the players has entered correct credentials or not</returns>
    private bool ValidateCredentials()
    {
        username = usernameField.text;
        password = passwordField.text;
        if (string.IsNullOrWhiteSpace(username))
            errorMessage.GetComponent<TextMeshProUGUI>().SetText("ENTER AN APPROPRIATE USERNAME!");
        else if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            errorMessage.GetComponent<TextMeshProUGUI>().SetText("PASSWORD MUST BE 8-CHARACTERS LONG!");
        else
            return true;
        return false;
    }

    /// <summary>
    /// Sign up or sign in baased on the selected signing method
    /// </summary>
    public void Sign()
    {
         if (ValidateCredentials())
         {
             if (signingMethod.Equals("SignIn"))
                 SignIn();
             else if (signingMethod.Equals("SignUp"))
                 SignUp();
         }
    }

    /// <summary>
    /// Sign in
    /// </summary>
    private void SignIn()
    {
        LootLockerSDKManager.WhiteLabelLogin(username, password, false, response =>
        {
            if (!response.success)
            {
                Debug.Log("Error while signing in");
                errorMessage.GetComponent<TextMeshProUGUI>().SetText("WRONG USERNAME OR PASSWORD");
                return;
            }
            else
            {
                string token = response.SessionToken;
                LootLockerSDKManager.EndSession("LeaderboardPlayer", response =>
                {
                    if (!response.success)
                    {
                        Debug.Log("Guest session did not end");
                        return;
                    }
                    else
                    {
                        LootLockerSDKManager.StartWhiteLabelSession(username, password, response =>
                        {
                            errorMessage.SetActive(false);
                            ChangeScene change = gameObject.AddComponent<ChangeScene>();
                            change.GetComponent<ChangeScene>().setSceneName("LoadingScreen");
                            change.GetComponent<ChangeScene>().OnButtonClick();
                        });
                    }
                });
            }
        });
    }

    /// <summary>
    /// Sign up
    /// </summary>
    private void SignUp()
    {
        LootLockerSDKManager.WhiteLabelSignUp(username, password, response =>
        {
            if (!response.success)
            {
                Debug.Log("Error while signing up");;
                errorMessage.GetComponent<TextMeshProUGUI>().SetText("USERNAME ALREADY EXISTS");
                return;
            }
            else
            {
                LootLockerSDKManager.EndSession("LeaderboardPlayer", response =>
                {
                    if (!response.success)
                    {
                        Debug.Log("Guest session did not end");
                        return;
                    }
                    else
                    {
                        LootLockerSDKManager.StartWhiteLabelSession(username, password, response =>
                        {
                            PlayerNameRequest name = new();
                            name.name = username;
                            LootLockerAPIManager.SetPlayerName(name, response =>
                            {
                                if (!response.success)
                                {
                                    Debug.Log("Failed to set player name");
                                    return;
                                }
                                else
                                {
                                    errorMessage.SetActive(false);
                                    ChangeScene change = gameObject.AddComponent<ChangeScene>();
                                    change.GetComponent<ChangeScene>().setSceneName("LoadingScreen");
                                    change.GetComponent<ChangeScene>().OnButtonClick();
                                }
                            });
                        });
                    }
                });
            }
        });
    }
}
