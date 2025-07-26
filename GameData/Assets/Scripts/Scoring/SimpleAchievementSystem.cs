using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Data structure for achievement title and description.
[System.Serializable]
public class AchievementData
{
    public string title;
    public string description;
}

public enum AchievementCategory
{
    NoteRecognition,
    MelodyPlay,
    FreePlay
}

// Manages achievement data, icons, and unlock status across three game categorie.

public class SimpleAchievementSystem : MonoBehaviour
{
    [Header("Achievement Icons")]
    public List<Sprite> SoundMatchIcons = new List<Sprite>();
    public List<Sprite> melodyPlayIcons = new List<Sprite>();
    public List<Sprite> FreePlayIcons = new List<Sprite>();

    [Header("Achievement Data")]
    public List<AchievementData> SoundMatchAchievements = new List<AchievementData>(); 
    public List<AchievementData> melodyPlayAchievements = new List<AchievementData>();
    public List<AchievementData> FreePlayAchievements = new List<AchievementData>();


    // Predefined achievement IDs for note recognition mode
    public static readonly string[] SoundMatchIDs = {
    "SM_1",            // 0 - Played Sound Match once
    "SM_5",            // 1 - Played Sound Match 5 times
    "SM_10",           // 2 - Played Sound Match 10 times
    "SM_25",           // 3 - Played Sound Match 25 times
    "SM_score",        // 4 - Achieved a perfect score in Sound Match (easy)
    "SM_score_normal", // 5 - Perfect score on normal difficulty
    "SM_score_hard"    // 6 - Perfect score on hard difficulty
};

    public static readonly string[] MelodyPlayIDs = {
        "melody_started_jonathan",   // 0
        "melody_started_star",       // 1
        "melody_started_symphony",   // 2
        "melody_finished_jonathan",  // 3
        "melody_finished_star",      // 4
        "melody_finished_symphony",  // 5
        "melody_challenge_jonathan", // 6
        "melody_challenge_star",     // 7
        "melody_challenge_symphony"  // 8
    };

    public static readonly string[] FreePlayIDs = {
        "played_first",   // 0
        "played_fifth",   // 1
        "played_ten",     // 2
        "played_20",      // 3
        "played_50",      // 4
        "played_100"      //5
    };

    // Get achievement data by category and index.
    public AchievementData GetAchievementData(AchievementCategory category, int index)
    {
        switch (category)
        {
            case AchievementCategory.NoteRecognition:
                return index >= 0 && index < SoundMatchAchievements.Count ? SoundMatchAchievements[index] : null;
            case AchievementCategory.MelodyPlay:
                return index >= 0 && index < melodyPlayAchievements.Count ? melodyPlayAchievements[index] : null;
            case AchievementCategory.FreePlay:
                return index >= 0 && index < FreePlayAchievements.Count ? FreePlayAchievements[index] : null;
            default:
                return null;
        }
    }

    // Get achievement icon by category and index.
    public Sprite GetAchievementIcon(AchievementCategory category, int index)
    {
        switch (category)
        {
            case AchievementCategory.NoteRecognition:
                return index >= 0 && index < SoundMatchIcons.Count ? SoundMatchIcons[index] : null;
            case AchievementCategory.MelodyPlay:
                return index >= 0 && index < melodyPlayIcons.Count ? melodyPlayIcons[index] : null;
            case AchievementCategory.FreePlay:
                return index >= 0 && index < FreePlayIcons.Count ? FreePlayIcons[index] : null;
            default:
                return null;
        }
    }

    // Get achievement ID by category and index.
    public string GetAchievementID(AchievementCategory category, int index)
    {
        switch (category)
        {
            case AchievementCategory.NoteRecognition:
                return index >= 0 && index < SoundMatchIDs.Length ? SoundMatchIDs[index] : "";
            case AchievementCategory.MelodyPlay:
                return index >= 0 && index < MelodyPlayIDs.Length ? MelodyPlayIDs[index] : "";
            case AchievementCategory.FreePlay:
                return index >= 0 && index < FreePlayIDs.Length ? FreePlayIDs[index] : "";
            default:
                return "";
        }
    }

    // Check if achievement is unlocked.
    public bool IsUnlocked(AchievementCategory category, int index)
    {
        string id = GetAchievementID(category, index);
        return UserManager.CurrentUser?.achievements?.Contains(id) ?? false;
    }

    // Get total achievements in a category.
    public int GetTotalAchievements(AchievementCategory category)
    {
        switch (category)
        {
            case AchievementCategory.NoteRecognition:
                return SoundMatchIDs.Length;
            case AchievementCategory.MelodyPlay:
                return MelodyPlayIDs.Length;
            case AchievementCategory.FreePlay:
                return FreePlayIDs.Length;
            default:
                return 0;
        }
    }

}
