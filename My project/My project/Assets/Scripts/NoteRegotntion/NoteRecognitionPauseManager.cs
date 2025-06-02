/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteRecognitionPauseManager : MonoBehaviour
{

    public Sprite pauseIcon;
    public Sprite playIcon;
    public Image buttonImage;

    public static bool IsGamePaused { get; private set; } = false;

    private SingleNoteRecognitionManager noteManager;

    void Start()
    {
        if (noteManager == null)
            noteManager = FindObjectOfType<SingleNoteRecognitionManager>();
    }


    public void TogglePause()
    {
        IsGamePaused = !IsGamePaused;
        Time.timeScale = IsGamePaused ? 0f : 1f;

        if (IsGamePaused)
            noteManager?.PauseGame("PuseBTN");
        else
            noteManager?.ResumeGame("PuseBTN");

        if (buttonImage != null)
            buttonImage.sprite = IsGamePaused ? playIcon : pauseIcon;
    }



}*/
// Updated NoteRecognitionPauseManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteRecognitionPauseManager : MonoBehaviour
{
    public Sprite pauseIcon;
    public Sprite playIcon;
    public Image buttonImage;

    public static bool IsGamePaused { get; private set; } = false;
    private static bool PausedByButton = false; // Pause button state
    private static bool PausedByLevelPanel = false; // Level panel state

    private SingleNoteRecognitionManager noteManager;

    void Start()
    {
        if (noteManager == null)
            noteManager = FindObjectOfType<SingleNoteRecognitionManager>();
    }

    public void TogglePause()
    {
        PausedByButton = !PausedByButton;
        UpdateGamePauseState();
        UpdateButtonIcon();
    }

    public void SetLevelPanelPause(bool isPaused)
    {
        PausedByLevelPanel = isPaused;
        UpdateGamePauseState();
    }

    private void UpdateGamePauseState()
    {
        bool shouldBePaused = PausedByButton || PausedByLevelPanel;

        if (shouldBePaused != IsGamePaused)
        {
            IsGamePaused = shouldBePaused;
            Time.timeScale = IsGamePaused ? 0f : 1f;

            if (IsGamePaused)
                noteManager?.PauseGame("System");
            else
                noteManager?.ResumeGame("System");
        }
    }

    private void UpdateButtonIcon()
    {
        if (buttonImage != null)
            buttonImage.sprite = PausedByButton ? playIcon : pauseIcon;
    }

    // Debug method to check current state
    public void LogCurrentState()
    {
        Debug.Log($"Game Paused: {IsGamePaused}, By Button: {PausedByButton}, By Level Panel: {PausedByLevelPanel}");
    }
}

