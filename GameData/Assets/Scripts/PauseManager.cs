using UnityEngine;
using UnityEngine.UI;

// Controls game pause state, updated by button or pannal,
public class PauseManager : MonoBehaviour
{
    // UI components for pause button.
    [Header("Pause Button UI")]
    public Sprite pauseIcon;
    public Sprite playIcon;
    public Image buttonImage;

    // Pause state tracking
    public static bool IsPaused { get; private set; } // Current puse state
    public static bool ByButton = false;
    public static bool ByPanel = false;

    private SingleNoteRecognitionManager noteManager;

    void Start()
    {
        noteManager = FindObjectOfType<SingleNoteRecognitionManager>();
    }

    // Called by pause button.
    public void TogglePause()
    {
        ByButton = !ByButton;
        UpdateGamePauseState();
        UpdateButtonIcon();
    }

    // Called by panels/menus
    public void SetPanelPause(bool isPaused)
    {
        ByPanel = isPaused;
        UpdateGamePauseState();
    }

    // Updates the actual game pause state based on all pause sources
    private void UpdateGamePauseState()
    {
        bool shouldBePaused; // What the pause state should be.
        shouldBePaused = ByButton || ByPanel;

        // Check if puse state need to be updated
        if (shouldBePaused != IsPaused)
        {
            IsPaused = shouldBePaused;
            Time.timeScale = IsPaused ? 0f : 1f; // Freeze/unfreeze time.

            // Notify manager of pause state changes.
            if (IsPaused)
                noteManager?.PauseGame("System");
            else
                noteManager?.ResumeGame("System");
        }
    }

    // Updates the pause button icon.
    private void UpdateButtonIcon()
    {
        if (buttonImage != null)
            buttonImage.sprite = ByButton ? playIcon : pauseIcon;
    }

    // Debug method to check current state!!!!!!!!!!!!!!!!!!!!!!
    public void LogCurrentState()
    {
        Debug.Log($"Game Paused: {IsPaused}, By Button: {ByButton}, By Level Panel: {ByPanel}");
    }
}
