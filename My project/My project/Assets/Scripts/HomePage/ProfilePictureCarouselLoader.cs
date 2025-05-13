using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ProfilePictureCarouselLoader : MonoBehaviour
{
    [Header("Profile Picture Buttons")]
    public List<Button> profilePictureButtons; // Buttons to display pictures

    [Header("Navigation Buttons")]
    public Button nextButton;
    public Button previousButton;

    [Header("Profile Pictures")]
    public List<Sprite> availableProfilePictures; // 13 available pictures

    public int currentPageIndex = 0; // Made public so it can be accessed from other scripts
    private int picturesPerPage => profilePictureButtons.Count;
    private int selectedPictureIndex = -1;

    public event Action<int> OnProfilePictureSelected; // Pass selected index

    /*private void Start()
    {
        SetupProfilePictureButtons();
    }*/

    public void SetupProfilePictureButtons()
    {
        int startIdx = currentPageIndex * picturesPerPage;

        for (int i = 0; i < profilePictureButtons.Count; i++)
        {
            Button button = profilePictureButtons[i];
            Image pictureImage = button.transform.Find("ProfileIMG")?.GetComponent<Image>();

            int pictureIdx = startIdx + i;

            if (pictureIdx < availableProfilePictures.Count)
            {
                if (pictureImage != null)
                    pictureImage.sprite = availableProfilePictures[pictureIdx];

                int capturedIndex = pictureIdx; // Capture correct index
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => HandleProfilePictureClicked(capturedIndex));
                button.interactable = true;
            }
            else
            {
                if (pictureImage != null)
                    pictureImage.sprite = null;

                button.onClick.RemoveAllListeners();
                button.interactable = false;
            }
        }

        UpdateNavigationButtons();
        HighlightSelectedPicture(); // Make sure to highlight after setup
    }

    private void HandleProfilePictureClicked(int pictureIndex)
    {
        Debug.LogWarning(">>> Picture clicked in carousel, index = " + pictureIndex);

        selectedPictureIndex = pictureIndex;
        HighlightSelectedPicture(); // Visual highlight

        // Make sure the event is fired with the correct index
        if (OnProfilePictureSelected != null)
        {
            Debug.Log(">>> Firing OnProfilePictureSelected with index: " + pictureIndex);
            OnProfilePictureSelected.Invoke(pictureIndex); // Notify listeners
        }
        else
        {
            Debug.LogError(">>> No listeners for OnProfilePictureSelected event!");
        }
    }

    // This method can be called externally to set the selected picture
    public void SetSelectedPicture(int pictureIndex)
    {
        selectedPictureIndex = pictureIndex;
        HighlightSelectedPicture();
    }

    private void HighlightSelectedPicture()
    {
        int startIdx = currentPageIndex * picturesPerPage;

        for (int i = 0; i < profilePictureButtons.Count; i++)
        {
            Button button = profilePictureButtons[i];
            Image pictureImage = button.transform.Find("ProfileIMG")?.GetComponent<Image>();
            int pictureIdx = startIdx + i;

            if (pictureImage != null)
            {
                if (pictureIdx == selectedPictureIndex)
                    pictureImage.color = Color.yellow; // Selected color
                else
                    pictureImage.color = Color.white; // Default color
            }
        }
    }

    private void UpdateNavigationButtons()
    {
        int maxPage = (availableProfilePictures.Count - 1) / picturesPerPage;

        if (nextButton != null)
            nextButton.interactable = currentPageIndex < maxPage;

        if (previousButton != null)
            previousButton.interactable = currentPageIndex > 0;
    }

    public void OnNextPage()
    {
        int maxPage = (availableProfilePictures.Count - 1) / picturesPerPage;
        if (currentPageIndex < maxPage)
        {
            currentPageIndex++;
            SetupProfilePictureButtons();
        }
    }

    public void OnPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            SetupProfilePictureButtons();
        }
    }

    // Helper method to navigate to the page containing a specific picture index
    public void NavigateToPageContainingIndex(int pictureIndex)
    {
        int targetPage = pictureIndex / picturesPerPage;
        if (targetPage != currentPageIndex)
        {
            currentPageIndex = targetPage;
            SetupProfilePictureButtons();
        }
    }
}