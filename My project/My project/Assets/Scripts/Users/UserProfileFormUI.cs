using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

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
    private bool mustOpenCreateUserPanel = false;

    public ProfilePictureCarouselLoader profilePicCarousel;

    void Awake()
    {
        userSlotsLoader = GetComponent<UserCarouselLoader>();
        userSlotsLoader.OnUserSelected += HandleUserSelectedExternally;

        userDataProvider = GetComponent<UserDisplayUpdater>();

        // Connect to carousel events - THIS IS CRITICAL for picture selection to work!
        if (profilePicCarousel != null)
        {
            // Make sure we remove any existing handlers first to prevent duplicates
            profilePicCarousel.OnProfilePictureSelected -= SelectProfilePicture;
            profilePicCarousel.OnProfilePictureSelected += SelectProfilePicture;
            // Initialize carousel when the component awakens
            profilePicCarousel.SetupProfilePictureButtons();
            Debug.Log("ProfilePictureCarousel connected successfully");
        }
        else
        {
            Debug.LogError("ProfilePictureCarousel reference is missing! Profile selection won't work correctly.");
        }

        // Keep the existing button setup for backward compatibility
        for (int i = 0; i < profilePictureButtons.Count; i++)
        {
            int index = i; // capture correct index for each button
            profilePictureButtons[i].onClick.RemoveAllListeners(); // prevent duplicates
            profilePictureButtons[i].onClick.AddListener(() => SelectProfilePicture(index));
        }
    }

    void Start()
    {
        userDataProvider.LoadUsers(); // Get latest data
        var users = userDataProvider.Users;

        string lastUsername = CurrentUserManager.GetLastConnectedUsername();
        UserProfile lastUser = users.Find(u => u.username == lastUsername);

        if (lastUser != null)
        {
            CurrentUserManager.SetCurrentUser(lastUser);
            userDataProvider.UpdateUserDisplay();
            settingsManager.ShowUsersOptions();
        }
        else if (users.Count == 0)
        {
            Debug.Log("No users found. Opening New User Panel...");
            mustCreateUser = true;
            mustOpenCreateUserPanel = true;
            settingsManager.ShowNewUsersPNL();
            settingsManager.SetUIInteractable(false);
        }
        else
        {
            // Fallback: no saved user, but users exist
            settingsManager.ShowUsersOptions();
            userDataProvider.ClearUserDisplay();
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
            ProfileINDX = 0; // Default to first profile picture
            Debug.Log("Creating new user with default profile index: " + ProfileINDX);
        }
        else
        {
            isEditMode = true;
            userBeingEdited = userToEdit;
            usernameInput.text = userToEdit.username;
            ProfileINDX = userToEdit.ProfileINDX;
            Debug.Log("Editing user with profile index: " + ProfileINDX);
        }

        // Make sure the carousel is updated to show correct selection
        if (profilePicCarousel != null)
        {
            // Navigate to the page containing the profile picture
            profilePicCarousel.NavigateToPageContainingIndex(ProfileINDX);

            // Set the selected picture in the carousel
            profilePicCarousel.SetSelectedPicture(ProfileINDX);
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

        Debug.Log("Saving user with profile index: " + ProfileINDX);

        if (isEditMode)
        {
            UserEditorService.EditUser(userBeingEdited, username, ProfileINDX);
            UserDataManager.SaveUsers(userDataProvider.Users);
            CurrentUserManager.SetCurrentUser(userBeingEdited);
        }
        else
        {
            UserProfile newUser = UserEditorService.CreateNewUser(username, ProfileINDX);
            userDataProvider.Users.Add(newUser);
            UserDataManager.SaveUsers(userDataProvider.Users);
            CurrentUserManager.SetCurrentUser(newUser);
        }

        if (mustCreateUser)
        {
            mustCreateUser = false;
            settingsManager.SetUIInteractable(true);
        }

        createUserPanel.SetActive(false);
        usernameInput.text = "";

        settingsManager.CloseOptions();

        userSlotsLoader.ReloadUserButtons();
        userDataProvider.UpdateUserDisplay();
    }

    public void SelectProfilePicture(int index)
    {
        ProfileINDX = index; // This is the CRUCIAL line that sets which profile pic to save
        Debug.Log("Selected profile picture: " + index + " - ProfileINDX is now: " + ProfileINDX);

        // Visually highlight the selected picture in carousel
        if (profilePicCarousel != null)
        {
            profilePicCarousel.SetSelectedPicture(index);
        }

        // Also update your local buttons list if you're still using it
        for (int i = 0; i < profilePictureButtons.Count; i++)
        {
            bool selected = i == index % profilePictureButtons.Count; // Use modulo to match displayed buttons
            profilePictureButtons[i].transform.localScale = selected ? Vector3.one * 1.2f : Vector3.one;
        }
    }

    public void DeleteUser()
    {
        if (userBeingEdited != null)
        {
            userDataProvider.Users.Remove(userBeingEdited);
            UserDataManager.SaveUsers(userDataProvider.Users);

            CurrentUserManager.SetCurrentUser(null);
            Debug.Log("User deleted.");

            createUserPanel.SetActive(false);

            if (userDataProvider.Users.Count > 0)
            {
                UserProfile fallbackUser = userDataProvider.Users[userDataProvider.Users.Count - 1];
                CurrentUserManager.SetCurrentUser(fallbackUser);
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
        CurrentUserManager.SetCurrentUser(user);
        userDataProvider.UpdateUserDisplay();
    }

    private void OnDestroy()
    {
        createButton.onClick.RemoveListener(ConfirmClicked);
    }
}