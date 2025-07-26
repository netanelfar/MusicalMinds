using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages note recognition game settings UI.
public class NoteRecognitionSettings : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject DifficultyPNL;
    public GameObject HintPNL;

    [Header("Manager References")]
    public GameObject SettingsPNL;
    public FeedbackManager feedback;
    public PauseManager pause;
    public SingleNoteRecognitionManager noteManager;

    // Find local managers.
    void Awake()
    {
        feedback = GetComponent<FeedbackManager>();  
        pause = GetComponent<PauseManager>();        
    }

    // Shows difficulty panel.
    public void OpenDifficultyPanel()
    {
        if (HintPNL != null) HintPNL.SetActive(false);
        if (DifficultyPNL != null) DifficultyPNL.SetActive(true);
    }

    // Shows hint panel.
    public void OpenHintPanel()
    {
        if (DifficultyPNL != null)
        {
            DifficultyPNL.SetActive(false);
        }
        if (HintPNL != null)
        {
            HintPNL.SetActive(true);
        }
    }

    // Enables hints, closes panels, and update pause manager.
    public void EnableHints()
    {
        UserManager.UpdateNoteRecHint(1);
        SettingsPNL.SetActive(false );
        HintPNL.SetActive(false);
        DifficultyPNL.SetActive(true);
        if (pause != null)
        {
            pause.SetPanelPause(false);
        }
    }

    // Disable hints, closes panels, and update pause manager.
    public void DisableHints()
    {
        UserManager.UpdateNoteRecHint(0);
        SettingsPNL.SetActive(false);
        HintPNL.SetActive(false);
        DifficultyPNL.SetActive(true);
        if (pause!= null)
        {
            pause.SetPanelPause(false);
        }
    }

    // Toggles main settings panel UI and pause state.
    public void ToggleSettingsPanel()
    {
        bool isActive = SettingsPNL.activeSelf;
        SettingsPNL.SetActive(!isActive);

        if (pause != null)
        {
            pause.SetPanelPause(!isActive);
        }
    }

    // Changes difficulty level and restarts game.
    public void SetLevel(int level)
    {
        noteManager.StopAllCoroutines();
        noteManager.CancelInvoke();
        KeyControl.inputAllowed = false;
        noteManager.noteTimer?.StopTimer();
        noteManager.feedback.ClearFeedback();
        UserManager.UpdateNoteRecognitionLevel(level);

        SettingsPNL.SetActive(false);
        if (pause != null)
        {
            pause.SetPanelPause(false);
        }

        noteManager.RestartLevel();
        Debug.Log("Note level changed to: " + level);      
    }

}
