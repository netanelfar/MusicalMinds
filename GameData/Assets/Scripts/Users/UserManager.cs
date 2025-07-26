using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Static manager class that handles all user profile operations including loading, saving, and current user state management
public static class UserManager
{
    // Current user 
    public static UserProfile CurrentUser { get; private set; }

    // File management
    private static string savePath => Application.persistentDataPath + "/users.json";
    private const string LastUserKey = "LastConnectedUsername";

    // Events for UI updates
    public static System.Action<UserProfile> OnCurrentUserChanged;

    // ===== CURRENT USER MANAGEMENT =====

    // Sets the active user.
    public static void SetCurrentUser(UserProfile user)
    {
        CurrentUser = user;

        if (user != null)
        {
            PlayerPrefs.SetString(LastUserKey, user.username);
        }
        else
        {
            PlayerPrefs.DeleteKey(LastUserKey);
        }

        PlayerPrefs.Save();
        Debug.Log("Current user set to: " + (user?.username ?? "None"));

        // Triggers UserUI.UpdateUserDisplay.
        OnCurrentUserChanged?.Invoke(user);
    }

    // Loads the last connected user or falls back to most recent registered user.
    public static void EnsureCurrentUserLoadedFrom(List<UserProfile> users)
    {
        if (CurrentUser != null) return;

        string lastUsername = GetLastConnectedUsername();
        UserProfile lastUser = users.Find(u => u.username == lastUsername);

        if (lastUser != null)
        {
            SetCurrentUser(lastUser);
        }
        else if (users.Count > 0) // Fallback.
        {
            SetCurrentUser(users[users.Count - 1]); 
        }
        else
        {
            SetCurrentUser(null); //New user panel.
        }
    }

    // Retrieves the last connected user username.
    public static string GetLastConnectedUsername()
    {
        return PlayerPrefs.GetString(LastUserKey, null);
    }

    // ===== FILE I/O =====
    // Saves the entire user list to a JSON file.
    public static void SaveUsers(List<UserProfile> users)
    {
        UserDataWrapper wrapper = new UserDataWrapper { users = users };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Users saved to " + savePath);
    }

    // Loads all users from JSON file.
    public static List<UserProfile> LoadUsers()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No user data found. Creating new empty users file.");
            List<UserProfile> emptyList = new List<UserProfile>();
            SaveUsers(emptyList);
            return emptyList;
        }

        string json = File.ReadAllText(savePath);
        UserDataWrapper wrapper = JsonUtility.FromJson<UserDataWrapper>(json);
        return wrapper.users;
    }

    // ===== USER OPERATIONS =====

    // Checks if a username already exists.
    public static bool IsUsernameTaken(List<UserProfile> users, string username)
    {
        return users.Exists(u => u.username == username);
    }

    // Creates a new user profile with default settings.
    public static UserProfile CreateNewUser(string username, int profileIndex)
    {
        return new UserProfile
        {
            username = username,
            level = 1,
            freePlayCounter = 0,
            noteRecognitionCounter = 0,
            MelodyPlayCounter = 0,
            achievements = new List<string>(),
            points = 0,
            systemPressHasColor = true,
            preferredPianoSize = 4,
            preferredScreenSize = "1280x720",
            volume = 1f,
            ProfileINDX = profileIndex,
            NoteRecognitionDifficulty = 1,
            showNoteRecHints = 1,
            littleStarLevel = 0,
            littleJonathanLevel = 0,
            ninthSymphonyLevel = 0
        };
    }

    // Updates an existing user's username and profile.
    public static void EditUser(UserProfile user, string newUsername, int profileIndex)
    {
        user.username = newUsername;
        user.ProfileINDX = profileIndex;
    }

    // Deletes user from the file system.
    public static List<UserProfile> DeleteUser(UserProfile userToDelete)
    {
        if (userToDelete == null)
        {
            Debug.LogWarning("UserToDelete is null");
            return LoadUsers();
        }

        Debug.Log("UserManager.DeleteUser called for: " + userToDelete.username);
        List<UserProfile> allUsers = LoadUsers();


        // Find and remove by username instead of object reference
        UserProfile userInList = allUsers.Find(u => u.username == userToDelete.username);
        bool removed = false;
        if (userInList != null)
        {
            removed = allUsers.Remove(userInList); ///// unused?
        }

        SaveUsers(allUsers);

        if (CurrentUser != null && CurrentUser.username == userToDelete.username)
        {
            if (allUsers.Count > 0)
            {
                UserProfile fallbackUser = allUsers[allUsers.Count - 1];
                SetCurrentUser(fallbackUser);
            }
            else
            {
                SetCurrentUser(null);
            }
        }
        return allUsers;
    }

    // ===== USER SETTINGS UPDATES =====

    // Updates the current user's preferred piano size.
    public static void UpdatePianoSize(int newSize)
    {
        if (CurrentUser == null) return;

        CurrentUser.preferredPianoSize = newSize;
        SaveCurrentUserToFile();
    }

    // Updates the current user's note recognition difficulty level (1-3).
    public static void UpdateNoteRecognitionLevel(int newLevel)
    {
        if (CurrentUser == null || newLevel < 1 || newLevel > 3) return;

        CurrentUser.NoteRecognitionDifficulty = newLevel;
        SaveCurrentUserToFile();
    }

    // Updates the current user's note recognition hint display.
    public static void UpdateNoteRecHint(int value)
    {
        if (CurrentUser == null) return;

        CurrentUser.showNoteRecHints = value;
        SaveCurrentUserToFile();
    }

    // Adds points to the current user's total score.
    public static void UpdateUserPoints(int pointsToAdd)
    {
        if (CurrentUser == null) return;

        CurrentUser.points += pointsToAdd;
        SaveCurrentUserToFile();


    }

    // Saves current user's game progress data to file.
    public static void SaveUserDetailsAfterGame()
    {
        if (CurrentUser == null) return;

        List<UserProfile> allUsers = LoadUsers();
        foreach (var user in allUsers)
        {
            if (user.username == CurrentUser.username)
            {
                user.points = CurrentUser.points;
                user.level = CurrentUser.level;
                user.freePlayCounter = CurrentUser.freePlayCounter;
                user.noteRecognitionCounter = CurrentUser.noteRecognitionCounter;
                break;
            }
        }
        SaveUsers(allUsers);
    }

    // Updates the current user's level.
    public static void UpdateUserLevel(int newLevel)
    {
        if (CurrentUser == null) return;

        CurrentUser.level = newLevel;
        SaveCurrentUserToFile();
    }

    // Increments the play counter for the current game mode.
    public static void UpdatePlayModeCounter(int increment = 1)
    {
        if (CurrentUser == null) return;

        switch (GameSettings.CurrentGameMode)
        {
            case GameSettings.GameMode.FreePlay:
                CurrentUser.freePlayCounter += increment;
                break;

            case GameSettings.GameMode.SingleNoteRecognition:
                CurrentUser.noteRecognitionCounter += increment;
                break;

            case GameSettings.GameMode.MelodyPlay:
                CurrentUser.MelodyPlayCounter += increment;
                break;
        }

        SaveCurrentUserToFile();
    }

    // Adds achievement to current user.
    public static void AddAchievement(string achievement)
    {
        if (CurrentUser == null || CurrentUser.achievements.Contains(achievement)) return;

        CurrentUser.achievements.Add(achievement);
        SaveCurrentUserToFile();
    }

    // Updates the current user's volume preference.
    public static void UpdateVolume(float newVolume)
    {
        if (CurrentUser == null) return;

        CurrentUser.volume = newVolume;
        SaveCurrentUserToFile();
    }

    // Save current user changes to file.
    public static void SaveCurrentUserToFile()
    {
        if (CurrentUser == null) return;

        List<UserProfile> allUsers = LoadUsers();
        foreach (var user in allUsers)
        {
            if (user.username == CurrentUser.username)
            {
                // Update the file version with current user data.
                user.preferredPianoSize = CurrentUser.preferredPianoSize;
                user.NoteRecognitionDifficulty = CurrentUser.NoteRecognitionDifficulty;
                user.showNoteRecHints = CurrentUser.showNoteRecHints;
                user.points = CurrentUser.points;
                user.systemPressHasColor = CurrentUser.systemPressHasColor;
                user.level = CurrentUser.level;
                user.freePlayCounter = CurrentUser.freePlayCounter;
                user.noteRecognitionCounter = CurrentUser.noteRecognitionCounter;
                user.achievements = CurrentUser.achievements;
                user.volume = CurrentUser.volume;
                user.preferredScreenSize = CurrentUser.preferredScreenSize;
                user.littleStarLevel = CurrentUser.littleStarLevel;
                user.littleJonathanLevel = CurrentUser.littleJonathanLevel;
                user.ninthSymphonyLevel = CurrentUser.ninthSymphonyLevel;
                break;
            }
        }
        SaveUsers(allUsers);
    }


    // Update the user's progress level for a specific song.
    public static void UpdateSongLevel(string songName, int newLevel)
    {
        if (CurrentUser == null) return;

        bool shouldUpdate = false;

        switch (songName)
        {
            case "Little Star":
                if (newLevel > CurrentUser.littleStarLevel)
                {
                    CurrentUser.littleStarLevel = newLevel;
                    shouldUpdate = true;
                }
                break;

            case "Little Jonathan":
                if (newLevel > CurrentUser.littleJonathanLevel)
                {
                    CurrentUser.littleJonathanLevel = newLevel;
                    shouldUpdate = true;
                }
                break;

            case "Beethoven’s 9th":
                if (newLevel > CurrentUser.ninthSymphonyLevel)
                {
                    CurrentUser.ninthSymphonyLevel = newLevel;
                    shouldUpdate = true;
                }
                break;

            default:
                Debug.LogWarning($"Unknown song name: {songName}");
                return;
        }

        if (shouldUpdate)
        {
            SaveCurrentUserToFile();
            Debug.Log($"Updated {songName} level to {newLevel} for user {CurrentUser.username}");
        }
    }

    // Returns the current user's progress level for a song.
    public static int GetSongLevel(string songName)
    {
        if (CurrentUser == null) return 0;

        switch (songName)
        {
            case "Little Star":
                return CurrentUser.littleStarLevel;
            case "Little Jonathan":
                return CurrentUser.littleJonathanLevel;
            case "Beethoven’s 9th":
                return CurrentUser.ninthSymphonyLevel;
            default:
                Debug.LogWarning($"Unknown song name: {songName}");
                return 0;
        }
    }
}