using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class SingleNoteRecognitionManager : MonoBehaviour
{
    public KeyControl keyControl;
    public TextMeshProUGUI feedbackText;
    public TimerSliderDisplay noteTimer;
    public GameObject levelPNL;

    private string targetNote;
    private int wrongAttempts = 0;
    private float noteStartTime = 0f;
    private bool waitingForInput = false;
    private bool isSystemPlayingNote = false;
    
    private bool PausedByBTN = false;
    private bool PausedByPNl = false; //level panal
    
    private int notesAttempted = 0;
    private int notesCorrect = 0;
    private int notesPerLevel = 10;

    public GameObject endOfLevelPanel; 
    public TextMeshProUGUI CorrectNotesTXT;
    public TextMeshProUGUI EndLevelFeedbackTXT;





    private List<string> allNotes = new List<string>
    {
        "Low C", "Low Cs", "Low D", "Low Ds", "Low E", "Low F", "Low Fs", "Low G", "Low Gs", "Low A", "Low As", "Low B",
        "Up C", "Up Cs", "Up D", "Up Ds", "Up E", "Up F", "Up Fs", "Up G", "Up Gs", "Up A", "Up As", "Up B"
    };

    void Start()
    {
        GameSettings.CurrentGameMode = GameSettings.GameMode.SingleNoteRecognition;
        noteTimer.OnTimerFinished = HandleTimeOut;
        Invoke(nameof(PickRandomNote), 1f);

        if (CurrentUserManager.CurrentUser != null)
        {
            Debug.Log($"Entered Sound Match: {CurrentUserManager.CurrentUser.username}");
        }
    }

    public void PauseGame(string reason)
    {
        if (reason == "PuseBTN")
            PausedByBTN = true;
        else if (reason == "LevelPNL")
            PausedByPNl = true;

        if (PausedByBTN || PausedByPNl)
        {
            noteTimer?.PauseTimer();
            KeyControl.inputAllowed = false;
            waitingForInput = false;
            ShowFeedback("Game Paused", Color.yellow);
        }
    }

    public void ResumeGame(string reason)
    {
        if (reason == "PuseBTN")
            PausedByBTN = false;
        else if (reason == "LevelPNL")
            PausedByPNl = false;

        if (!PausedByBTN && !PausedByPNl)
        {
            noteTimer?.ResumeTimer();
            KeyControl.inputAllowed = true;
            waitingForInput = true;
            ClearFeedback();
        }
    }


    public void PickRandomNote()
    {
        CancelInvoke(nameof(ClearFeedback));
        ClearFeedback();

        KeyControl.inputAllowed = false;
        int randomIndex = Random.Range(0, allNotes.Count);
        targetNote = allNotes[randomIndex];

        wrongAttempts = 0;
        noteStartTime = Time.time;
        isSystemPlayingNote = true;
        waitingForInput = false;

        keyControl.PressAsSystem(targetNote);
        Invoke(nameof(StartPlayerInputAndTimer), 0.4f); //dealy after system play note
    }

    /*private void StartPlayerInputAndTimer()
    {
        isSystemPlayingNote = false;
        waitingForInput = true;
        KeyControl.inputAllowed = true;
        noteTimer.StartTimer();
    }*/

    private void StartPlayerInputAndTimer()
    {
        isSystemPlayingNote = false;
        waitingForInput = true;
        KeyControl.inputAllowed = true;

        int level = CurrentUserManager.CurrentUser.Notelvl;
        switch (level)
        {
            case 1: noteTimer.maxTime = 6f; break;  // Easy
            case 2: noteTimer.maxTime = 4f; break;  // Normal
            case 3: noteTimer.maxTime = 2.5f; break;  // Hard
            default: noteTimer.maxTime = 4f; break;
        }

        noteTimer.StartTimer();
    }


    /*public bool OnPlayerPressedNote(string note)
    {
        if (!waitingForInput || isSystemPlayingNote)
            return false;

        if (note == targetNote)
        {
            notesCorrect++;
            noteTimer.StopTimer();
            ShowFeedback("Correct!", Color.green);
            CalculatePointsAddition();

            waitingForInput = false;
            KeyControl.inputAllowed = false;
            noteTimer.PlayFillAnimation();
            Invoke(nameof(PickRandomNote), 2f);
            return true;
        }
        else
        {
            wrongAttempts++;
            StartCoroutine(ReplayTargetNote());
            keyControl.ShakeKey(note);
            return false;
        }

        if (notesAttempted >= notesPerLevel)
        {
            Invoke(nameof(ShowEndOfLevelPanel), 2f);
        }
        else
        {
            Invoke(nameof(PickRandomNote), 2f);
        }

        return note == targetNote;

    }*/

    public bool OnPlayerPressedNote(string note)
    {
        if (!waitingForInput || isSystemPlayingNote)
            return false;

        notesAttempted++;

        if (note == targetNote)
        {
            notesCorrect++;
            noteTimer.StopTimer();
            ShowFeedback("Correct!", Color.green);
            CalculatePointsAddition();

            waitingForInput = false;
            KeyControl.inputAllowed = false;
            noteTimer.PlayFillAnimation();
        }
        else
        {
            wrongAttempts++;
            StartCoroutine(ReplayTargetNote());
            keyControl.ShakeKey(note);
        }

        if (notesAttempted >= notesPerLevel)
        {
            Invoke(nameof(ShowEndOfLevelPanel), 2f);
        }
        else if (note == targetNote)
        {
            Invoke(nameof(PickRandomNote), 2f);
        }

        return note == targetNote;
    }


    private void ShowEndOfLevelPanel()
    {
        KeyControl.inputAllowed = false;
        waitingForInput = false;
        isSystemPlayingNote = false;

        noteTimer.StopTimer();
        StopAllCoroutines(); // Stops ReplayTargetNote if it's running
        CancelInvoke(nameof(PickRandomNote)); // Just in case a delayed note is scheduled

        if (CorrectNotesTXT != null)
        {
            CorrectNotesTXT.text = $"{notesCorrect}/{notesPerLevel}";
            EndLevelFeedbackTXT.text = $"{ GetEndOfLevelMessage() }";
        }

        endOfLevelPanel.SetActive(true);
    }



    public void RestartLevel()
    {
        endOfLevelPanel.SetActive(false);
        notesAttempted = 0;
        notesCorrect = 0;
        waitingForInput = false;
        isSystemPlayingNote = false;
        KeyControl.inputAllowed = false;

        StopAllCoroutines();
        CancelInvoke();

        ClearFeedback();
        Invoke(nameof(PickRandomNote), 1f);
    }


    private void HandleTimeOut()
    {
        Debug.Log("Time ran out! No points.");

        waitingForInput = false;
        isSystemPlayingNote = false;
        KeyControl.inputAllowed = false;

        ShowFeedback("Too slow!", Color.red);
        noteTimer.StopTimer();
        noteTimer.PlayFillAnimation();

        StopAllCoroutines();
        Invoke(nameof(PickRandomNote), 2f);
    }

    private IEnumerator ReplayTargetNote()
    {
        CancelInvoke(nameof(ClearFeedback));
        ClearFeedback();

        isSystemPlayingNote = true;
        waitingForInput = false;
        KeyControl.inputAllowed = false;

        yield return new WaitForSeconds(0.9f);

        keyControl.PressAsSystem(targetNote);
        yield return new WaitForSeconds(0.6f);

        isSystemPlayingNote = false;
        waitingForInput = true;
        KeyControl.inputAllowed = true;
    }

    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;

        feedbackText.text = message;
        feedbackText.color = color;

        CancelInvoke(nameof(ClearFeedback));
        Invoke(nameof(ClearFeedback), 2f);
    }

    private void ClearFeedback()
    {
        if (feedbackText != null)
            feedbackText.text = "";
    }

    /*private void CalculatePointsAddition()
    {
        float timeTaken = Time.time - noteStartTime;

        if (wrongAttempts == 0)
        {
            if (timeTaken <= 2f)
                CurrentUserManager.CurrentUser.points += 15;
            else
                CurrentUserManager.CurrentUser.points += 10;
        }
        else if (wrongAttempts == 1)
        {
            CurrentUserManager.CurrentUser.points += 5;
        }
        else
        {
            CurrentUserManager.CurrentUser.points += 2;
        }

        UserEditorService.SaveUserDitalesAfterGame();
    }*/

    private void CalculatePointsAddition()
    {
        float timeTaken = Time.time - noteStartTime;
        int level = CurrentUserManager.CurrentUser.Notelvl;
        int basePoints;

        if (wrongAttempts == 0)
        {
            basePoints = (timeTaken <= 2f) ? 15 : 10;
        }
        else if (wrongAttempts == 1)
        {
            basePoints = 5;
        }
        else
        {
            basePoints = 2;
        }

        // Bonus based on level
        float multiplier = 1f;
        if (level == 2) multiplier = 1.5f;
        else if (level == 3) multiplier = 2f;

        int finalPoints = Mathf.RoundToInt(basePoints * multiplier);
        CurrentUserManager.CurrentUser.points += finalPoints;

        UserEditorService.SaveUserDitalesAfterGame();
    }



    /////
    ///
    /*
    public void ToggleLevelPanal()
    {
        bool isActive = levelPNL.activeSelf;

        if (isActive)
        {
            levelPNL.SetActive(false);
            ResumeGame("LevelPNL");
        }
        else
        {
            levelPNL.SetActive(true);
            PauseGame("LevelPNL");
        }
    }

    public void SetLevel(int level)
    {
        UserEditorService.UpdateNoteRecognitionLevel(level);
        ToggleLevelPanal();

        // Optional feedback
        Debug.Log("Note level changed to: " + level);
        RestartLevel();


    }*/

    public void ToggleLevelPanal()
    {
        bool isActive = levelPNL.activeSelf;
        levelPNL.SetActive(!isActive);

        // Update the pause manager about level panel state
        NoteRecognitionPauseManager pauseManager = FindObjectOfType<NoteRecognitionPauseManager>();
        if (pauseManager != null)
        {
            pauseManager.SetLevelPanelPause(!isActive); // !isActive because we just toggled it
        }
    }

    public void SetLevel(int level)
    {
        // Stop all current game activity first
        waitingForInput = false;
        isSystemPlayingNote = false;
        KeyControl.inputAllowed = false;

        // Stop timer and any running coroutines
        noteTimer?.StopTimer();
        StopAllCoroutines();
        CancelInvoke();

        // Clear feedback
        ClearFeedback();

        // Update user level
        UserEditorService.UpdateNoteRecognitionLevel(level);

        // Close the level panel and update pause state
        levelPNL.SetActive(false);
        NoteRecognitionPauseManager pauseManager = FindObjectOfType<NoteRecognitionPauseManager>();
        if (pauseManager != null)
        {
            pauseManager.SetLevelPanelPause(false); // Level panel is now closed
        }

        // Reset level progress
        notesAttempted = 0;
        notesCorrect = 0;
        wrongAttempts = 0;

        // Fill the timer to show fresh start
        noteTimer?.PlayFillAnimation(0.5f);

        // Optional feedback
        Debug.Log("Note level changed to: " + level);

        // Start new level after a short delay to let the fill animation show
        Invoke(nameof(PickRandomNote), 1f);
    }

    private string GetEndOfLevelMessage()
    {
        float successRatio = (float)notesCorrect / notesPerLevel;

        List<string> lowSuccessMessages = new List<string>
    {
        "Nice try!", "Good practice!", "You're learning!", "Keep going!", "Don't give up!"
    };

        List<string> mediumSuccessMessages = new List<string>
    {
        "Well done!", "You're getting better!", "Keep it up!", "You're doing great!", "Almost there!"
    };

        List<string> highSuccessMessages = new List<string>
    {
        "Amazing!", "Fantastic job!", "Super star!", "You nailed it!", "Perfect!"
    };

        if (successRatio <= 1f / 3f)
            return lowSuccessMessages[Random.Range(0, lowSuccessMessages.Count)];
        else if (successRatio <= 2f / 3f)
            return mediumSuccessMessages[Random.Range(0, mediumSuccessMessages.Count)];
        else
            return highSuccessMessages[Random.Range(0, highSuccessMessages.Count)];
    }

}
