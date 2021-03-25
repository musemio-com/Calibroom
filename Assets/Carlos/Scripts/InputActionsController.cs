using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the behaviour of Input Actions
/// </summary>
public class InputActionsController : MonoBehaviour
{
    /// <summary>
    /// Collection of action events available in the input system (generated through the editor)
    /// </summary>
    public CalibroomInputActions CalibroomActions;

    private void Awake()
    {
        // Create instance on awake
        if (CalibroomActions == null)
            CalibroomActions = new CalibroomInputActions();

        // Assign a delegate with the ExitGame() method to the ExitGame action
        CalibroomActions.Menu.ExitGame.performed += x => QuitGame();
    }

    private void OnEnable()
    {
        // We need to manually enable input actions
        CalibroomActions.Enable();
    }

    private void OnDisable()
    {
        // We need to manually disable input actions
        CalibroomActions.Disable();
    }

    /// <summary>
    /// Quits the game completely
    /// </summary>
    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        // Account for editor applications by forcing to leave playmode
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }
}
