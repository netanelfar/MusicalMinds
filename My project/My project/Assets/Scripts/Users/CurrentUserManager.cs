using UnityEngine;

public static class UserManager
{
    public static UserProfile CurrentUser { get; private set; }

    private const string LastUserKey = "LastConnectedUsername";

    public static void SetCurrentUser(UserProfile user)
    {
        CurrentUser = user;
        PlayerPrefs.SetString(LastUserKey, user.username);
        PlayerPrefs.Save(); 
        Debug.Log("Current user set to: " + user.username);
    }

    public static string GetLastConnectedUsername()
    {
        return PlayerPrefs.GetString(LastUserKey, null); // Will return null if not found
    }
}
