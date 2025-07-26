using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// Manages player points, level progression, and calculates score based on game performance.
public class PointsManager : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI PointsNumber;
    public TextMeshProUGUI LevelText;

    // Initialize UI with connected user's points and level.
    void Start()
    {
        UpdatePointsText();
        UpdateLevelText();
    }

    // Updates points display text from current user data.
    void UpdatePointsText()
    {
        if (PointsNumber != null && UserManager.CurrentUser != null)
        {
            PointsNumber.text = UserManager.CurrentUser.points.ToString();
        }
    }

    // ------ BONUS METHODS ------ //

    // Calculates piano size bonus multiplier (only for note recognition)
    private float GetPianoSizeBonus()
    {
        int pianoSize = UserManager.CurrentUser?.preferredPianoSize ?? 3;
        if (pianoSize == 4) return 1.1f;  // 10% bonus
        if (pianoSize == 5) return 1.25f; // 25% bonus
        return 1f; // No bonus
    }

    // Calculates hint bonus multiplier (only for note recognition)
    private float GetHintBonus()
    {
        if (UserManager.CurrentUser?.showNoteRecHints == 0)
            return 1.3f; // 30% bonus for no hints
        return 1f; // No bonus
    }

    // Calculates difficulty bonus multiplier (note recognition)
    private float GetDifficultyBonus(int difficulty)
    {
        if (difficulty == 2) return 1.5f; // 50% bonus
        if (difficulty == 3) return 2f;   // 100% bonus
        return 1f; // No bonus
    }

    // Calculates bonus for disabled system color. 
    private float GetNoColorBonus()
    {
        if (UserManager.CurrentUser?.systemPressHasColor == false) // No color assistance
            return 1.5f; // 50% bonus for playing without color help
        return 1f; // No bonus
    }

    // ------ POINT CALCULATIONS ------ //

    // Calculates points for note recognition based on time, mistakes, difficulty, and settings
    public int CalculateNoteRecognitionPoints(float timeTaken, int wrongAttempts, int difficulty)
    {
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

        float multiplier = GetDifficultyBonus(difficulty) * GetHintBonus() * GetPianoSizeBonus() * GetNoColorBonus(); 
        int finalPoints = Mathf.RoundToInt(basePoints * multiplier);

        UserManager.UpdateUserPoints(finalPoints);
        UpdatePointsText();
        CheckLevelUp();
        return finalPoints;
    }

    // Calculate points for completing melody play steps.
    public int CalculateMelodyStepPoints(int stepNumber, int totalSteps, bool hasMistakes)
    {
        int basePoints = hasMistakes ? 3 : 8; // 3 for mistakes, 8 for perfect
        float multiplier = GetNoColorBonus();
        int finalPoints = Mathf.RoundToInt(basePoints * multiplier);

        UserManager.UpdateUserPoints(finalPoints);
        UpdatePointsText();
        CheckLevelUp();
        Debug.Log($"Melody step completed: Perfect={!hasMistakes}, Points: {finalPoints}");
        return finalPoints;
    }

    // Calculate points for challenge steps (much higher rewards for combined steps).
    public int CalculateChallengePoints(int stepsCompleted, int totalNotesInChallenge, bool perfectChallenge)
    {
        int basePoints = perfectChallenge ? 20 : 10;

        // Challenge complexity multiplier - receive more points for longer challenges
        float complexityMultiplier = 1f + (stepsCompleted * 0.4f); // Each step adds 40% more points

        // No mistakes in entire sequence bonus.
        if (perfectChallenge)
        {
            complexityMultiplier *= 1.8f; // 80% bonus
        }

        int finalPoints = Mathf.RoundToInt(basePoints * complexityMultiplier);

        UserManager.UpdateUserPoints(finalPoints);
        UpdatePointsText();
        CheckLevelUp();
        Debug.Log($"Challenge completed: {stepsCompleted} steps, {totalNotesInChallenge} notes, Perfect: {perfectChallenge}, Points: {finalPoints}");
        return finalPoints;
    }


    // Checks if user leveled up.
    public void CheckLevelUp()
    {
        if (UserManager.CurrentUser == null) return;

        int currentLevel = UserManager.CurrentUser.level;
        int currentPoints = UserManager.CurrentUser.points;

        Debug.Log($"CheckLevelUp: Current Level: {currentLevel}, Current Points: {currentPoints}");

        // Check for level up.
        bool leveledUp = false;
        int newLevel = currentLevel;

        while (true)
        {
            int nextLevel = newLevel + 1;
            int nextLevelThreshold = GetLevelThreshold(nextLevel);

            Debug.Log($"Checking if {currentPoints} >= {nextLevelThreshold} (threshold for level {nextLevel})");

            if (currentPoints >= nextLevelThreshold)
            {
                newLevel = nextLevel;
                leveledUp = true;
                Debug.Log($"Level up! Moving to level {newLevel}");
            }
            else
            {
                break; 
            }
        }

        if (leveledUp)
        {
            UserManager.UpdateUserLevel(newLevel);
            Debug.Log($"Final level up! You are now level {newLevel}");
            UpdateLevelText();
        }
    }




    // Returns points required for a specific level using progressive scaling formula.
    private int GetLevelThreshold(int level)
    {
        // Progressive scaling: each level requires significantly more points
        // Level 2: 500 points  
        // Level 3: 900 points
        // Level 4: 1400 points
        // Level 5: 2000 points, etc.    
        int threshold = 200 * level + 150 * (level - 1) * level / 2;   
        Debug.Log($"Level {level} threshold: {threshold}");
        return threshold;      
    }

    // Updates level UI.
    void UpdateLevelText()
    {
        if (LevelText != null && UserManager.CurrentUser != null)
        {
            LevelText.text = UserManager.CurrentUser.level.ToString();
        }
    }

    // Get points needed for next level.
    public int GetPointsToNextLevel()
    {
        if (UserManager.CurrentUser == null) return 0;

        int currentLevel = UserManager.CurrentUser.level;
        int currentPoints = UserManager.CurrentUser.points;
        int nextLevelThreshold = GetLevelThreshold(currentLevel + 1);
        return Mathf.Max(0, nextLevelThreshold - currentPoints);
    }

    // Refreshes both points and level UI displays
    public void UpdateAllUI()
    {
        UpdatePointsText();
        UpdateLevelText();
    }

}
