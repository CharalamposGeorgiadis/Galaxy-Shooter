using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ReBindUi : MonoBehaviour
{
    // Reference to the action to be rebound
    [Tooltip("Reference to the action to be rebound")]
    [SerializeField]
    private InputActionReference inputActionReference;

    // Disable binding the mouse to movement controls
    [Tooltip("Disable binding the mouse to movement controls")]
    [SerializeField]
    private bool excludeMouse = true;

    // Select key binding for actions that have multiple bindings
    [Tooltip("Select key binding for actions that have multiple bindings")]
    [Range(0, 10)]
    [SerializeField]
    private int selectedBinding;

    // For formatting the binding name
    [Tooltip("For formatting the binding name")]
    [SerializeField]
    private InputBinding.DisplayStringOptions displayStringOptions;

    // Input Binding info
    [Header("Binding Info - DO NOT EDIT")]
    [SerializeField]
    private InputBinding inputBinding;

    // Binding index
    private int bindingIndex;

    // Action name
    private string actionName;

    [Header("UI Fields")]

    // Action Text
    [Tooltip("Action text")]
    [SerializeField]
    private TextMeshProUGUI actionText;

    // Re-binding button
    [Tooltip("Re-binding button")]
    [SerializeField]
    private Button rebindingButton;

    // Re-bind button text
    [Tooltip("Re-bind button text")]
    [SerializeField]
    private TextMeshProUGUI rebindText;

    // Reset button
    [Tooltip("Reset button")]
    [SerializeField]
    private Button resetButton;

    // Rebind Overlay
    [Tooltip("Rebind Overlay")]
    [SerializeField]
    private GameObject overlayPanel;

    /// <summary>
    /// Called when a value is changed in the inspector
    /// </summary>
    private void OnValidate()
    {
        if (inputActionReference == null)
            return;
        GetBindingInfo();
        UpdateUI();
    }

    /// <summary>
    /// Called when the object becomes active
    /// </summary>
    private void OnEnable()
    {
        rebindingButton.onClick.AddListener(() => DoRebind());
        resetButton.onClick.AddListener(() => ResetBinding());

        if (inputActionReference != null)
        {
            GetBindingInfo();
            InputManager.LoadBindingOverride(actionName);
            UpdateUI();
        }

        InputManager.RebindComplete += UpdateUI;
        InputManager.RebindCancelled += UpdateUI;
    }

    /// <summary>
    /// Called when the object becomes inactive
    /// </summary>
    private void OnDisable()
    {
        InputManager.RebindComplete -= UpdateUI;
        InputManager.RebindCancelled -= UpdateUI;
    }

    /// <summary>
    /// Gets binding info
    /// </summary>
    private void GetBindingInfo()
    {
        if (inputActionReference != null)
            actionName = inputActionReference.action.name;

        if (inputActionReference.action.bindings.Count > selectedBinding)
        {
            inputBinding = inputActionReference.action.bindings[selectedBinding];
            bindingIndex = selectedBinding;
        }
    }

    /// <summary>
    /// Updates UI
    /// </summary>
    private void UpdateUI()
    {
        if (actionText != null)
            if (inputBinding.name == "")
                actionText.SetText(actionName.ToUpper());
            else
                actionText.SetText(inputBinding.name.ToUpper());

        if (rebindText != null)
        {
            if (Application.isPlaying)
            {
                // Grab info from Input Manager                
                string newButtonText = InputManager.GetBindingName(actionName, bindingIndex);
                if (newButtonText.Contains("Press"))
                    newButtonText = newButtonText.Replace("Press ", "");
                rebindText.SetText(newButtonText);
            }
            else
            {
                string newButtonText = inputActionReference.action.GetBindingDisplayString(bindingIndex);
                if (newButtonText.Contains("Press"))
                    newButtonText = newButtonText.Replace("Press ", "");
                rebindText.SetText(newButtonText); 
            }
        }
    }

    /// <summary>
    /// Rebinds a control
    /// </summary>
    private void DoRebind()
    {
        overlayPanel.SetActive(true);
        if (actionName.Equals("Fire"))
            InputManager.StartRebind("HoldToFire", bindingIndex, rebindText, excludeMouse, overlayPanel);
        InputManager.StartRebind(actionName, bindingIndex, rebindText, excludeMouse, overlayPanel);
    }

    /// <summary>
    /// Resets binding of a control
    /// </summary>
    private void ResetBinding()
    {
        if (actionName.Equals("Fire"))
            InputManager.ResetBinding("HoldToFire", bindingIndex);
        InputManager.ResetBinding(actionName, bindingIndex);
        UpdateUI();
    }
}
