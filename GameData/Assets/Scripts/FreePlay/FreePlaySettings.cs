using UnityEngine;

/// Handles the Free Play settings panel logic, including showing/hiding the UI
/// and updating the user's preferred piano size.
public class FreePlaySettings : MonoBehaviour
{
    [Header("Panel Reference")]
    public GameObject settingsPanel;
    public FreePlayGameManager gameManager;


    private bool isPanelVisible = false;

    /// Toggles the visibility of the settings panel.
    /// Called when the settings button is clicked.
    public void ToggleSettingsPanel()
    {
        isPanelVisible = !isPanelVisible;
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(isPanelVisible);
        }

    }
    /// Called when the user selects a new piano size.
    /// Saves the preference and updates the piano layout.
    public void SetPianoSize(int size)
    {
        if (UserManager.CurrentUser == null) return;

        UserManager.UpdatePianoSize(size);
        Debug.Log($"[FreePlaySettings] Piano size updated to: {size}");

        // Apply the change to the piano layout
        if (gameManager != null)
            gameManager.SendMessage("SetupPianoSize");

        // Optional: Close the panel after applying
        ToggleSettingsPanel();
    }
}
