
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// Handles user-related interface logic.
public class UserUI : MonoBehaviour
{
    [Header("Display UI")]
    public TMP_Text usernameText;
    public TMP_Text levelText;
    public UnityEngine.UI.Image profileImage;
    public TMP_Text inputCheckTEXT;

    public List<Sprite> profilePictures;

    [Header("Parent Info")]
    public GameObject parentInfoPanel;
    public Button profileImageButton;
    public ParentInfoDisplay parentInfoDisplay; 



    // User panal UI
    [Header("Form UI")]
    public GameObject createUserPanel;
    public ComputerKeyboardInput usernameInput;
    public UnityEngine.UI.Button createButton;

    [Header("References")]
    public SettingsUIController settingsManager;
    public ProfilePictureCarouselLoader profilePicCarousel;

    // Get carousel from same GameObject
    private UserCarouselLoader userCarousel;

    // Current state
    public List<UserProfile> Users { get; private set; }
    private int selectedProfileIndex = 0; // currently connected user profile picture indx.
    private bool isEditMode = false;
    private UserProfile userBeingEdited = null;
    private bool mustCreateUser = false;
    private bool mustOpenCreateUserPanel = false;

    // Initializes references and subscribes to user/profile events
    void Awake()
    {
        userCarousel = GetComponent<UserCarouselLoader>();

        // Update display when current user changes
        UserManager.OnCurrentUserChanged += UpdateUserDisplay;

        // Set selected user from carousel.
        if (userCarousel != null)
        userCarousel.OnUserSelected += SelectUser;

        // Handle profile picture selection in the carousel
        if (profilePicCarousel != null)
        {
            profilePicCarousel.OnProfilePictureSelected -= SelectProfilePicture;
            profilePicCarousel.OnProfilePictureSelected += SelectProfilePicture;
            profilePicCarousel.SetupProfilePictureButtons();
        }
    }

    // Loads users from file and update UI.
    void Start()
    {
        if (EventSystem.current == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        LoadUsers();
        UserManager.EnsureCurrentUserLoadedFrom(Users);
        UpdateUserDisplay(UserManager.CurrentUser);

        // Open create user if there aren't players yet.
        if (UserManager.CurrentUser == null)
        {
            Debug.Log("No users found. Opening New User Panel...");
            mustCreateUser = true;
            mustOpenCreateUserPanel = true;
            settingsManager.ShowNewUsersPNL();
        }
        else
        {
            createUserPanel.SetActive(false);
        }
        userCarousel?.ReloadUserButtons();
    }

    // Checks if the create panel should be opened.
    void Update()
    {
        if (mustOpenCreateUserPanel)
        {
            mustOpenCreateUserPanel = false;
            OpenCreateUserPanel();
        }
    }

    // Toggle user data panal.
    public void ToggleParentInfoPanel()
    {
        if (parentInfoPanel != null)
        {
            bool wasActive = parentInfoPanel.activeSelf;
            parentInfoPanel.SetActive(!wasActive);
            if (!wasActive && parentInfoPanel.activeSelf && parentInfoDisplay != null)
            {
                parentInfoDisplay.UpdateParentInfo();
            }
        }
    }


    // Loads all user profiles from storage.
    public void LoadUsers()
    {
        Users = UserManager.LoadUsers();
    }

    // Updates UI elements with current user information.
    public void UpdateUserDisplay(UserProfile user)
    {
        if (user != null)
        {
            usernameText.gameObject.SetActive(true);
            levelText.gameObject.SetActive(true);
            profileImage.gameObject.SetActive(true);

            usernameText.text = "Name: " + user.username;
            levelText.text = "Level: " + user.level;

            if (user.ProfileINDX >= 0 && user.ProfileINDX < profilePictures.Count)
            {
                profileImage.sprite = profilePictures[user.ProfileINDX];
                Debug.Log("Successfully set profile picture with index: " + user.ProfileINDX);
            }
            else
            {
                Debug.LogWarning("Invalid profile index: " + user.ProfileINDX + ", using default");
                profileImage.sprite = profilePictures.Count > 0 ? profilePictures[0] : null;
            }
        }
        else
        {
            ClearUserDisplay();
        }
    }

    // Hides all user display elements.
    public void ClearUserDisplay()
    {
        usernameText.gameObject.SetActive(false);
        levelText.gameObject.SetActive(false);
        profileImage.gameObject.SetActive(false);
    }

    // Sets the selected user as current.
    public void SelectUser(UserProfile user)
    {
        UserManager.SetCurrentUser(user);
        settingsManager.CloseOptions();
    }

    // Opens user creation/editing panel with appropriate mode setup.
    public void OpenCreateUserPanel(UserProfile userToEdit = null)
    {
        createUserPanel.SetActive(true);

        if (userToEdit == null)
        {
            // Create mode
            isEditMode = false;
            userBeingEdited = null;
            usernameInput.SetText("");
            inputCheckTEXT.text = "";
            selectedProfileIndex = 0;
        }
        else
        {
            // Edit mode
            isEditMode = true;
            userBeingEdited = userToEdit;
            usernameInput.SetText(userToEdit.username);
            inputCheckTEXT.text = "";
            selectedProfileIndex = userToEdit.ProfileINDX;
        }

        // Update profile picture carousel
        if (profilePicCarousel != null)
        {
            profilePicCarousel.NavigateToPageContainingIndex(selectedProfileIndex);
            profilePicCarousel.SetSelectedPicture(selectedProfileIndex);
        }
    }

    // Updates selected profile index and display.
    public void SelectProfilePicture(int index)
    {
        selectedProfileIndex = index;
        if (profilePicCarousel != null)
        {
            profilePicCarousel.SetSelectedPicture(index);
        }
    }

    // Validates and saves user creation/editing.
    public void ConfirmUser()
    {
        if (usernameInput == null)
        {
            Debug.LogError("Username input field is null!");
            return;
        }
        string username = usernameInput.GetText().Trim();

        if (string.IsNullOrEmpty(username))
        {
            inputCheckTEXT.text = "Username cannot be empty.";
            return;
        }

        if (!isEditMode && UserManager.IsUsernameTaken(Users, username))
        {
            inputCheckTEXT.text = "Username already exists.";
            return;
        }
        inputCheckTEXT.text = "";
        if (isEditMode)
        {
            // Edit existing user
            UserManager.EditUser(userBeingEdited, username, selectedProfileIndex);
            UserManager.SaveUsers(Users);
            UserManager.SetCurrentUser(userBeingEdited);
        }
        else
        {
            // Create new user
            UserProfile newUser = UserManager.CreateNewUser(username, selectedProfileIndex);
            Users.Add(newUser);
            UserManager.SaveUsers(Users);
            UserManager.SetCurrentUser(newUser);
        }

        if (mustCreateUser)
        {
            mustCreateUser = false;
        }

        CloseCreatePanel();
        userCarousel?.ReloadUserButtons();
    }

    // Deletes current user and handles UI.
    public void DeleteUser()
    {
        UserProfile userToDelete = userBeingEdited ?? UserManager.CurrentUser;

        if (userToDelete != null)
        {
            Debug.Log("Deleting user: " + userToDelete.username);

            // Let UserManager handle the deletion logic.
            Users = UserManager.DeleteUser(userToDelete);

            // Clear the userBeingEdited. 
            userBeingEdited = null;

            // Handle UI updates.
            if (Users.Count == 0)
            {
                Debug.Log("No users left, showing create first user panel");
                ShowCreateFirstUser();
            }
            else
            {
                Debug.Log("Users remain, showing users options. Current user: " + (UserManager.CurrentUser?.username ?? "None"));
                settingsManager.ShowUsersOptions();
                CloseCreatePanel();
            }
            
            UpdateUserDisplay(UserManager.CurrentUser);
            userCarousel?.ReloadUserButtons();

        }
        else
        {
            Debug.LogWarning("No user to delete");
        }
    }

    /// ////////////
    private void CloseCreatePanel()
    {
        createUserPanel.SetActive(false);
        settingsManager.CloseOptions();

    }

    // Resets form state variables and clears input fields.
    public void ResetFormState()
    {
        isEditMode = false;
        userBeingEdited = null;
        selectedProfileIndex = 0;

        // Clear input fields.
        if (usernameInput != null)
        {
            usernameInput.SetText("");
        }
        if (inputCheckTEXT != null)
        {
            inputCheckTEXT.text = "";
        }
    }

    /// //////////
    private void ShowCreateFirstUser()
    {
        settingsManager.ShowNewUsersPNL();
        //settingsManager.SetUIInteractable(false);
    }

    // Edit current user.
    public void EditCurrentUser()
    {
        if (UserManager.CurrentUser != null)
        {
            OpenCreateUserPanel(UserManager.CurrentUser);
        }
    }

    /// /////
    // Unsubscribes from events when component is destroyed
    void OnDestroy()
    {
        UserManager.OnCurrentUserChanged -= UpdateUserDisplay;
    }

}
