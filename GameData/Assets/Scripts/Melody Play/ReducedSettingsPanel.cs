using UnityEngine;

// Settings panel toggle with pause integration.
public class ReducedSettingsPanel : MonoBehaviour
{
    public GameObject settingsPanel; 
    public PauseManager pauseManager;
    private bool isPanelOpen = false;

    // Initialize panel state.
    void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        if (pauseManager == null)
            pauseManager = FindObjectOfType<PauseManager>();
    }

    // Toggle settings panel visibility and pause state.
    public void ToggleSettingsPanel()
    {
        isPanelOpen = !isPanelOpen;
        if (settingsPanel != null)
            settingsPanel.SetActive(isPanelOpen);
        if (pauseManager != null)
            pauseManager.SetPanelPause(isPanelOpen);
    }
}
