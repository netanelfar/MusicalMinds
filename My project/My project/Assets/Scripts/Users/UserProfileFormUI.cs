using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

// Handles creating and saving new users
public class UserProfileFormUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInput;
    public Button createButton;

    [Header("Settings Manager")]
    public SettingsUIController settingsManager;

    public GameObject createUserPanel;
    public GameObject UsersPanel;
    public List<Button> profilePictureButtons;

    private UserCarouselLoader userSlotsLoader;
    private UserDisplayUpdater userDataProvider;

    private int ProfileINDX = 0;
    private bool isEditMode = false;
    private UserProfile userBeingEdited = null;
    private bool mustCreateUser = false;
    private bool mustOpenCreateUserPanel = false; // NEW

    void Awake()
    {
        userSlotsLoader = GetComponent<UserCarouselLoader>();
        userSlotsLoader.OnUserSelected += HandleUserSelectedExternally;

        userDataProvider = GetComponent<UserDisplayUpdater>();
    }

    void Start()
    {
        if (userDataProvider.Users == null || userDataProvider.Users.Count == 0)
        {
            Debug.Log("No users found. Opening New User Panel...");

            mustCreateUser = true;
            mustOpenCreateUserPanel = true;

            settingsManager.ShowNewUsersPNL();
            return;
        }

        Debug.Log("Loaded users at Start:");
        foreach (var user in userDataProvider.Users)
        {
            Debug.Log("- " + user.username);
        }

        userSlotsLoader.ReloadUserButtons();
        createUserPanel.SetActive(false);
    }


    void Update()
    {
        if (mustOpenCreateUserPanel)
        {
            mustOpenCreateUserPanel = false;
            OpenCreateUserPanel();
        }
    }

    public void OpenCreateUserPanel(UserProfile userToEdit = null)
    {
        createUserPanel.SetActive(true);

        if (userToEdit == null)
        {
            isEditMode = false;
            userBeingEdited = null;
            usernameInput.text = "";
            ProfileINDX = 0;
            SelectProfilePicture(ProfileINDX);
        }
        else
        {
            isEditMode = true;
            userBeingEdited = userToEdit;
            usernameInput.text = userToEdit.username;
            ProfileINDX = userToEdit.ProfileINDX;
            SelectProfilePicture(ProfileINDX);
        }

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

        if (!isEditMode && UserEditorService.IsUsernameTaken(userDataProvider.Users, username))
        {
            Debug.LogWarning("Username already exists.");
            return;
        }

        if (isEditMode)
        {
            UserEditorService.EditUser(userBeingEdited, username, ProfileINDX);
            UserDataManager.SaveUsers(userDataProvider.Users);
            UserManager.SetCurrentUser(userBeingEdited);
        }
        else
        {
            UserProfile newUser = UserEditorService.CreateNewUser(username, ProfileINDX);
            userDataProvider.Users.Add(newUser);
            UserDataManager.SaveUsers(userDataProvider.Users);
            UserManager.SetCurrentUser(newUser);
        }

        if (mustCreateUser)
        {
            mustCreateUser = false;
            settingsManager.SetUIInteractable(true);
        }
        //
        createUserPanel.SetActive(false);
        usernameInput.text = "";
//
        settingsManager.CloseOptions();

        userSlotsLoader.ReloadUserButtons();
        userDataProvider.UpdateUserDisplay();
    }

    public void SelectProfilePicture(int index)
    {
        ProfileINDX = index;
        Debug.Log("Selected profile picture: " + index);

        for (int i = 0; i < profilePictureButtons.Count; i++)
        {
            bool selected = i == index;
            profilePictureButtons[i].interactable = !selected;
            profilePictureButtons[i].transform.localScale = selected ? Vector3.one * 1.2f : Vector3.one;
        }
    }

    public void DeleteUser()
    {
        if (userBeingEdited != null)
        {
            userDataProvider.Users.Remove(userBeingEdited);
            UserDataManager.SaveUsers(userDataProvider.Users);

            UserManager.SetCurrentUser(null);
            Debug.Log("User deleted.");

            createUserPanel.SetActive(false);

            if (userDataProvider.Users.Count > 0)
            {
                UserProfile fallbackUser = userDataProvider.Users[userDataProvider.Users.Count - 1];
                UserManager.SetCurrentUser(fallbackUser);
                settingsManager.ShowUsersOptions();
                userSlotsLoader.ReloadUserButtons();
            }
            else
            {
                mustCreateUser = true;
                mustOpenCreateUserPanel = true;

                settingsManager.ShowNewUsersPNL();
                settingsManager.SetUIInteractable(false);
            }

            userDataProvider.UpdateUserDisplay();
        }
    }

    private void HandleUserSelectedExternally(UserProfile user)
    {
        UserManager.SetCurrentUser(user);
        userDataProvider.UpdateUserDisplay();
    }

    private void OnDestroy()
    {
        createButton.onClick.RemoveListener(ConfirmClicked);
    }
}
