using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FixedUserSlotsLoader : MonoBehaviour
{
    [Header("Fixed Buttons")]
    public List<Button> userButtons; // Buttons for user slots

    [Header("Navigation Buttons")]
    public Button nextButton;
    public Button previousButton;

    private UserDataProvider userDataProvider; // NEW: Use shared user data
    private List<UserProfile> users;
    private int currentPageIndex = 0;
    private int usersPerPage => userButtons.Count;

    private void Awake()
    {
        userDataProvider = GetComponent<UserDataProvider>(); // Find UserDataProvider on same object
    }

    void Start()
    {
        userDataProvider = GetComponent<UserDataProvider>();
        users = userDataProvider.Users;

        string lastUsername = UserManager.GetLastConnectedUsername();
        if (!string.IsNullOrEmpty(lastUsername))
        {
            UserProfile lastUser = users.Find(u => u.username == lastUsername);
            if (lastUser != null)
            {
                UserManager.SetCurrentUser(lastUser);
                userDataProvider.UpdateUserDisplay();
            }
            else
            {
                Debug.LogWarning("Last connected user not found.");
                userDataProvider.ClearUserDisplay();
            }
        }
        else
        {
            userDataProvider.ClearUserDisplay();
        }

        SetupUserButtons();
    }


    private void SetupUserButtons()
    {
        int startIdx = currentPageIndex * usersPerPage;

        for (int i = 0; i < userButtons.Count; i++)
        {
            Button button = userButtons[i];
            TMP_Text nameText = button.transform.Find("UserNameTXT")?.GetComponent<TMP_Text>();
            Image profileImage = button.transform.Find("UserIMG")?.GetComponent<Image>();

            int userIdx = startIdx + i;

            if (userIdx < users.Count)
            {
                UserProfile user = users[userIdx];

                if (nameText != null)
                    nameText.text = user.username;

                if (profileImage != null && user.ProfileINDX >= 0 && user.ProfileINDX < userDataProvider.profilePictures.Count)
                    profileImage.sprite = userDataProvider.profilePictures[user.ProfileINDX];

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnUserSelected(user));
                button.interactable = true;
            }
            else
            {
                if (nameText != null)
                    nameText.text = "Empty Slot";

                if (profileImage != null)
                    profileImage.sprite = null;

                button.onClick.RemoveAllListeners();
                button.interactable = false;
            }
        }

        UpdateNavigationButtons();
    }

    private void UpdateNavigationButtons()
    {
        int maxPage = (users.Count - 1) / usersPerPage;

        if (nextButton != null)
            nextButton.interactable = currentPageIndex < maxPage;

        if (previousButton != null)
            previousButton.interactable = currentPageIndex > 0;
    }

    private void OnUserSelected(UserProfile user)
    {
        UserManager.SetCurrentUser(user);
        Debug.Log("Switched to user: " + user.username);

        userDataProvider.UpdateUserDisplay(); //  Update user info immediately
    }

    public void ReloadUserButtons()
    {
        userDataProvider.LoadUsers(); // Reload users from file
        users = userDataProvider.Users;
        currentPageIndex = 0;
        SetupUserButtons();
    }

    public void OnNextPage()
    {
        int maxPage = (users.Count - 1) / usersPerPage;
        if (currentPageIndex < maxPage)
        {
            currentPageIndex++;
            SetupUserButtons();
        }
    }

    public void OnPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            SetupUserButtons();
        }
    }
}
