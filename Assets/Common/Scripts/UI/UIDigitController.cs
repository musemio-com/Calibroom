using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the behaviour of an UI Digit for Calibroom
/// </summary>
public class UIDigitController : MonoBehaviour
{
    /// <summary>
    /// The digit to modify (as string)
    /// </summary>
    public TMP_Text Digit;
    /// <summary>
    /// The digit to modify (as int)
    /// </summary>
    private int m_DigitInt;

    private void OnEnable()
    {
        // Make sure to match the string to the int
        UpdateUIDigit(ref Digit, m_DigitInt);
    }

    private void UpdateUIDigit(ref TMP_Text digit, int digitInt)
    {
        // Make sure to init ui digit
        if (digit == null)
            digit = GetComponent<TMP_Text>();

        digit.text = digitInt.ToString();

    }

    /// <summary>
    /// Adds an amount to the digit
    /// </summary>
    public void AddAmount(int amount)
    {
        m_DigitInt += amount;
        
        if (m_DigitInt > 9)
            m_DigitInt = 9;
        else if (m_DigitInt < 0)
            m_DigitInt = 0;
        
        UpdateUIDigit(ref Digit, m_DigitInt);
    }

    public string GetDigit()
    {
        UpdateUIDigit(ref Digit, m_DigitInt);
        if (Digit != null)
            return Digit.text;
        else
            return "0";
    }

    public int GetDigitInt()
    {
        return m_DigitInt;
    }
}
