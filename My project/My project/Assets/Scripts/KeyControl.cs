using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyControl : MonoBehaviour
{
    public static bool inputAllowed = true;

    // Colors
    private Color correctColor = new Color(0.3f, 1f, 0.4f, 1f); // green
    private Color wrongColor = new Color(1f, 0.3f, 0.3f, 1f);    // red
    private Color systemColor = new Color(0.6f, 0.6f, 1f, 1f);   // blue

    private Dictionary<string, Coroutine> activeCoroutines = new Dictionary<string, Coroutine>();

    /// <summary>
    /// Get the GameObject for the given note name
    /// </summary>
    public GameObject GetKeyObject(string note)
    {
        return GameObject.Find(note);
    }

    /// <summary>
    /// Called when player presses a key
    /// </summary>
    public void Press(string note)
    {
        if (!inputAllowed) return;

        GameObject keyObj = GetKeyObject(note);
        if (keyObj == null)
        {
            Debug.LogWarning($"No GameObject found for note: {note}");
            return;
        }

        AudioManager.Instance.PlayNote(note);
        Image image = keyObj.GetComponent<Image>();

        bool isCorrect = false;

        if (GameSettings.CurrentGameMode == GameSettings.GameMode.SingleNoteRecognition)
        {
            var recognizer = FindObjectOfType<SingleNoteRecognitionManager>();
            if (recognizer != null)
                isCorrect = recognizer.OnPlayerPressedNote(note);
        }

        Color flashColor = isCorrect ? correctColor : wrongColor;

        if (image != null)
        {
            if (activeCoroutines.ContainsKey(note) && activeCoroutines[note] != null)
            {
                StopCoroutine(activeCoroutines[note]);
            }

            activeCoroutines[note] = StartCoroutine(FlashAndRestore(image, note, 0.2f, flashColor));
        }
    }

    /// <summary>
    /// Called when system plays a note
    /// </summary>
    public void PressAsSystem(string note)
    {
        GameObject keyObj = GetKeyObject(note);
        if (keyObj == null)
        {
            Debug.LogWarning($"No GameObject found for note: {note}");
            return;
        }

        AudioManager.Instance.PlayNote(note);
        Image image = keyObj.GetComponent<Image>();

        if (image != null)
        {
            if (activeCoroutines.ContainsKey(note) && activeCoroutines[note] != null)
            {
                StopCoroutine(activeCoroutines[note]);
            }

            activeCoroutines[note] = StartCoroutine(FlashAndRestore(image, note, 0.2f, systemColor));
        }
    }

    /// <summary>
    /// Flash the key a given color and then return to default
    /// </summary>
    private IEnumerator FlashAndRestore(Image image, string note, float duration, Color flashColor)
    {
        image.color = flashColor;
        yield return new WaitForSeconds(duration);

        Color defaultColor = note.Contains("s") ? Color.black : Color.white;
        image.color = defaultColor;

        activeCoroutines[note] = null;
    }

    /// <summary>
    /// Visually shake the key (used for wrong presses)
    /// </summary>
    public void ShakeKey(string note, float duration = 0.3f, float distance = 10f)
    {
        GameObject keyObj = GetKeyObject(note);
        if (keyObj != null)
        {
            StartCoroutine(ShakeKeyCoroutine(keyObj, duration, distance));
        }
    }

    /// <summary>
    /// Coroutine to shake key side to side
    /// </summary>
    private IEnumerator ShakeKeyCoroutine(GameObject keyObj, float duration, float distance)
    {
        Vector3 originalPos = keyObj.transform.localPosition;
        float elapsed = 0f;
        float speed = 40f;

        while (elapsed < duration)
        {
            float xOffset = Mathf.Sin(elapsed * speed) * distance;
            keyObj.transform.localPosition = originalPos + new Vector3(xOffset, 0, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        keyObj.transform.localPosition = originalPos;
    }
}
