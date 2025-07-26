using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages melody playback, player input validation, step progression, and challenge mode.
public class MelodyPlayManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject chooseSongPanel;
    public GameObject mapPanel;
    public GameObject stepCompletedPanel;

    [Header("Component References")]
    public KeyControl keyControl;
    public FeedbackManager feedbackManager;
    public TMPro.TextMeshProUGUI endOfLevelText;
    public PauseManager pause;
    public StepMapGenerator stepMapGenerator;
    public PointsManager pointsManager;

    [Header("Song Data")]
    public SongInfo currentSong;

    [Header("Gameplay Settings")]
    public float shortDelay = 0.5f;
    public float stepDelay = 1f;

    [Header("Challenge Mode UI")]
    public GameObject challengeButton;
    public GameObject challengeText;
    [Header("Playback Timing")]
    [SerializeField] private float challengeStepGap = 0.75f; // Pause between challenge steps

    // --- Pause reason keys ---
    private const string PAUSE_REASON_BUTTON = "PauseBTN";
    private const string PAUSE_REASON_PANEL = "LevelPNL";
    private const string PAUSE_REASON_SYSTEM = "System";

    // Pause State
    // Pause flags for input and playback
    private bool pausedByButton = false; // Pause triggered by pause button
    private bool pausedByPanel = false;  // Pause triggered by level complete panel

    // Game State Control
    private Coroutine playCoroutine = null;

    // Step Progress Tracking
    private int currentStepIndex = 0;
    private List<string> currentExpectedNotes;
    private int currentMatchIndex = 0;
    private bool hasMistake = false;

    // Challenge Mode State
    private bool isInChallengeMode = false;
    private bool isSystemPlayingChallenge = false;
    private int challengeMaxStep = 0;
    private List<string> combinedChallengeStep = new List<string>(); // stores combined steps

    // Points tracking.
    private int sessionPoints = 0;

    // Initializes game mode and activates song selection panel.
    void Start()
    {
        pause = FindObjectOfType<PauseManager>();

        if (pointsManager == null)
        {
            pointsManager = FindObjectOfType<PointsManager>();
        }
        if (chooseSongPanel != null)
        {
            chooseSongPanel.SetActive(true);
        }
    }

    // Set selected song and play step.
    public void SetCurrentSongFromMap(string songName, List<List<string>> songSteps, int startingStepIndex)
    {
        currentSong = new SongInfo(songName, songSteps);
        if (mapPanel != null)
        {
            mapPanel.SetActive(false);
        }
        currentStepIndex = startingStepIndex;
        Debug.Log($"Song selected from map: {songName}, Starting at step: {currentStepIndex + 1}");
        chooseSongPanel.SetActive(false);
        sessionPoints = 0;
        Invoke(nameof(PlayCurrentStep), stepDelay); // play selected step
    }

    // play current step.
    public void PlayCurrentStep()
    {
        StartCoroutine(PlayCurrentStepCoroutine());
    }

    // Starts note sequence playback
    private IEnumerator PlayCurrentStepCoroutine()
    {
        // Reset state
        currentMatchIndex = 0;
        hasMistake = false;

        if (currentSong == null || currentStepIndex >= currentSong.steps.Count)
        {
            Debug.LogWarning("No step to play or song not loaded.");
            yield break;
        }

        // Stop previous playback
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
            yield return new WaitForSeconds(0.4f); //  Delay between steps.
        }

        // Set notes and start playback
        currentExpectedNotes = isInChallengeMode ? combinedChallengeStep : currentSong.steps[currentStepIndex];
        playCoroutine = StartCoroutine(PlayNotesSequentially(currentExpectedNotes));
    }

    // Switch between player input mode and system playback mode
    private void SetInputState(bool inputAllowed)
    {
        KeyControl.inputAllowed = inputAllowed;
    }

    // Play current step's notes (skiped in challenge mode).
    private IEnumerator PlayNotesSequentially(List<string> notes)
    {
        if (isInChallengeMode && !isSystemPlayingChallenge)
        {
            SetInputState(true);
            playCoroutine = null;
            yield break;
        }

        SetInputState(false);

        foreach (var note in notes)
        {
            while (pausedByButton || pausedByPanel)
            {
                yield return new WaitForSeconds(0.1f);
            }

            keyControl.PressAsSystem(note);
            yield return new WaitForSeconds(shortDelay);
        }

        SetInputState(true);
        playCoroutine = null;
    }

    // Check if game can accept player input
    private bool CanAcceptInput()
    {
        return !(pausedByButton || pausedByPanel || !KeyControl.inputAllowed)
               && currentExpectedNotes != null
               && currentMatchIndex < currentExpectedNotes.Count;
    }

    // Validate player note against expected sequence.
    private bool CheckNote(string note)
    {
        string expected = currentExpectedNotes[currentMatchIndex];
        bool correct = (note == expected);
        Debug.Log($"{(correct ? "yes" : "no")} Player played: {note}, Expected: {expected}");

        if (!correct)
            hasMistake = true;

        currentMatchIndex++;
        return correct;
    }

    // Calculate and award points.
    private void AwardPoints(bool challenge, bool hasMistakes)
    {
        int points = challenge
            ? pointsManager?.CalculateChallengePoints(challengeMaxStep + 1, combinedChallengeStep.Count, !hasMistakes) ?? 0
            : pointsManager?.CalculateMelodyStepPoints(currentStepIndex, currentSong.steps.Count, hasMistakes) ?? 0;

        sessionPoints += points;

        if (!challenge)
        {
            if (!hasMistakes) // only update progress on success.
                UserManager.UpdateSongLevel(currentSong.name, currentStepIndex + 1);

            Debug.Log($"Step {currentStepIndex + 1} {(hasMistakes ? "failed" : "completed")}! Earned {points} points");
        }
        else
        {
            Debug.Log($"Challenge {(hasMistakes ? "failed" : "completed")}! Earned {points} points");
        }

        pointsManager?.UpdateAllUI();
        CheckAndAwardMelodyAchievements();

    }


    public bool OnPlayerPressedNote(string note)
    {
        if (!CanAcceptInput())
            return false;

        bool isCorrect = CheckNote(note);


        // Waite for more input
        if (currentMatchIndex < currentExpectedNotes.Count)
            return isCorrect;

        // End of step.
        KeyControl.inputAllowed = false;

        AwardPoints(isInChallengeMode, hasMistake);
        pointsManager?.UpdateAllUI();

        if (hasMistake)
        {
            Debug.Log("❌ Step failed — restarting...");
            StartCoroutine(isInChallengeMode ? HandleChallengeFailed() : RestartStep());
        }
        else
        {
            StartCoroutine(ShowStepCompletedPanel());
        }

        return isCorrect; // Used for feedback color.
    }

    // Display end of step panel.
    private IEnumerator ShowStepCompletedPanel()
    {
        Debug.Log("Step completed successfully!");
        yield return new WaitForSeconds(0.5f); //Short delay before showing the panel

        if (stepCompletedPanel != null)
        {
            stepCompletedPanel.SetActive(true);

            // Show challenge button starting from second step.
            if (challengeButton != null)
            {
                challengeButton.SetActive(currentStepIndex > 0);
            }
            if (challengeText != null)
            {
                challengeText.SetActive(currentStepIndex > 0);
            }

            if (feedbackManager != null && endOfLevelText != null && currentExpectedNotes != null)
            {
                Debug.Log($"Challenge completed! Earned {sessionPoints} points total");
                string message = feedbackManager.GetEndOfLevelMessage(1, 1);
                endOfLevelText.text = message;
            }
        }
        sessionPoints = 0;
        yield break;
    }

    // Manully restarts step
    /*
    public void OnRestartStepButtonPressed()
    {
        StartCoroutine(RestartStep());
    }
    */

    public void OnRestartButtonPressed()
    {
        // Just reset and use your existing PlayCurrentStep logic!
        ResetInputState();

        if (isInChallengeMode)
        {
            // Set the flag to replay the melody
            isSystemPlayingChallenge = true;
        }

        PlayCurrentStep(); // This already handles both modes
    }

    private void ResetInputState()
    {
        StopAllCoroutines();
        currentMatchIndex = 0;
        hasMistake = false;
        SetInputState(false);
        if (stepCompletedPanel != null)
            stepCompletedPanel.SetActive(false);
    }/// <summary>
    /// //
    /// </summary>
    /// <returns></returns>
    // Restarts current step 
    private IEnumerator RestartStep()
    {
        if (stepCompletedPanel != null)
        {
            stepCompletedPanel.SetActive(false);
        }
        yield return new WaitForSeconds(1f);
        PlayCurrentStep();
    }

    // Moves player to next step.
    public void OnNextStepButtonPressed()
    {
        if (currentSong == null || currentStepIndex >= currentSong.steps.Count - 1)
        {
            Debug.LogWarning("No next step available");
            return;
        }

        if (isInChallengeMode)
        {
            Debug.Log("Exiting challenge mode and moving to next step");
            isInChallengeMode = false;
            isSystemPlayingChallenge = false;
            combinedChallengeStep.Clear();
        }

        // Update user progress.
        int completedStepLevel = currentStepIndex + 1;
        UserManager.UpdateSongLevel(currentSong.name, completedStepLevel);

        // Move to next step.
        currentStepIndex++;
        stepCompletedPanel.SetActive(false);
        PlayCurrentStep();
    }

    // Creates a combined step from all steps up to current step.
    private List<string> CreateCombinedChallengeStep()
    {
        List<string> combinedStep = new List<string>();

        for (int i = 0; i <= challengeMaxStep; i++)
        {
            combinedStep.AddRange(currentSong.steps[i]);
        }

        return combinedStep;
    }

    // Button handler to start challenge mode.
    public void OnChallengeButtonPressed()
    {
        StartCoroutine(StartChallenge());
    }

    // Initialize challenge mode and play complete melody.
    private IEnumerator StartChallenge()
    {
        Debug.Log("Starting challenge mode!");

        isInChallengeMode = true;
        isSystemPlayingChallenge = true;
        challengeMaxStep = currentStepIndex;

        // Create combined step.
        combinedChallengeStep = CreateCombinedChallengeStep();
        Debug.Log($"Challenge created with {combinedChallengeStep.Count} total notes");

        if (stepCompletedPanel != null)
        {
            stepCompletedPanel.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PlayAllChallengeSteps());

        // Then start player input phase
        isSystemPlayingChallenge = false;
        PlayCurrentStep();
    }

    // Plays steps in sequence for challenge mode.
    private IEnumerator PlayAllChallengeSteps()
    {
        Debug.Log("Playing complete melody...");
        KeyControl.inputAllowed = false;

        for (int stepIndex = 0; stepIndex <= challengeMaxStep; stepIndex++)
        {
            List<string> stepNotes = currentSong.steps[stepIndex];

            // Add a pause between steps (except for the first one)
            if (stepIndex > 0)
            {
                yield return new WaitForSeconds(challengeStepGap); // smoother pacing between steps
            }

            // Play all notes in this step
            foreach (var note in stepNotes)
            {
                while (pausedByButton || pausedByPanel)
                {
                    yield return new WaitForSeconds(0.1f);
                }

                keyControl.PressAsSystem(note);
                yield return new WaitForSeconds(challengeStepGap); // smoother pacing between steps
            }
        }

        // Add a longer pause before player input
        yield return new WaitForSeconds(1f);
        Debug.Log("Now repeat the complete melody!");
    }

    // Handles challenge  mode fail.
    private IEnumerator HandleChallengeFailed()
    {
        Debug.Log("Challenge failed! Starting over...");
        yield return new WaitForSeconds(shortDelay);

        isSystemPlayingChallenge = true;
        yield return StartCoroutine(PlayAllChallengeSteps());

        isSystemPlayingChallenge = false;
        PlayCurrentStep();
    }

    // Pauses gameplay based on the given reason.
    public void PauseGame(string reason)
    {
        if (reason == PAUSE_REASON_BUTTON)
            pausedByButton = true;
        else if (reason == PAUSE_REASON_PANEL)
            pausedByPanel = true;
        else if (reason == PAUSE_REASON_SYSTEM)
            pausedByButton = true; // reuse same logic as button pause

        if (pausedByButton || pausedByPanel)
        {
            if (playCoroutine != null)
            {
                StopCoroutine(playCoroutine);
                playCoroutine = null;
            }

            KeyControl.inputAllowed = false;
            if (feedbackManager != null)
                feedbackManager.ShowFeedback("Game Paused", Color.yellow);
        }
    }

    // Resumes gameplay if all pause sources are cleared.
    public void ResumeGame(string reason)
    {
        if (reason == PAUSE_REASON_BUTTON)
            pausedByButton = false;
        else if (reason == PAUSE_REASON_PANEL)
            pausedByPanel = false;
        else if (reason == PAUSE_REASON_SYSTEM)
            pausedByButton = false;
        if (!pausedByButton && !pausedByPanel)
        {
            if (feedbackManager != null)
                feedbackManager.ClearFeedback();
        }
    }

    // Return to song progress map.
    public void ReOpenMap()
    {
        Debug.Log("Cancelling current step (and challenge if active) and opening step map...");

        // Stop all running coroutines
        StopAllCoroutines();

        playCoroutine = null;
        isSystemPlayingChallenge = false;
        isInChallengeMode = false;
        KeyControl.inputAllowed = false;
        currentMatchIndex = 0;
        hasMistake = false;

        if (stepCompletedPanel != null)
            stepCompletedPanel.SetActive(false);
        if (mapPanel != null)
            mapPanel.SetActive(true);
        if (stepMapGenerator != null && currentSong != null)
        {
            stepMapGenerator.GenerateMap(currentSong.name, currentSong.steps);
        }
        else
        {
            Debug.LogWarning("Missing StepMapGenerator or current song data.");
        }
    }

    // Check for new achievements.
    private void CheckAndAwardMelodyAchievements()
    {
        if (UserManager.CurrentUser == null) return;

        string songName = currentSong.name;
        int songLevel = UserManager.GetSongLevel(songName);

        // === FIRST STEP COMPLETED ===
        if (songLevel >= 1)
        {
            switch (songName)
            {
                case "Little Jonathan":
                    UserManager.AddAchievement(SimpleAchievementSystem.MelodyPlayIDs[0]); break;
                case "Little Star":
                    UserManager.AddAchievement(SimpleAchievementSystem.MelodyPlayIDs[1]); break;
                case "Beethoven’s 9th":
                    UserManager.AddAchievement(SimpleAchievementSystem.MelodyPlayIDs[2]); break;
            }
        }

        // === LAST STEP COMPLETED ===
        if (songLevel == currentSong.steps.Count)
        {
            switch (songName)
            {
                case "Little Jonathan":
                    UserManager.AddAchievement(SimpleAchievementSystem.MelodyPlayIDs[3]); break;
                case "Little Star":
                    UserManager.AddAchievement(SimpleAchievementSystem.MelodyPlayIDs[4]); break;
                case "Beethoven’s 9th":
                    UserManager.AddAchievement(SimpleAchievementSystem.MelodyPlayIDs[5]); break;
            }
        }

        // === FINAL CHALLENGE COMPLETED ===
        if (isInChallengeMode && challengeMaxStep == currentSong.steps.Count - 1 && !hasMistake)
        {
            switch (songName)
            {
                case "Little Jonathan":
                    UserManager.AddAchievement(SimpleAchievementSystem.MelodyPlayIDs[6]); break;
                case "Little Star":
                    UserManager.AddAchievement(SimpleAchievementSystem.MelodyPlayIDs[7]); break;
                case "Beethoven’s 9th":
                    UserManager.AddAchievement(SimpleAchievementSystem.MelodyPlayIDs[8]); break;
            }
        }
    }
}


