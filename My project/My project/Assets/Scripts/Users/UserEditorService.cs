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
            ProfileINDX = profileIndex
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
        if (UserManager.CurrentUser == null) return;

        // Update in-memory
        UserManager.CurrentUser.preferredPianoSize = newSize;

        // Update in file
        List<UserProfile> allUsers = UserDataManager.LoadUsers();
        foreach (var user in allUsers)
        {
            if (user.username == UserManager.CurrentUser.username)
            {
                user.preferredPianoSize = newSize;
                break;
            }
        }

        UserDataManager.SaveUsers(allUsers);
    }


    public static void SaveUserDitalesAfterGame()
    {
        if (UserManager.CurrentUser == null) return;

        List<UserProfile> allUsers = UserDataManager.LoadUsers();

        foreach (var user in allUsers)
        {
            if (user.username == UserManager.CurrentUser.username)
            {
                user.points = UserManager.CurrentUser.points;
                break;
            }
        }

        UserDataManager.SaveUsers(allUsers);
    }

}
