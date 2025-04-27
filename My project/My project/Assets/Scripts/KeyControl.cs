using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyControl : MonoBehaviour
{
    // Dictionary to save currently pressed keys
    private Dictionary<string, Coroutine> activeCoroutines = new Dictionary<string, Coroutine>();
    Color customGreen = new Color(0.3f, 1f, 0.4f, 1f); // green
    Color systemColor = new Color(0.6f, 0.6f, 1f, 1f); // light blue for system play





    public void Press(string note)
    {
        AudioManager.Instance.PlayNote(note);
        GameObject keyObj = GameObject.Find(note);
        if (keyObj != null)
        {
            var image = keyObj.GetComponent<Image>();
            if (image != null)
            {
                // Stop active color for this key
                if (activeCoroutines.ContainsKey(note) && activeCoroutines[note] != null)
                {
                    StopCoroutine(activeCoroutines[note]);
                }

                // Start new color for this key
                activeCoroutines[note] = StartCoroutine(FlashAndRestore(image, note, 0.2f, customGreen));
            }
        }
        else
        {
            Debug.LogWarning($"[KeyControl] No GameObject found with name: {note}");
        }

        if (GameSettings.CurrentGameMode == GameSettings.GameMode.SingleNoteRecognition)
        {
            var recognizer = FindObjectOfType<SingleNoteRecognitionManager>();
            if (recognizer != null)
            {
                recognizer.OnPlayerPressedNote(note);
            }
        }

    }

    public void PressAsSystem(string note)
    {
        AudioManager.Instance.PlayNote(note);
        GameObject keyObj = GameObject.Find(note);
        if (keyObj != null)
        {
            var image = keyObj.GetComponent<Image>();
            if (image != null)
            {
                if (activeCoroutines.ContainsKey(note) && activeCoroutines[note] != null)
                {
                    StopCoroutine(activeCoroutines[note]);
                }

                activeCoroutines[note] = StartCoroutine(FlashAndRestore(image, note, 0.2f, systemColor));
            }
        }
        else
        {
            Debug.LogWarning($"[KeyControl] No GameObject found with name: {note}");
        }
    }

    private IEnumerator FlashAndRestore(Image image, string note, float duration, Color flashColor)
    {
        image.color = flashColor;
        yield return new WaitForSeconds(duration);

        Color defaultColor = note.Contains("s") ? Color.black : Color.white;
        image.color = defaultColor;
        activeCoroutines[note] = null;
    }
}