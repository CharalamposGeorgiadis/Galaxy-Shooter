using UnityEngine;
using UnityEngine.InputSystem;
using System;
using TMPro;

public class InputManager : MonoBehaviour
{
    // Player Input Actions
    public static PlayerInputActions inputActions;

    // Action event for when the rebinding process is complete
    public static event Action RebindComplete;

    // Action event for when the rebinding process is cancelled
    public static event Action RebindCancelled;

    // Action event for when the rebinding process has started
    public static event Action<InputAction, int> RebindStarted;

    /// <summary>
    /// Called when the script instance is being loaded
    /// </summary>
    void Awake()
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();
    }

    /// <summary>
    /// Begins rebinding process
    /// </summary>
    /// <param name="actionName">Name of the Input action that is being rebound</param>
    /// <param name="bindingIndex">Index of the Input action that is being rebound</param>
    /// <param name="buttonText">Text of the button that is currently pressed</param>
    /// <param name="excludeMouse">Boolean containing whether mouse buttons can be bound to keyboard Input Actions (Example: Movement)</param>
    /// <param name="overlayPanel">Binding overlay panel</param>
    public static void StartRebind(string actionName, int bindingIndex, TextMeshProUGUI buttonText, bool excludeMouse, GameObject overlayPanel)
    {
        InputAction action = inputActions.asset.FindAction(actionName);
        if (action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Could not find action or binding");
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
            {
                DoRebind(action, firstPartIndex, buttonText, true, excludeMouse, overlayPanel);
            }
        }
        else
        {
            DoRebind(action, bindingIndex, buttonText, false, excludeMouse, overlayPanel);
        }
    }

    /// <summary>
    /// Rebinds a control
    /// </summary>
    /// <param name="actionToRebind">Input action to rebind</param>
    /// <param name="bindingIndex">Index of the Input action</param>
    /// <param name="buttonText">Text of the button that is currently pressed</param>
    /// <param name="allCompositeParts">Boolean containing whether the Input action that is being rebound is composite or not</param>
    /// <param name="excludeMouse">Boolean containing whether mouse buttons can be bound to keyboard Input Actions (Example: Movement)</param>
    /// <param name="overlayPanel">Binding overlay panel</param>
    private static void DoRebind(InputAction actionToRebind, int bindingIndex, TextMeshProUGUI buttonText, bool allCompositeParts, bool excludeMouse, GameObject overlayPanel)
    {
        if (actionToRebind == null || bindingIndex < 0)
        {
            return;
        }
            
        buttonText.SetText("Press a Button");

        actionToRebind.Disable();

        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);

        rebind.OnComplete(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();

            if (allCompositeParts)
            {
                var nextBindingIndex = bindingIndex + 1;
                if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isPartOfComposite)
                    DoRebind(actionToRebind, nextBindingIndex, buttonText, allCompositeParts, excludeMouse, overlayPanel);
            }

            SaveBindingOverride(actionToRebind);
            overlayPanel.SetActive(false);
            RebindComplete?.Invoke();
        });



        rebind.OnCancel(operation => {
            overlayPanel.SetActive(false);
            actionToRebind.Enable();
            operation.Dispose();

            RebindCancelled?.Invoke();
        });

        rebind.WithCancelingThrough("<Keyboard>/escape");

        if (excludeMouse)
            rebind.WithControlsExcluding("Mouse");

        // Begin Rebinding process
        RebindStarted?.Invoke(actionToRebind, bindingIndex);
        rebind.Start();
    }

    /// <summary>
    /// Gets Binding Name
    /// </summary>
    /// <param name="actionName">Name of the Input Action that is being rebound</param>
    /// <param name="bindingIndex">Index of the Input Action that is being rebound</param>
    /// <returns>Name of the binding of the Input Action that is being rebound</returns>
    public static string GetBindingName(string actionName, int bindingIndex)
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();

        InputAction action = inputActions.asset.FindAction(actionName);
        return action.GetBindingDisplayString(bindingIndex);
    }

    /// <summary>
    /// Saves the new bindings to the Player's preferences
    /// </summary>
    /// <param name="action">Rebound Input Action</param>
    private static void SaveBindingOverride(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
    }

    /// <summary>
    /// Loads the bindings saved to the Player's preferences
    /// </summary>
    /// <param name="actionName">Name of the rebound Input Action</param>
    public static void LoadBindingOverride(string actionName)
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();

        InputAction action = inputActions.asset.FindAction(actionName);
        for (int i = 0; i < action.bindings.Count; i++)
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
                action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
    }

    /// <summary>
    /// Resets chosen binding
    /// </summary>
    /// <param name="actionName">Name of the Input Action whose binding is being reset</param>
    /// <param name="bindingIndex">Index of the binding that is being reset</param>
    public static void ResetBinding(string actionName, int bindingIndex)
    {
        InputAction action = inputActions.asset.FindAction(actionName);

        if (action == null || action.bindings.Count <= bindingIndex)
            Debug.Log("Could not find action or binding");

        if (action.bindings[bindingIndex].isComposite)
            for (int i = bindingIndex; i < action.bindings.Count && action.bindings[i].isComposite; i++)
                action.RemoveBindingOverride(i);
        else
            action.RemoveBindingOverride(bindingIndex);

        SaveBindingOverride(action);
    }
}
