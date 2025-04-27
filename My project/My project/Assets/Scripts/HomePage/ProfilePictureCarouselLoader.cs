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

    private int currentPageIndex = 0;
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
    }

    private void HandleProfilePictureClicked(int pictureIndex)
    {
        selectedPictureIndex = pictureIndex;
        HighlightSelectedPicture(); // Visual highlight
        OnProfilePictureSelected?.Invoke(pictureIndex); // Notify listeners
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
                    pictureImage.color = Color.yellow; // Example: Yellow border
                else
                    pictureImage.color = Color.white; // Default
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
}
