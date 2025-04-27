using UnityEngine;

// Manages the currently connected user 

public static class UserManager
{
    public static UserProfile CurrentUser { get; private set; }

    private const string LastUserKey = "LastConnectedUsername";

    public static void SetCurrentUser(UserProfile user)
    {
        CurrentUser = user;

        if (user != null)
        {
            PlayerPrefs.SetString(LastUserKey, user.username);
        }
        else
        {
            PlayerPrefs.DeleteKey(LastUserKey); // Clear last user if no user
        }

        PlayerPrefs.Save();
        Debug.Log("Current user set to: " + (user != null ? user.username : "None"));
    }


    // Returns the last connected user
    public static string GetLastConnectedUsername()
    {
        return PlayerPrefs.GetString(LastUserKey, null); 
    }
}
