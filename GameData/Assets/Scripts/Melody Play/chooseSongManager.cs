using System.Collections.Generic;
using UnityEngine;

// Manages song selection UI.
public class ChooseSongManager : MonoBehaviour
{
    public GameObject chooseSongPanel;
    public MelodyPlayManager soundMatchManager;
    public ProgressBar progressBarJonathan;
    public ProgressBar progressBarStar;
    public ProgressBar progressBarSymphony;
    public GameObject stepMapPanel;
    public StepMapGenerator stepMapGenerator;

    // Opens song map for the selected song.
    public void ChooseSongByName(string songName)
    {
        List<List<string>> SongNotes = GetSongSteps(songName);
        if (SongNotes != null)
        {
            chooseSongPanel.SetActive(false);
            stepMapPanel.SetActive(true);
            stepMapGenerator.GenerateMap(songName, SongNotes);
        }
        else
        {
            Debug.LogWarning("Song not found: " + songName);
        }
    }

    // Returns note sequences for each step in the song.
    public List<List<string>> GetSongSteps(string songName)
    {
        switch (songName)
        {
            case "Little Jonathan":
                return new List<List<string>>
                {
                    new List<string> { "67", "64", "64" },
                    new List<string> { "65", "62", "62" },
                    new List<string> { "60", "62", "64" },
                    new List<string> { "65", "67", "67" },
                    new List<string> { "67", "67", "64" },
                    new List<string> { "64", "65", "62" },
                    new List<string> { "62", "60", "64" },
                    new List<string> { "67", "67", "60" },
                    new List<string> { "62", "62", "62" },
                    new List<string> { "62", "62", "64" },
                    new List<string> { "65", "64", "64" },
                    new List<string> { "64", "64", "64" },
                    new List<string> { "65", "67", "67" },
                    new List<string> { "64", "64", "65" },
                    new List<string> { "62", "62", "60" },
                    new List<string> { "64", "67", "67" },
                    new List<string> { "60" }
                };

            case "Little Star":
                return new List<List<string>>
                {
                    new List<string> { "55", "55", "62" },
                    new List<string> { "62", "64", "64" },
                    new List<string> { "62", "60", "60" },
                    new List<string> { "59", "59", "57" },
                    new List<string> { "55", "62", "62" },
                    new List<string> { "60", "60", "59" },
                    new List<string> { "59", "57", "62" },
                    new List<string> { "62", "60", "60" },
                    new List<string> { "59", "59", "57" },
                    new List<string> { "55", "55", "62" },
                    new List<string> { "62", "64", "64" },
                    new List<string> { "62", "60", "60" },
                    new List<string> { "59", "59", "57" },
                    new List<string> { "57", "55", "62" },
                    new List<string> { "62", "60", "60" },
                    new List<string> { "59", "59", "57" },
                    new List<string> { "62", "62", "60" },
                    new List<string> { "60", "59", "57" },
                    new List<string> { "55", "55", "62" },
                    new List<string> { "62", "64", "64" },
                    new List<string> { "62", "60", "60" },
                    new List<string> { "59", "59", "57" },
                    new List<string> { "57", "55" }
                };

            case "Beethoven’s 9th":
                return new List<List<string>>
                {
                    new List<string> { "64", "64", "65" },
                    new List<string> { "67", "67", "65" },
                    new List<string> { "64", "62", "60" },
                    new List<string> { "60", "62", "64" },
                    new List<string> { "64", "62", "62" },
                    new List<string> { "64", "64", "65" },
                    new List<string> { "67", "67", "65" },
                    new List<string> { "64", "62", "60" },
                    new List<string> { "60", "62", "64" },
                    new List<string> { "62", "60", "60" },
                    new List<string> { "62", "62", "64" },
                    new List<string> { "60", "62", "64" },
                    new List<string> { "65", "64", "60" },
                    new List<string> { "62", "64", "65" },
                    new List<string> { "64", "62", "60" },
                    new List<string> { "62", "55", "64" },
                    new List<string> { "64", "65", "67" },
                    new List<string> { "67", "65", "64" },
                    new List<string> { "62", "60", "60" },
                    new List<string> { "62", "64", "62" },
                    new List<string> { "60", "60" }
                };

            default:
                return null;
        }
    }

    // Updates progress bars.
    
    void OnEnable()
    {
        UpdateAllProgressBars();
    }

    // Refreshes all song progress bars.
    void UpdateAllProgressBars()
    {
        progressBarJonathan.UpdateProgress(
            UserManager.GetSongLevel("Little Jonathan"),
            GetSongSteps("Little Jonathan")?.Count ?? 0
        );

        progressBarStar.UpdateProgress(
            UserManager.GetSongLevel("Little Star"),
            GetSongSteps("Little Star")?.Count ?? 0
        );

        progressBarSymphony.UpdateProgress(
            UserManager.GetSongLevel("Beethoven’s 9th"),
            GetSongSteps("Beethoven’s 9th")?.Count ?? 0
        );
    }

    // Returns to song selection.
    public void ReturnToChooseSongPanel()
    {
        if (stepMapPanel != null)
            stepMapPanel.SetActive(false);

        if (chooseSongPanel != null)
            chooseSongPanel.SetActive(true);

        UpdateAllProgressBars(); 
    }
}

