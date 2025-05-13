using System.Collections.Generic;

public static class UserEditorService
{
    // ---- Username Validation ----
    public static bool IsUsernameTaken(List<UserProfile> users, string username)
    {
        return users.Exists(u => u.username == username);
    }

    // ---- Create User ----
    public static UserProfile CreateNewUser(string username, int profileIndex)
    {
        return new UserProfile
        {
            username = username,
            level = 1,
            freePlayCounter = 0,
            noteRecognitionCounter = 0,
            achievements = new List<string>(),
            points = 0,
            preferredPianoSize = 25,
            preferredScreenSize = "1280x720",
            volume = 1f,
            ProfileINDX = profileIndex,
            Notelvl = 1,
        
        
         };
    }

    // ---- Edit Profile ----
    public static void EditUser(UserProfile user, string newUsername, int profileIndex)
    {
        user.username = newUsername;
        user.ProfileINDX = profileIndex;
    }

    // ---- Update Piano Size ----
    public static void UpdatePianoSize(int newSize)
    {
        if (CurrentUserManager.CurrentUser == null) return;

        // Update in-memory
        CurrentUserManager.CurrentUser.preferredPianoSize = newSize;

        // Update in file
        List<UserProfile> allUsers = UserDataManager.LoadUsers();
        foreach (var user in allUsers)
        {
            if (user.username == CurrentUserManager.CurrentUser.username)
            {
                user.preferredPianoSize = newSize;
                break;
            }
        }

        UserDataManager.SaveUsers(allUsers);
    }


    public static void SaveUserDitalesAfterGame()
    {
        if (CurrentUserManager.CurrentUser == null) return;

        List<UserProfile> allUsers = UserDataManager.LoadUsers();

        foreach (var user in allUsers)
        {
            if (user.username == CurrentUserManager.CurrentUser.username)
            {
                user.points = CurrentUserManager.CurrentUser.points;
                break;
            }
        }

        UserDataManager.SaveUsers(allUsers);
    }


    public static void UpdateNoteRecognitionLevel(int newLevel)
    {
        if (CurrentUserManager.CurrentUser == null) return;

        if (newLevel < 1 || newLevel > 3) return; // Only allow 1, 2, or 3 for now

        // Update in-memory
        CurrentUserManager.CurrentUser.Notelvl = newLevel;

        // Update in file
        List<UserProfile> allUsers = UserDataManager.LoadUsers();
        foreach (var user in allUsers)
        {
            if (user.username == CurrentUserManager.CurrentUser.username)
            {
                user.Notelvl = newLevel;
                break;
            }
        }

        UserDataManager.SaveUsers(allUsers);
    }




}
