using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyControl : MonoBehaviour
{
    // Dictionary to save currently pressed keys
    private Dictionary<string, Coroutine> activeCoroutines = new Dictionary<string, Coroutine>();
    Color customGreen = new Color(0.3f, 1f, 0.4f, 1f); // green


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
                activeCoroutines[note] = StartCoroutine(FlashAndRestore(image, note, 0.2f));
            }
        }
        else
        {
            Debug.LogWarning($"[KeyControl] No GameObject found with name: {note}");
        }
    }

    private IEnumerator FlashAndRestore(Image image, string note, float duration)
    {
        image.color = customGreen; // Color pressed key
        yield return new WaitForSeconds(duration);

        // Determine default color based on key name
        Color defaultColor = note.Contains("s") ? Color.black : Color.white;

        image.color = defaultColor; // restore to default color
        activeCoroutines[note] = null; // Clear the reference
    }
}