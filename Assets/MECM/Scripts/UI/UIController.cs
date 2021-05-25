using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages UI Flow
/// </summary>
public class UIController : MonoBehaviour
{
    /// <summary>
    /// Screen in which the user selects the ID
    /// </summary>
    public UISelectIDController SelectIDScreen;
    /// <summary>
    /// Screen where user can see and confirm their ID, or go back and edit it
    /// </summary>
    public UIConfirmIDController ConfirmIDScreen;

    private void OnEnable()
    {
        LoadSelectIDScreen();
    }

    /// <summary>
    /// Opens up the select id screen 
    /// </summary>
    public void LoadSelectIDScreen()
    {
        // Select screen visible
        if (SelectIDScreen != null)
            SelectIDScreen.SetActive(true);
        // Confirm scren invisible at start
        if (ConfirmIDScreen != null)
            ConfirmIDScreen.SetActive(false);
    }

    /// <summary>
    /// Opens up the confirm id screen and prepares all its parameters
    /// </summary>
    public void LoadConfirmIDScreen()
    {
        if(SelectIDScreen != null && ConfirmIDScreen != null)
        {
            ConfirmIDScreen.SetActive(true, SelectIDScreen.GetUserID());
            SelectIDScreen.SetActive(false);
        }
    }


}
