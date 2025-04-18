using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class UserDataManager
{
    private static string savePath => Application.persistentDataPath + "/users.json";

    public static void SaveUsers(List<UserProfile> users)
    {
        UserDataWrapper wrapper = new UserDataWrapper { users = users };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Users saved to " + savePath);
    }

    public static List<UserProfile> LoadUsers()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No user data found. Creating new empty users file.");

            // Create empty users list and save it
            List<UserProfile> emptyList = new List<UserProfile>();
            SaveUsers(emptyList);

            return emptyList;
        }

        string json = File.ReadAllText(savePath);
        UserDataWrapper wrapper = JsonUtility.FromJson<UserDataWrapper>(json);
        return wrapper.users;
    }

}
