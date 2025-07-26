using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyControl : MonoBehaviour
{
    public Transform notesParent;
    public static bool inputAllowed = true;
    // Easy to add more octaves
    private HashSet<int> blackKeys = new HashSet<int>
{ 
    // Lower octave
    37, 39, 42, 44, 46,
    // Your current range  
    49, 51, 54, 56, 58, 61, 63, 66, 68, 70,
    // Higher octave
    73, 75, 78, 80, 82
};

    void Awake()
    {
        // Make this GameObject persist across scenes
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        for (int i = 60; i <= 92; i++)
        {
            string note = i.ToString();
            GameObject prefab = Resources.Load<GameObject>($"Notes/{note}");
            if (prefab != null)
            {
                GameObject key = Instantiate(prefab, notesParent);
                key.name = note;
            }
        }
    }
    // Colors
    private Color correctColor = new Color(0.3f, 1f, 0.4f, 1f); // green
    private Color wrongColor = new Color(1f, 0.3f, 0.3f, 1f);    // red
    private Color systemColor = new Color(0.6f, 0.6f, 1f, 1f);   // blue

    private Dictionary<string, Coroutine> activeCoroutines = new Dictionary<string, Coroutine>();


    /// Get the GameObject for the given note name
    public GameObject GetKeyObject(string note)
    {
        return GameObject.Find(note);
    }

    /// Called when player presses a key
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
        Color flashColor = wrongColor;//

        if (GameSettings.CurrentGameMode == GameSettings.GameMode.SingleNoteRecognition)
        {
            var recognizer = FindObjectOfType<SingleNoteRecognitionManager>();
            if (recognizer != null)
                isCorrect = recognizer.OnPlayerPressedNote(note);
        }
        else if (GameSettings.CurrentGameMode == GameSettings.GameMode.MelodyPlay)
        {
            var matcher = FindObjectOfType<MelodyPlayManager>();
            if (matcher != null)
                isCorrect = matcher.OnPlayerPressedNote(note);
        }
        else if (GameSettings.CurrentGameMode == GameSettings.GameMode.FreePlay)
        {
            isCorrect = true;
            flashColor = correctColor; // or systemColor if you want a different look
        }

        //Color flashColor = isCorrect ? correctColor : wrongColor;
        flashColor = isCorrect ? correctColor : wrongColor;

        if (image != null)
        {
            if (activeCoroutines.ContainsKey(note) && activeCoroutines[note] != null)
            {
                StopCoroutine(activeCoroutines[note]);
            }

            activeCoroutines[note] = StartCoroutine(FlashAndRestore(image, note, 0.2f, flashColor));
        }
        if (!isCorrect)
        {
            ShakeKey(note); // shake key on wrong press
        }

    }


    /// Called when system plays a note
    public void PressAsSystem(string note)
    {
        GameObject keyObj = GetKeyObject(note);
        if (keyObj == null)
        {
            Debug.LogWarning($"No GameObject found for note: {note}");
            return;
        }

        AudioManager.Instance.PlayNote(note);

        if (UserManager.CurrentUser?.systemPressHasColor == true)
        {
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

    }


    /// Flash the key a given color and then return to default
    private IEnumerator FlashAndRestore(Image image, string noteNumber, float duration, Color flashColor)
    {
        image.color = flashColor;
        yield return new WaitForSeconds(duration);

        // Determine if it's a black key based on MIDI note number
        if (int.TryParse(noteNumber, out int midiNote))
        {
            Color defaultColor = blackKeys.Contains(midiNote) ? Color.black : Color.white;
            image.color = defaultColor;
        }
        else
        {
            image.color = Color.white; // fallback for invalid note numbers
        }

        activeCoroutines[noteNumber] = null;
    }


    /// Visually shake the key (used for wrong presses)

    public void ShakeKey(string note, float duration = 0.3f, float distance = 10f)
    {
        GameObject keyObj = GetKeyObject(note);
        if (keyObj != null)
        {
            StartCoroutine(ShakeKeyCoroutine(keyObj, duration, distance));
        }
    }

    /// Coroutine to shake key side to side

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
