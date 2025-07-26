using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Manages Free Play mode by:
/// - Awarding points at timed intervals
/// - Adjusting the piano layout based on user preferences
/// - Unlocking achievements based on session count
public class FreePlayGameManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float rewardInterval = 30f; //120f  2 minutes
    [SerializeField] private int pointsPerReward = 15;//5

    [Header("References")]
    public PointsManager pointsManager;

    [Header("Piano Octaves")]
    public GameObject Octave3;
    public GameObject Octave4;
    public GameObject Octave5;

    // Private fields
    private float playTimer = 0f;
    private bool isActive = true;
    private int rewardCount = 0;

    /// Called when the scene starts. Sets game mode, configures piano, enables input.
    void Start()
    {
        GameSettings.CurrentGameMode = GameSettings.GameMode.FreePlay;

        SetupPianoSize();
        CheckAndAwardFreePlayAchievements();
        KeyControl.inputAllowed = true;

        Debug.Log("[FreePlay] Started - Rewarding " + pointsPerReward + " points every " + rewardInterval + " seconds");
    }

    /// Adjusts visible octaves on the virtual piano based on the user's preferred size.
    private void SetupPianoSize()
    {
        if (UserManager.CurrentUser != null)
        {
            int preferredSize = UserManager.CurrentUser.preferredPianoSize;

            Octave3.SetActive(true);
            Octave4.SetActive(preferredSize >= 4);
            Octave5.SetActive(preferredSize >= 5);

            Debug.Log($"[FreePlay] Piano size set to {preferredSize} octaves");
        }
        else
        {
            // Default: show all octaves if no user
            Octave3.SetActive(true);
            Octave4.SetActive(true);
            Octave5.SetActive(true);
        }
    }

    /// Called every frame. Handles the reward timer.
    void Update()
    {
        // Safety checks
        if (!isActive || UserManager.CurrentUser == null) return;

        // Increment timer
        playTimer += Time.deltaTime;

        // Check if reward interval reached
        if (playTimer >= rewardInterval)
        {
            AwardTimerPoints();
            playTimer -= rewardInterval; // Reset timer
        }
    }

    /// Gives the user points and updates the points UI and level system.
    private void AwardTimerPoints()
    {
        rewardCount++;

        // Award points
        UserManager.UpdateUserPoints(pointsPerReward);

        // Update UI
        if (pointsManager != null)
            pointsManager.UpdateAllUI();
        if (pointsManager != null)
        {
            pointsManager.CheckLevelUp();
            pointsManager.UpdateAllUI();
        }

        Debug.Log($"[FreePlay] Awarded {pointsPerReward} points (Reward #{rewardCount})");
    }


    /// Checks if the current user qualifies for Free Play achievements and awards them.
    private void CheckAndAwardFreePlayAchievements()
    {
        if (UserManager.CurrentUser == null) return;

        int currentPlayCount = UserManager.CurrentUser.freePlayCounter;
        int[] milestones = { 1, 5, 10, 20, 50, 100 };

        for (int i = 0; i < milestones.Length; i++)
        {
            if (currentPlayCount == milestones[i])
            {
                string achievementID = SimpleAchievementSystem.FreePlayIDs[i];
                UserManager.AddAchievement(achievementID);
                Debug.Log($"[FreePlay] Achievement unlocked: {achievementID}");
            }
        }
    }

    /// Allows external components to pause or resume the reward timer.
    public void SetActive(bool active)
    {
        isActive = active;
        Debug.Log("[FreePlay] Timer " + (active ? "resumed" : "paused"));
    }
    /// Called when the object is destroyed (e.g., scene change). Logs the session result.
    void OnDestroy()
    {
        Debug.Log($"[FreePlay] Session ended - {rewardCount} rewards given");
    }
}