using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SingleNoteRecognitionManager : MonoBehaviour
{
    public KeyControl keyControl; // Assign in inspector
    public TextMeshProUGUI feedbackText; // Assign in inspector (optional)

    private string targetNote;
    private bool waitingForInput = false;
    private bool isSystemPlayingNote = false;
    private int wrongAttempts = 0;
    private float noteStartTime = 0f;

    private List<string> allNotes = new List<string>
    {
        "Low C", "Low Cs", "Low D", "Low Ds", "Low E", "Low F", "Low Fs", "Low G", "Low Gs", "Low A", "Low As", "Low B",
        "Up C", "Up Cs", "Up D", "Up Ds", "Up E", "Up F", "Up Fs", "Up G", "Up Gs", "Up A", "Up As", "Up B"
    };

    void Start()
    {
        GameSettings.CurrentGameMode = GameSettings.GameMode.SingleNoteRecognition;
        Invoke(nameof(PickRandomNote), 1f); // Delay first note slightly
        if (UserManager.CurrentUser != null)//debug temp
        {

            Debug.Log("Enterd Sound Match");
            Debug.Log("Username: " + UserManager.CurrentUser.username);
            Debug.Log("Level: " + UserManager.CurrentUser.level);
            Debug.Log("Free Play Counter: " + UserManager.CurrentUser.freePlayCounter);
            Debug.Log("Note Recognition Counter: " + UserManager.CurrentUser.noteRecognitionCounter);
            Debug.Log("Points: " + UserManager.CurrentUser.points);
        }
    }

    public void PickRandomNote()
    {
        int randomIndex = Random.Range(0, allNotes.Count);
        targetNote = allNotes[randomIndex];
        Debug.Log($"Target Note: {targetNote}");

        wrongAttempts = 0;
        noteStartTime = Time.time;

        isSystemPlayingNote = true;
        keyControl.PressAsSystem(targetNote);
        Invoke(nameof(EnablePlayerInput), 0.6f);
    }

    public void OnPlayerPressedNote(string note)
    {
        Debug.Log($"Player pressed note: {note}");

        if (!waitingForInput || isSystemPlayingNote) return;

        if (note == targetNote)
        {
            Debug.Log("Correct!");
            ShowFeedback("Correct!", Color.green);

            CalculatePointsAddition();



            waitingForInput = false;
            Invoke(nameof(PickRandomNote), 1.2f);
        }
        else
        {
            Debug.Log($"Wrong! You pressed {note}, expected {targetNote}");
            ShowFeedback("Try again!", Color.yellow);

            wrongAttempts++;

           

            StartCoroutine(ReplayTargetNote());
        }
    }

    public void CalculatePointsAddition()
    {
        float timeTaken = Time.time - noteStartTime;

        if (wrongAttempts == 0)
        {
            if (timeTaken <= 2f)
                UserManager.CurrentUser.points += 15; // correct in less then 2 sec +15
            else
                UserManager.CurrentUser.points += 10; // Correct +10
        }
        else if (wrongAttempts == 1)
        {
            UserManager.CurrentUser.points += 5; // One wrong attempt +5
        }
        else
        {
            UserManager.CurrentUser.points += 2; // Multiple wrong attempts +2
        }
        UserEditorService.SaveUserDitalesAfterGame(); 

    }

    private IEnumerator ReplayTargetNote()
    {
        isSystemPlayingNote = true;
        yield return new WaitForSeconds(0.9f); // Wait before replaying the note.

        keyControl.PressAsSystem(targetNote);
        yield return new WaitForSeconds(0.6f); //Wait 0.6 seconds after replaying the note,

        EnablePlayerInput();
    }

    void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;

        feedbackText.text = message;
        feedbackText.color = color;

        CancelInvoke(nameof(ClearFeedback));
        Invoke(nameof(ClearFeedback), 2f); // Show feedback for 2 seconds
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

    /*public void SaveProgress()
    {
        UserEditorService.SaveUserDitalesAfterGame();
        Debug.Log("Progress saved.");
    }*/
}
