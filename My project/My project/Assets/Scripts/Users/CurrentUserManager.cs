using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UserManager
{
    public static UserProfile CurrentUser { get; private set; }

    public static void SetCurrentUser(UserProfile user)
    {
        CurrentUser = user;
        Debug.Log("Current user set to: " + user.username);
    }
}
