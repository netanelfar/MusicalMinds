using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Loads users and updates the UI to show the current user's information.
public class UserDisplayUpdater : MonoBehaviour
{
    [Header("Profile Pictures")] // Profile photos list
    public List<Sprite> profilePictures;

    public List<UserProfile> Users { get; private set; } // Loaded users

    [Header("UI References")]
    public TMP_Text usernameText;
    public TMP_Text levelText;
    public Image profileImage;

    private void Awake()
    {
        LoadUsers();
    }

    public void LoadUsers()
    {
        Users = UserDataManager.LoadUsers();
    }

    public void UpdateUserDisplay()
    {
        if (CurrentUserManager.CurrentUser != null)
        {
            usernameText.gameObject.SetActive(true);
            levelText.gameObject.SetActive(true);
            profileImage.gameObject.SetActive(true);

            usernameText.text = CurrentUserManager.CurrentUser.username;
            levelText.text = "Level: " + CurrentUserManager.CurrentUser.level;

            int profileIndex = CurrentUserManager.CurrentUser.ProfileINDX;
            Debug.Log("Trying to show profile index: " + profileIndex);
            Debug.Log("Profile picture list count: " + profilePictures.Count);

            if (profileIndex >= 0 && profileIndex < profilePictures.Count)
            {
                profileImage.sprite = profilePictures[profileIndex];
                Debug.Log("Successfully set profile picture with index: " + profileIndex);
            }
            else
            {
                Debug.LogWarning("Invalid profile index: " + profileIndex + ", using default");
                profileImage.sprite = profilePictures.Count > 0 ? profilePictures[0] : null;
            }
        }
        else
        {
            ClearUserDisplay();
        }
    }

    public void ClearUserDisplay()
    {
        usernameText.gameObject.SetActive(false);
        levelText.gameObject.SetActive(false);
        profileImage.gameObject.SetActive(false);
    }
}