using UnityEngine;
using TMPro;

/// Captures keyboard input for text entry and displays it using a TMP_Text component.
/// Intended for username or simple string input in UI.
public class ComputerKeyboardInput : MonoBehaviour
{
    [Header("Display")]
    public TMP_Text displayText;

    private string currentText = "";
    private bool isActive = true; // Always active now

    private TouchScreenKeyboard mobileKeyboard;  // Unity's on-screen keyboard
    private bool isMobileInput = false;          // Flag to track if using mobile keyboard


    /// Initialize text display when the object starts.
    void Start()
    {
        UpdateDisplay();
    }

    /// Updates per frame, using the appropriate input method for platform.
    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
    HandleMobileInput();
#else
        HandleDesktopInput();
#endif
    }
    /// Handles desktop input using Input.inputString.
    private void HandleDesktopInput()
    {
        bool textChanged = false; // Track if any changes occurred

        foreach (char c in Input.inputString)
        {
            if (c == '\b' && currentText.Length > 0) // Backspace
            {
                currentText = currentText.Substring(0, currentText.Length - 1);
                textChanged = true;
            }
            else if (char.IsLetterOrDigit(c) || c == ' ') // Valid characters
            {
                if (currentText.Length < 20) // Max length
                {
                    currentText += c;
                    textChanged = true;
                }
            }
        }

        if (textChanged)
            UpdateDisplay();
    }
    /// Handles mobile input using TouchScreenKeyboard.
    private void HandleMobileInput()
    {
        // Open keyboard on tap (if not already open)
        if (mobileKeyboard == null && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            mobileKeyboard = TouchScreenKeyboard.Open(currentText, TouchScreenKeyboardType.Default, false, false, false, false);
        }

        // Update the text from mobile keyboard
        if (mobileKeyboard != null && mobileKeyboard.status == TouchScreenKeyboard.Status.Visible)
        {
            if (mobileKeyboard.text != currentText)
            {
                currentText = mobileKeyboard.text.Substring(0, Mathf.Min(mobileKeyboard.text.Length, 20));
                UpdateDisplay();
            }
        }
    }

    /// Sets the current text programmatically.
    /// name="text" The text to set as input
    public void SetText(string text)
    {
        currentText = text;
        UpdateDisplay();
    }


    /// Returns the current text string.

    /// returns The user-entered text
    public string GetText()
    {
        return currentText;
    }

    /// Updates the TMP_Text component to reflect the current input.
    /// Shows a placeholder when empty, otherwise shows the typed text with a "|" cursor.
    private void UpdateDisplay()
    {
        if (string.IsNullOrEmpty(currentText))
        {
            displayText.text = "Type username here...";
            displayText.color = Color.white;
        }
        else
        {
            displayText.text = currentText + "|"; // Show cursor
            displayText.color = Color.gray;
        }
    }
}