using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

// Manages devided profile picture selection.
public class ProfilePictureCarouselLoader : MonoBehaviour
{
    [Header("Profile Picture Buttons")]
    public List<Button> profilePictureButtons; 

    [Header("Navigation Buttons")]
    public Button nextButton;
    public Button previousButton;

    [Header("Profile Pictures")]
    public List<Sprite> availableProfilePictures; 

    public int currentPageIndex = 0; 
    private int picturesPerPage => profilePictureButtons.Count;
    private int selectedPictureIndex = -1;

    public event Action<int> OnProfilePictureSelected; // Pass selected index
    private List<Image> cachedImages = new List<Image>();

    // Setup profile picture buttons for current page.
    public void SetupProfilePictureButtons()
    {
        if (cachedImages.Count == 0)
        {
            foreach (Button button in profilePictureButtons)
            {
                cachedImages.Add(button.transform.Find("ProfileIMG")?.GetComponent<Image>());
            }
        }

        int startIdx = currentPageIndex * picturesPerPage;
        for (int i = 0; i < profilePictureButtons.Count; i++)
        {
            Button button = profilePictureButtons[i];
            Image pictureImage = cachedImages[i]; 

            int pictureIdx = startIdx + i;

            if (pictureIdx < availableProfilePictures.Count)
            {
                if (pictureImage != null)
                    pictureImage.sprite = availableProfilePictures[pictureIdx];

                int capturedIndex = pictureIdx;
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
        HighlightSelectedPicture();
    }
    // Handle profile picture selection .
    private void HandleProfilePictureClicked(int pictureIndex)
    {
        Debug.LogWarning(">>> Picture clicked in carousel, index = " + pictureIndex);

        selectedPictureIndex = pictureIndex;
        HighlightSelectedPicture();

        if (OnProfilePictureSelected != null)
        {
            Debug.Log(">>> Firing OnProfilePictureSelected with index: " + pictureIndex);
            OnProfilePictureSelected.Invoke(pictureIndex); // Notify listeners
        }
       
    }

    // Set selected picture externally.
    public void SetSelectedPicture(int pictureIndex)
    {
        selectedPictureIndex = pictureIndex;
        HighlightSelectedPicture();
    }

    // Apply visual highlighting to selected picture.
    private void HighlightSelectedPicture()
    {
        int startIdx = currentPageIndex * picturesPerPage;

        for (int i = 0; i < profilePictureButtons.Count; i++)
        {
            Button button = profilePictureButtons[i];
            //Image pictureImage = button.transform.Find("ProfileIMG")?.GetComponent<Image>();
            Image pictureImage = cachedImages[i]; // Use cached instead of Find

            int pictureIdx = startIdx + i;

            if (pictureImage != null)
            {
                if (pictureIdx == selectedPictureIndex)
                    pictureImage.color = Color.yellow; 
                else
                    pictureImage.color = Color.white; 
            }
        }
    }

    // Enable/disable navigation buttons based on current page.
    private void UpdateNavigationButtons()
    {
        int maxPage = (availableProfilePictures.Count - 1) / picturesPerPage;

        if (nextButton != null)
            nextButton.interactable = currentPageIndex < maxPage;

        if (previousButton != null)
            previousButton.interactable = currentPageIndex > 0;
    }

    // Navigate to next page of profile pictures.
    public void OnNextPage()
    {
        int maxPage = (availableProfilePictures.Count - 1) / picturesPerPage;
        if (currentPageIndex < maxPage)
        {
            currentPageIndex++;
            SetupProfilePictureButtons();
        }
    }

    // Navigate to previous page of profile pictures.
    public void OnPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            SetupProfilePictureButtons();
        }
    }

    // Helper method to navigate to the page containing a specific picture index.
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