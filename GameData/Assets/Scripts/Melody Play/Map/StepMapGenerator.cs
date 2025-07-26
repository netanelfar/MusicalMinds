using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Generates visual step in song progression map.
public class StepMapGenerator : MonoBehaviour
{
    public GameObject stepButtonPrefab;
    public Transform gridContainer;
    public MelodyPlayManager melodyManager;
    private string currentSongName;
    public TextMeshProUGUI SongTitle;
    private List<List<string>> currentSongSteps;

    // Creates step buttons.
    public void GenerateMap(string songName, List<List<string>> steps)
    {
        currentSongName = songName;
        currentSongSteps = steps;
        SongTitle.text = currentSongName;
        int userLevel = UserManager.GetSongLevel(songName);

        // Clear previous buttons
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < steps.Count; i++)
        {
            int stepIndex = i;
            GameObject btn = Instantiate(stepButtonPrefab, gridContainer);
            StepButton stepButton = btn.GetComponent<StepButton>();

            bool isUnlocked = stepIndex <= userLevel;
            bool isCurrent = stepIndex == userLevel;

            System.Action clickAction = () =>
            {
                Debug.Log($"Step button {stepIndex + 1} clicked! isUnlocked: {isUnlocked}");

                // Only allow starting from unlocked steps
                if (isUnlocked)
                {
                    Debug.Log($"Starting game from step {stepIndex + 1}");
                    melodyManager.SetCurrentSongFromMap(songName, steps, stepIndex);
                    gameObject.SetActive(false); 
                }
            };
            stepButton.Setup(stepIndex + 1, isUnlocked, isCurrent, clickAction);
        }
    }
}

