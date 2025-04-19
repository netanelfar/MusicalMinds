using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class NewUserCreator : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInput;
    public Button createButton;
    public GameObject createUserPanel; // Panel with input field and create button
    public GameObject UsersPanel;
    public List<Button> profilePictureButtons; // Profile picture selection buttons

    private FixedUserSlotsLoader userSlotsLoader;
    private UserDataProvider userDataProvider;

    private int ProfileINDX = 0;

    void Awake()
    {
        userSlotsLoader = GetComponent<FixedUserSlotsLoader>();
        userDataProvider = GetComponent<UserDataProvider>();
    }

    void Start()
    {
        Debug.Log("Loaded users at Start:");
        foreach (var user in userDataProvider.Users)
        {
            Debug.Log("- " + user.username);
        }

        userSlotsLoader.ReloadUserButtons(); // Refresh user buttons
        createUserPanel.SetActive(false);
    }

    public void OpenCreateUserPanel()
    {
        UsersPanel.SetActive(false);
        createUserPanel.SetActive(true);
        usernameInput.text = "";
        usernameInput.Select();
    }

    public void ConfirmClicked()
    {
        string username = usernameInput.text.Trim();

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogWarning("Username cannot be empty.");
            return;
        }

        if (userDataProvider.Users.Exists(u => u.username == username))
        {
            Debug.LogWarning("Username already exists.");
            return;
        }

        UserProfile newUser = new UserProfile
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
            ProfileINDX = ProfileINDX
        };

        userDataProvider.Users.Add(newUser); // Add to provider's list
        UserDataManager.SaveUsers(userDataProvider.Users); // Save updated list
        UserManager.SetCurrentUser(newUser);

        Debug.Log("Created and set current user: " + username);

        createUserPanel.SetActive(false);
        UsersPanel.SetActive(true);
        userSlotsLoader.ReloadUserButtons();
        userDataProvider.UpdateUserDisplay(); // Update displayed current user immediately
    }

    public void SelectProfilePicture(int index)
    {
        ProfileINDX = index;
        Debug.Log("Selected profile picture: " + index);

        for (int i = 0; i < profilePictureButtons.Count; i++)
        {
            if (i == index)
            {
                profilePictureButtons[i].interactable = false;
                profilePictureButtons[i].transform.localScale = Vector3.one * 1.2f;
            }
            else
            {
                profilePictureButtons[i].interactable = true;
                profilePictureButtons[i].transform.localScale = Vector3.one;
            }
        }
    }

    private void OnDestroy()
    {
        createButton.onClick.RemoveListener(ConfirmClicked);
    }
}
