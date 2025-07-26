using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Toggle whether system presses have color and saves user preferences.
public class systemColorToggle : MonoBehaviour
{
    public Toggle sColorToggle;
    public GameObject settingsPanel; 
    public PauseManager pauseManager;

    // Loads user Toggle color preference.
    void OnEnable()
    {
        if (UserManager.CurrentUser != null && sColorToggle != null)
        {
            sColorToggle.isOn = UserManager.CurrentUser.systemPressHasColor;
            sColorToggle.onValueChanged.RemoveAllListeners();
            sColorToggle.onValueChanged.AddListener(OnSystemColorToggleChanged);
        }
    }

    // Saves the new preference, closes settings panel, and resumes game.
    public void OnSystemColorToggleChanged(bool isOn)
    {
        if (UserManager.CurrentUser != null)
        {
            UserManager.CurrentUser.systemPressHasColor = isOn;
            UserManager.SaveCurrentUserToFile();
        }
        // Close settings panel.
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // Update pause manager.
        if (pauseManager != null)
            pauseManager.SetPanelPause(false);
       
    } 
}


