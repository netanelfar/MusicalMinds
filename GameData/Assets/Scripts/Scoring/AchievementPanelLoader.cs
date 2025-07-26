using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

// Manages achievement panel UI with category-based pagination and unlocked-first sorting.
public class AchievementPanelLoader : MonoBehaviour
{
    [Header("Note Recognition Section")]
    public List<GameObject> noteRecognitionSlots;
    public Button noteRecognitionNextButton;
    public Button noteRecognitionPreviousButton;

    [Header("Melody Play Section")]
    public List<GameObject> melodyPlaySlots;
    public Button melodyPlayNextButton;
    public Button melodyPlayPreviousButton;

    [Header("Free Play Section")]
    public List<GameObject> freePlaySlots;
    public Button freePlayNextButton;
    public Button freePlayPreviousButton;

    [Header("References")]
    public SimpleAchievementSystem achievementSystem;

    [Header("Panel Reference")]
    public GameObject achievementPanel;

    // Page tracking for each category.
    private int noteRecognitionPageIndex = 0;
    private int melodyPlayPageIndex = 0;
    private int freePlayPageIndex = 0;

    // Initialize achievements.
    void Start()
    {
        LoadAllAchievements();
        if (achievementPanel != null)
            achievementPanel.SetActive(false); // Start hidden.
    }

    // Toggles panel visibility.
    public void ToggleAchievemenPanel()
    {
        if (achievementPanel == null) return;

        bool isActive = achievementPanel.activeSelf;
        if (!isActive)
        {
            LoadAllAchievements();  // Refresh.
        }
        achievementPanel.SetActive(!isActive);
    }

    // Loads achievements for all categories and updates navigation.
    void LoadAllAchievements()
    {
        LoadCategoryAchievements(AchievementCategory.NoteRecognition, noteRecognitionSlots, noteRecognitionPageIndex);
        LoadCategoryAchievements(AchievementCategory.MelodyPlay, melodyPlaySlots, melodyPlayPageIndex);
        LoadCategoryAchievements(AchievementCategory.FreePlay, freePlaySlots, freePlayPageIndex);
        UpdateAllNavigationButtons();
    }

    // Populates UI slots with sorted achievements (unlocked first, then locked).
    void LoadCategoryAchievements(AchievementCategory category, List<GameObject> slots, int pageIndex)
    {
        if (slots == null || slots.Count == 0) return;

        List<int> sortedAchievements = new List<int>();
        int totalInCategory = achievementSystem.GetTotalAchievements(category);

        // Unlocked achievements first.
        for (int i = 0; i < totalInCategory; i++)
        {
            if (achievementSystem.IsUnlocked(category, i))
            {
                sortedAchievements.Add(i);
            }
        }

        // Add locked achievements.
        for (int i = 0; i < totalInCategory; i++)
        {
            if (!achievementSystem.IsUnlocked(category, i))
            {
                sortedAchievements.Add(i);
            }
        }

        int startIdx = pageIndex * slots.Count;

        for (int i = 0; i < slots.Count; i++)
        {
            GameObject slot = slots[i];
            int sortedIndex = startIdx + i;

            var iconImage = slot.transform.Find("AchievementIcon")?.GetComponent<Image>();
            var titleText = slot.transform.Find("AchievementTitle")?.GetComponent<TMPro.TMP_Text>();
            var descriptionText = slot.transform.Find("AchievementDescription")?.GetComponent<TMPro.TMP_Text>();

            if (sortedIndex < sortedAchievements.Count)
            {
                int achievementIndex = sortedAchievements[sortedIndex];
                var data = achievementSystem.GetAchievementData(category, achievementIndex);
                var icon = achievementSystem.GetAchievementIcon(category, achievementIndex);
                bool unlocked = achievementSystem.IsUnlocked(category, achievementIndex);

                Debug.Log($"Loading {category} Achievement {achievementIndex}: {data?.title} → unlocked = {unlocked}");

                if (iconImage != null) iconImage.sprite = icon;
                if (titleText != null) titleText.text = data?.title ?? "Unknown";
                if (descriptionText != null) descriptionText.text = data?.description ?? "No description";

                if (!unlocked)  // Locked Achievements.
                {
                    if (iconImage != null) iconImage.color = new Color(1, 1, 1, 0.1f);
                    if (titleText != null) titleText.color = Color.gray;
                }
                else
                {
                    if (iconImage != null) iconImage.color = Color.white;
                    if (titleText != null) titleText.color = Color.white;
                    if (descriptionText != null) descriptionText.color = Color.white;
                }
                slot.SetActive(true);
            }
            else
            {
                // Hide unused slots
                if (iconImage != null) iconImage.sprite = null;
                if (titleText != null) titleText.text = "";
                if (descriptionText != null) descriptionText.text = "";
                slot.SetActive(false);
            }
        }
    }

    // Updates navigation button states for all categories.
    void UpdateAllNavigationButtons()
    {
        UpdateCategoryNavigationButtons(AchievementCategory.NoteRecognition, noteRecognitionSlots, noteRecognitionPageIndex, noteRecognitionNextButton, noteRecognitionPreviousButton);
        UpdateCategoryNavigationButtons(AchievementCategory.MelodyPlay, melodyPlaySlots, melodyPlayPageIndex, melodyPlayNextButton, melodyPlayPreviousButton);
        UpdateCategoryNavigationButtons(AchievementCategory.FreePlay, freePlaySlots, freePlayPageIndex, freePlayNextButton, freePlayPreviousButton);
    }

    // Enables/disables navigation buttons based on current page and total pages
    void UpdateCategoryNavigationButtons(AchievementCategory category, List<GameObject> slots, int pageIndex, Button nextButton, Button previousButton)
    {
        if (slots == null || slots.Count == 0) return;

        int totalInCategory = achievementSystem.GetTotalAchievements(category);
        int maxPage = (totalInCategory - 1) / slots.Count;

        if (nextButton != null)
            nextButton.interactable = pageIndex < maxPage;

        if (previousButton != null)
            previousButton.interactable = pageIndex > 0;
    }

    // ----- Note Recognition Navigation -----//

    // Next page of note recognition achievements.
    public void OnNoteRecognitionNextPage()
    {
        int totalInCategory = achievementSystem.GetTotalAchievements(AchievementCategory.NoteRecognition);
        int maxPage = (totalInCategory - 1) / noteRecognitionSlots.Count;
        if (noteRecognitionPageIndex < maxPage)
        {
            noteRecognitionPageIndex++;
            LoadCategoryAchievements(AchievementCategory.NoteRecognition, noteRecognitionSlots, noteRecognitionPageIndex);
            UpdateCategoryNavigationButtons(AchievementCategory.NoteRecognition, noteRecognitionSlots, noteRecognitionPageIndex, noteRecognitionNextButton, noteRecognitionPreviousButton);
        }
    }

    // Previous page of note recognition achievements.
    public void OnNoteRecognitionPreviousPage()
    {
        if (noteRecognitionPageIndex > 0)
        {
            noteRecognitionPageIndex--;
            LoadCategoryAchievements(AchievementCategory.NoteRecognition, noteRecognitionSlots, noteRecognitionPageIndex);
            UpdateCategoryNavigationButtons(AchievementCategory.NoteRecognition, noteRecognitionSlots, noteRecognitionPageIndex, noteRecognitionNextButton, noteRecognitionPreviousButton);
        }
    }

    // -----  Melody Play Navigation -----//

    // Next page of Melody Play achievements.
    public void OnMelodyPlayNextPage()
    {
        int totalInCategory = achievementSystem.GetTotalAchievements(AchievementCategory.MelodyPlay);
        int maxPage = (totalInCategory - 1) / melodyPlaySlots.Count;
        if (melodyPlayPageIndex < maxPage)
        {
            melodyPlayPageIndex++;
            LoadCategoryAchievements(AchievementCategory.MelodyPlay, melodyPlaySlots, melodyPlayPageIndex);
            UpdateCategoryNavigationButtons(AchievementCategory.MelodyPlay, melodyPlaySlots, melodyPlayPageIndex, melodyPlayNextButton, melodyPlayPreviousButton);
        }
    }

    // Previous page of Melody Play achievements.
    public void OnMelodyPlayPreviousPage()
    {
        if (melodyPlayPageIndex > 0)
        {
            melodyPlayPageIndex--;
            LoadCategoryAchievements(AchievementCategory.MelodyPlay, melodyPlaySlots, melodyPlayPageIndex);
            UpdateCategoryNavigationButtons(AchievementCategory.MelodyPlay, melodyPlaySlots, melodyPlayPageIndex, melodyPlayNextButton, melodyPlayPreviousButton);
        }
    }

    // -----  Free Play Navigation -----//

    // Next page of Free Play achievements.
    public void OnFreePlayNextPage()
    {
        int totalInCategory = achievementSystem.GetTotalAchievements(AchievementCategory.FreePlay);
        int maxPage = (totalInCategory - 1) / freePlaySlots.Count;
        if (freePlayPageIndex < maxPage)
        {
            freePlayPageIndex++;
            LoadCategoryAchievements(AchievementCategory.FreePlay, freePlaySlots, freePlayPageIndex);
            UpdateCategoryNavigationButtons(AchievementCategory.FreePlay, freePlaySlots, freePlayPageIndex, freePlayNextButton, freePlayPreviousButton);
        }
    }

    // Previous page of Free Play achievements.
    public void OnFreePlayPreviousPage()
    {
        if (freePlayPageIndex > 0)
        {
            freePlayPageIndex--;
            LoadCategoryAchievements(AchievementCategory.FreePlay, freePlaySlots, freePlayPageIndex);
            UpdateCategoryNavigationButtons(AchievementCategory.FreePlay, freePlaySlots, freePlayPageIndex, freePlayNextButton, freePlayPreviousButton);
        }
    }
}
