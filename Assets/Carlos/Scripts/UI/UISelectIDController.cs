using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages logic of Select ID UI Screen
/// </summary>
public class UISelectIDController : MonoBehaviour
{
    public UIDigitController DigitLeft;
    public UIDigitController DigitRight;

    /// <summary>
    /// Activates/Deactivates the screen
    /// </summary>
    /// <param name="option"></param>
    public void SetActive(bool option)
    {
        this.gameObject.SetActive(option);
    }

    public string GetUserID()
    {
        if(DigitLeft!=null && DigitRight!=null)
        {
            string userID = DigitLeft.GetDigit() + DigitRight.GetDigit();
            return userID;
        }
        return "0";
    }

}
