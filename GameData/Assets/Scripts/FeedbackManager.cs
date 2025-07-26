using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    public TextMeshProUGUI feedbackText;
    private Coroutine clearRoutine;

    // Displays feedback message with specified color and auto-clear duration
    public void ShowFeedback(string message, Color color, float duration = 2f)
    {
        if (clearRoutine != null) /// Cancel previous auto-clear
            StopCoroutine(clearRoutine);

        feedbackText.text = message;
        feedbackText.color = color;
        clearRoutine = StartCoroutine(ClearFeedbackAfterDelay(duration));
    }

    // Clears feedback after delay.
    private IEnumerator ClearFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClearFeedback();
    }

    // Clears the feedback text.
    public void ClearFeedback()
    {
        if (feedbackText != null)
            feedbackText.text = "";
    }

    // Stops pending auto-clear timers.
    public void CancelPendingClear()
    {
        if (clearRoutine != null)
        {
            StopCoroutine(clearRoutine);
            clearRoutine = null;
        }
    }

    // Returns random motivational message based on success ratio.
    public string GetEndOfLevelMessage(int notesCorrect, int notesPerLevel)
    {
        float successRatio = (float)notesCorrect / notesPerLevel;

        List<string> lowSuccessMessages = new List<string>
        {
            "Nice try!", "Good practice!", "You're learning!", "Keep going!", "Don't give up!"
        };

        List<string> mediumSuccessMessages = new List<string>
        {
            "Well done!", "You're getting better!", "Keep it up!", "You're doing great!", "Almost there!"
        };

        List<string> highSuccessMessages = new List<string>
        {
            "Amazing!", "Fantastic job!", "Super star!", "You nailed it!", "Perfect!"
        };

        if (successRatio <= 1f / 3f)
            return lowSuccessMessages[Random.Range(0, lowSuccessMessages.Count)];
        else if (successRatio <= 2f / 3f)
            return mediumSuccessMessages[Random.Range(0, mediumSuccessMessages.Count)];
        else
            return highSuccessMessages[Random.Range(0, highSuccessMessages.Count)];
    }

}
