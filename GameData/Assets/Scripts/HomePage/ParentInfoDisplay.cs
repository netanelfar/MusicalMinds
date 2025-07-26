using TMPro;
using UnityEngine;

/// Displays summary data for parents about the child's gameplay
public class ParentInfoDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text usernameText;
    public TMP_Text totalPointsText;
    public TMP_Text levelText;
    public TMP_Text freePlayText;
    public TMP_Text noteRecText;
    public TMP_Text melodyText;
    public TMP_Text achievementsText;
    public TMP_Text littleStarProgressText;
    public TMP_Text littleJonathanProgressText;
    public TMP_Text ninthSymphonyText;
    public TMP_Text mostPlayedModeText;


    void OnEnable()
    {
        UpdateParentInfo();
    }

    // Update connected user data
    public void UpdateParentInfo()
    {
        var user = UserManager.CurrentUser;
        if (user == null)
        {
            Debug.Log("User is null, attempting to load users first");
            // Try to ensure user is loaded
            UserManager.EnsureCurrentUserLoadedFrom(UserManager.LoadUsers());
            user = UserManager.CurrentUser;

            if (user == null)
            {
                Debug.LogWarning("Still no current user available");
                return;
            }
        }

        usernameText.text = $"User: {user.username}"; 
        totalPointsText.text = $"Points: {user.points}";
        levelText.text = $"Level: {user.level}";
        freePlayText.text = $"{user.freePlayCounter}";
        noteRecText.text = $"{user.noteRecognitionCounter}";
        melodyText.text = $"{user.MelodyPlayCounter}";
        achievementsText.text = $"Achievements: {user.achievements.Count}";
        littleStarProgressText.text = $"{user.littleStarLevel}";
        littleJonathanProgressText.text = $"{user.littleJonathanLevel}";
        ninthSymphonyText.text= $"{user.ninthSymphonyLevel}";


        // Most played mode
        string mostPlayed = GetMostPlayedMode(user);
        mostPlayedModeText.text = $"Most Played Mode: {mostPlayed}";
    }

    private string GetMostPlayedMode(UserProfile user)
    {
        int free = user.freePlayCounter;
        int note = user.noteRecognitionCounter;
        int melody = user.MelodyPlayCounter;

        if (free >= note && free >= melody) return "Free Play";
        else if (note >= melody) return "Note Recognition";
        else return "Melody Play";
    }
}
