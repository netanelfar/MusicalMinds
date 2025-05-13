using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteRecognitionPauseManager : MonoBehaviour
{

    public Sprite pauseIcon;
    public Sprite playIcon;
    public Image buttonImage;

    public static bool IsGamePaused { get; private set; } = false;

    private SingleNoteRecognitionManager noteManager;

    void Start()
    {
        if (noteManager == null)
            noteManager = FindObjectOfType<SingleNoteRecognitionManager>();
    }


    public void TogglePause()
    {
        IsGamePaused = !IsGamePaused;
        Time.timeScale = IsGamePaused ? 0f : 1f;

        if (IsGamePaused)
            noteManager?.PauseGame("PuseBTN");
        else
            noteManager?.ResumeGame("PuseBTN");

        if (buttonImage != null)
            buttonImage.sprite = IsGamePaused ? playIcon : pauseIcon;
    }



}
