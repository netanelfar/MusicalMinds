using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

// Manages user profile carousel display.
public class UserCarouselLoader : MonoBehaviour
{
    [Header("Fixed Buttons")]
    public List<Button> userButtons;

    [Header("Navigation Buttons")]
    public Button nextButton;
    public Button previousButton;

    private UserUI userUI;
    private List<UserProfile> users;
    private int currentPageIndex = 0;
    private int usersPerPage => userButtons.Count;

    // Cached references.
    private List<TMP_Text> cachedNameTexts = new List<TMP_Text>();
    private List<Image> cachedProfileImages = new List<Image>();

    private int maxPage => users != null && users.Count > 0 ? (users.Count - 1) / usersPerPage : 0;

    public event Action<UserProfile> OnUserSelected;

    // Initialize UserUI reference. 
    private void Awake()
    {
        userUI = GetComponent<UserUI>();
        CacheComponentReferences();

    }

    // Cache component references on startup.
    private void CacheComponentReferences()
    {
        foreach (Button button in userButtons)
        {
            TMP_Text nameText = button.transform.Find("UserNameTXT")?.GetComponent<TMP_Text>();
            Image profileImage = button.transform.Find("UserIMG")?.GetComponent<Image>();

            cachedNameTexts.Add(nameText);
            cachedProfileImages.Add(profileImage);
        }
    }

    // Populate user selection buttons for current page.
    private void SetupUserButtons()
    {
        int startIdx = currentPageIndex * usersPerPage;

        for (int i = 0; i < userButtons.Count; i++)
        {
            Button button = userButtons[i];
            TMP_Text nameText = cachedNameTexts[i];
            Image profileImage = cachedProfileImages[i];
            int userIdx = startIdx + i;

            if (userIdx < users.Count)
            {
                UserProfile user = users[userIdx];

                if (nameText != null)
                    nameText.text = user.username;

                // Get profile pictures from UI.
                if (profileImage != null && userUI != null && userUI.profilePictures != null &&
                    user.ProfileINDX >= 0 && user.ProfileINDX < userUI.profilePictures.Count)
                {
                    profileImage.sprite = userUI.profilePictures[user.ProfileINDX];
                }
                else if (profileImage != null)
                {
                    profileImage.sprite = null;
                }

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => HandleUserButtonClicked(user));
                button.interactable = true;
            }
            else
            {
                SetButtonToEmptyState(i);
            }
        }

        UpdateNavigationButtons();
    }

    // Set button to empty state.
    private void SetButtonToEmptyState(int buttonIndex)
    {
        Button button = userButtons[buttonIndex];
        TMP_Text nameText = cachedNameTexts[buttonIndex];
        Image profileImage = cachedProfileImages[buttonIndex];

        if (nameText != null)
            nameText.text = "";

        if (profileImage != null)
            profileImage.sprite = null;

        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = new Color(1f, 1f, 1f, 0.2f); // White with 20% opacity
        }

        button.onClick.RemoveAllListeners();
        button.interactable = false;
    }

    // Enable/disable navigation buttons based on current page.
    private void UpdateNavigationButtons()
    {
        if (nextButton != null)
            nextButton.interactable = currentPageIndex < maxPage;

        if (previousButton != null)
            previousButton.interactable = currentPageIndex > 0;
    }

    // Trigger user selection event.
    private void HandleUserButtonClicked(UserProfile user)
    {
        OnUserSelected?.Invoke(user);
    }

    // Reload user data and refresh button.
    public void ReloadUserButtons()
    {
        users = UserManager.LoadUsers();
        currentPageIndex = 0;

        if (users == null || users.Count == 0)
        {
            Debug.LogWarning("No users after reload. Skipping user button setup.");
            ClearAllUserButtons();
            return;
        }

        SetupUserButtons();
    }

    // Reset all user buttons to empty state.
    private void ClearAllUserButtons()
    {
        for (int i = 0; i < userButtons.Count; i++)
        {
            SetButtonToEmptyState(i);
        }
        if (nextButton != null)
            nextButton.interactable = false;
        if (previousButton != null)
            previousButton.interactable = false;
    }


    // Navigate to next page.
    public void OnNextPage()
    {
        int maxPage = (users.Count - 1) / usersPerPage;
        if (currentPageIndex < maxPage)
        {
            currentPageIndex++;
            SetupUserButtons();
        }
    }

    // Navigate to previous page.
    public void OnPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            SetupUserButtons();
        }
    }
}