using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages logic of Confrim ID UI Screen
/// </summary>
public class UIConfirmIDController : MonoBehaviour
{
    public TMP_Text UserID;
    int selectedUserID;
    /// <summary>
    /// Activates/Deactivates the screen
    /// </summary>
    /// <param name="option"></param>
    public void SetActive(bool option, string userID = "0")
    {
        if (UserID != null)
        {
            this.gameObject.SetActive(option);
            UserID.text = userID;
        }
    }
    public void setUserID(int _userID)
    {
        selectedUserID = _userID;
    }
    public int getUserID()
    {
        return selectedUserID;
    }
}
