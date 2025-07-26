using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;



// Manages single note recognition gameplay.
public class SingleNoteRecognitionManager : MonoBehaviour
{
    [Header("Core Components")]
    public KeyControl keyControl;
    public TextMeshProUGUI feedbackText;
    public TimerSliderDisplay noteTimer;
    public PauseManager pause;
    public PointsManager points;
    public FeedbackManager feedback;


    [Header("Panels")]
    public GameObject levelPNL;
    public GameObject endOfLevelPanel;

    [Header("Text")]
    public TextMeshProUGUI CorrectNotesTXT;
    public TextMeshProUGUI EndLevelFeedbackTXT;
    public TextMeshProUGUI LevelPointsTXT;

    [Header("Hint Arrow")]
    public GameObject hintArrowPrefab;
    private GameObject currentHintArrow;
    public RectTransform hintArrowParent;

    [Header("Piano Octaves")]
    public GameObject Octave3;
    public GameObject Octave4;
    public GameObject Octave5;


    // Game State Variables.
    private string targetNote;
    private int wrongAttempts = 0;
    private float noteStartTime = 0f;

    // Pause Management.
    private bool PausedByBTN = false;
    private bool PausedByPNl = false;

    // Level Progress Tracking.
    private int notesAttempted = 0;
    private int notesCorrect = 0;
    private int notesPerLevel = 8;
    private int LevePoints = 0;

    private List<string> allowedNotes = new List<string>();

    // Initialize game components and configuration based on user preferences.
    void Start()
    {
        KeyControl.inputAllowed = false;
        noteTimer.OnTimerFinished = HandleTimeOut;
        allowedNotes.AddRange(new List<string> { "48", "50", "52", "53", "55", "57", "59", "49", "51", "54", "56", "58" }); // Octave 3

        if (UserManager.CurrentUser != null)
        {
            Debug.Log($"Entered Sound Match: {UserManager.CurrentUser.username}");
            int preferredSize = UserManager.CurrentUser.preferredPianoSize;
            Octave3.SetActive(true);
            Octave4.SetActive(preferredSize >= 4);
            Octave5.SetActive(preferredSize == 5);

            if (preferredSize >= 4)
            {
                allowedNotes.AddRange(new List<string> { "60", "62", "64", "65", "67", "69", "71", "61", "63", "66", "68", "70" }); // Octave 4
            }
            if (preferredSize >= 5)
            {
                allowedNotes.AddRange(new List<string> { "72", "74", "76", "77", "79", "81", "83", "73", "75", "78", "80", "82" }); // Octave 5
            }
        }
        else// fake user
        {
            allowedNotes.AddRange(new List<string> { "60", "62", "64", "65", "67", "69", "71", "61", "63", "66", "68", "70", "72", "74", "76", "77", "79", "81", "83", "73", "75", "78", "80", "82" });
        }

        Invoke(nameof(PickRandomNote), 1f);
    }

    // Pauses game based on button or panel.
    public void PauseGame(string reason)
    {
        if (reason == "PuseBTN")
            PausedByBTN = true;
        else if (reason == "LevelPNL")
            PausedByPNl = true;

        if (PausedByBTN || PausedByPNl)
        {
            noteTimer?.PauseTimer();
            feedback.ShowFeedback("Game Paused", Color.yellow);
        }
    }

    // Resumes game only when both pause sources are cleared.
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
            feedback.ClearFeedback();
        }
    }

    // Selects and play random note from allowed range.
    public void PickRandomNote()
    {
        feedback.CancelPendingClear();
        feedback.ClearFeedback();
        KeyControl.inputAllowed = false; // Prevent user input while system plays note.

        if (allowedNotes.Count == 0)
        {
            Debug.LogError("allowedNotes is empty! Cannot pick a note.");
            return;
        }

        targetNote = allowedNotes[Random.Range(0, allowedNotes.Count)];
        wrongAttempts = 0;
        noteStartTime = Time.time;
        keyControl.PressAsSystem(targetNote);
        Invoke(nameof(StartPlayerInputAndTimer), 0.8f);
    }

    // Enables player input and starts difficulty-based timer.
    private void StartPlayerInputAndTimer()
    {
        KeyControl.inputAllowed = true;

        int level = UserManager.CurrentUser.NoteRecognitionDifficulty;
        switch (level)
        {
            case 1: noteTimer.maxTime = 6f; break;  // Easy
            case 2: noteTimer.maxTime = 4f; break;  // Normal
            case 3: noteTimer.maxTime = 2.5f; break;  // Hard
            default: noteTimer.maxTime = 4f; break;
        }

        noteTimer.StartTimer();
    }

    // Handles player note input, scoring, and level progression
    public bool OnPlayerPressedNote(string note)
    {
        if (!KeyControl.inputAllowed)
            return false;

        notesAttempted++;

        if (note == targetNote) // Correct.
        {
            notesCorrect++;
            noteTimer.StopTimer();
            feedback.ShowFeedback("Correct!", Color.green);
            float timeTaken = Time.time - noteStartTime;
            int level = UserManager.CurrentUser.NoteRecognitionDifficulty;
            LevePoints += points.CalculateNoteRecognitionPoints(timeTaken, wrongAttempts, level);
            KeyControl.inputAllowed = false;
            noteTimer.PlayFillAnimation();
        }
        else //Wrong.
        {
            wrongAttempts++;
            StartCoroutine(ReplayTargetNote());
            ShowHintArrow(note);
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

    // Creates and displays a hint arrow.
    private void ShowHintArrow(string playerNote)
    {
        if (UserManager.CurrentUser?.showNoteRecHints != 1) return;
        if (currentHintArrow != null) Destroy(currentHintArrow);

        int noteDistance = int.Parse(targetNote) - int.Parse(playerNote);
        currentHintArrow = Instantiate(hintArrowPrefab, hintArrowParent);
        currentHintArrow.GetComponent<HintArrow>().ShowHint(noteDistance);
    }

    // Shows end-of-level results and checks for achievements.
    private void ShowEndOfLevelPanel()
    {
        KeyControl.inputAllowed = false;
        noteTimer.StopTimer();
        StopAllCoroutines();
        CancelInvoke(nameof(PickRandomNote));

        // Check for perfect score achievement based on difficulty.
        if (notesCorrect == notesPerLevel)
        {
            int difficulty = UserManager.CurrentUser.NoteRecognitionDifficulty;
            if (difficulty == 1 && !UserManager.CurrentUser.achievements.Contains("SM_score"))
            {
                UserManager.AddAchievement("SM_score");
            }
            else if (difficulty == 2 && !UserManager.CurrentUser.achievements.Contains("SM_score_normal"))
            {
                UserManager.AddAchievement("SM_score_normal");
            }
            else if (difficulty == 3 && !UserManager.CurrentUser.achievements.Contains("SM_score_hard"))
            {
                UserManager.AddAchievement("SM_score_hard");
            }
        }

        // Check note recognition play count achievements.
        int noteRecCount = UserManager.CurrentUser.noteRecognitionCounter;

        if (noteRecCount == 1 && !UserManager.CurrentUser.achievements.Contains("SM_1"))
        {
            UserManager.AddAchievement("SM_1");
        }
        else if (noteRecCount == 5 && !UserManager.CurrentUser.achievements.Contains("SM_5"))
        {
            UserManager.AddAchievement("SM_5");
        }
        else if (noteRecCount == 10 && !UserManager.CurrentUser.achievements.Contains("SM_10"))
        {
            UserManager.AddAchievement("SM_10");
        }
        else if (noteRecCount == 25 && !UserManager.CurrentUser.achievements.Contains("SM_25"))
        {
            UserManager.AddAchievement("SM_25");
        }

        // Update UI 
        if (CorrectNotesTXT != null)
        {
            CorrectNotesTXT.text = $"{notesCorrect}/{notesPerLevel}";
        }
        if (EndLevelFeedbackTXT != null)
        {
            EndLevelFeedbackTXT.text = $"{feedback.GetEndOfLevelMessage(notesCorrect, notesPerLevel)}";
        }
        if (LevelPointsTXT != null)
        {
            LevelPointsTXT.text = $"{LevePoints}";
        }

        endOfLevelPanel.SetActive(true);
    }


    // Resets level state and starts new round.
    public void RestartLevel()
    {
        endOfLevelPanel.SetActive(false);
        notesAttempted = 0;
        notesCorrect = 0;
        LevePoints = 0;
        KeyControl.inputAllowed = false;
        StopAllCoroutines();
        CancelInvoke();
        feedback.ClearFeedback();
        Invoke(nameof(PickRandomNote), 1f);
    }

    // Handles timer expiration.
    private void HandleTimeOut()
    {
        Debug.Log("Time ran out! No points.");
        KeyControl.inputAllowed = false;
        feedback.ShowFeedback("Too slow!", Color.red);
        noteTimer.StopTimer();
        noteTimer.PlayFillAnimation();
        StopAllCoroutines();
        Invoke(nameof(PickRandomNote), 2f);
    }

    // Replays target note after wrong answer.
    private IEnumerator ReplayTargetNote()
    {
        feedback.CancelPendingClear();
        yield return new WaitForSeconds(0.9f); // Dealys before replaying note..
        keyControl.PressAsSystem(targetNote);
        yield return new WaitForSeconds(0.6f);// Delays after replaying note.
        KeyControl.inputAllowed = true;
    }

    // Returns message based on success ratio.
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
