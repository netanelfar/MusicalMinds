using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class NewUserCreator : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public Button createButton;
    public GameObject createUserPanel; // Panel with input field and create button
    public GameObject UsersPannel;
    private FixedUserSlotsLoader userSlotsLoader;

    private int ProfileINDX = 0;

    private List<UserProfile> users;

    void Awake()
    {
        userSlotsLoader = GetComponent<FixedUserSlotsLoader>();
    }

    void Start()
    {
        users = UserDataManager.LoadUsers();
        Debug.Log("Loaded users at Start:");
        foreach (var user in users)
        {
            Debug.Log("- " + user.username);
        }

        userSlotsLoader.ReloadUserButtons(); //  refresh buttons
        createUserPanel.SetActive(false);
    }


    public void OpenCreateUserPanel()
    {
        UsersPannel.SetActive(false);
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

        if (users.Exists(u => u.username == username))
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

        users.Add(newUser);
        UserDataManager.SaveUsers(users);
        UserManager.SetCurrentUser(newUser);

        Debug.Log("Created and set current user: " + username);

        createUserPanel.SetActive(false); // Close the panel after creation
        UsersPannel.SetActive(true);
        userSlotsLoader.ReloadUserButtons();

    }

    public void SelectProfilePicture(int index)
    {
        ProfileINDX = index;
        Debug.Log("Selected profile picture: " + index);
    }

    private void OnDestroy()
    {
        createButton.onClick.RemoveListener(ConfirmClicked);
    }
}
