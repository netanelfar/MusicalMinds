using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SingleNoteRecognitionManager : MonoBehaviour
{
    public KeyControl keyControl; // Assign your existing KeyControl script in the inspector
    private string targetNote;
    private bool waitingForInput = false;
    public TextMeshProUGUI feedbackText;
    private bool isSystemPlayingNote = false;



    private List<string> allNotes = new List<string>
    {
        "Low C", "Low Cs", "Low D", "Low Ds", "Low E", "Low F", "Low Fs", "Low G", "Low Gs", "Low A", "Low As", "Low B",
        "Up C", "Up Cs", "Up D", "Up Ds", "Up E", "Up F", "Up Fs", "Up G", "Up Gs", "Up A", "Up As", "Up B"
    };

    void Start()
    {
        GameSettings.CurrentGameMode = GameSettings.GameMode.SingleNoteRecognition; // Set mode here
        PickRandomNote();
    }


    public void PickRandomNote()
    {
        int randomIndex = Random.Range(0, allNotes.Count);
        targetNote = allNotes[randomIndex];
        Debug.Log($" Target Note: {targetNote}");

        isSystemPlayingNote = true;
        keyControl.Press(targetNote); // system plays the target note
        Invoke(nameof(EnablePlayerInput), 0.6f); // delay so player can't instantly answer
    }

    public void OnPlayerPressedNote(string note)
    {
        Debug.Log($"Player pressed note: {note}");

        if (!waitingForInput || isSystemPlayingNote) return;

        if (note == targetNote)
        {
            Debug.Log("Correct!");
            ShowFeedback("Correct!", Color.green);
        }
        else
        {
            Debug.Log($"Wrong! You pressed {note}, expected {targetNote}");
            ShowFeedback("Wrong!", Color.red);
        }

        waitingForInput = false;
        Invoke(nameof(PickRandomNote), 1.2f);
    }






    void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;

        feedbackText.text = message;
        feedbackText.color = color;

        CancelInvoke(nameof(ClearFeedback));
        Invoke(nameof(ClearFeedback), 1f);
    }

    void ClearFeedback()
    {
        if (feedbackText != null)
            feedbackText.text = "";
    }

    void EnablePlayerInput()
    {
        isSystemPlayingNote = false;
        waitingForInput = true;
    }


}
