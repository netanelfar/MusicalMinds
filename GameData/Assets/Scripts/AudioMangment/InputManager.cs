using UnityEngine;
using MidiJack;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;


/// Handles MIDI keyboard input and routes note presses to the game logic (KeyControl).
/// Implements a singleton pattern to ensure a single persistent instance.
public class InputManager : MonoBehaviour
{
    private Dictionary<int, string> midiNoteMap;

    public static InputManager Instance;

    private int[] supportedMidiNotes = {
    // Octave 3 (C3–B3)
    48, 50, 52, 53, 55, 57, 59,     // White
    49, 51, 54, 56, 58,             // Black

    // Octave 4 (C4–B4)
    60, 62, 64, 65, 67, 69, 71,     // White
    61, 63, 66, 68, 70,             // Black

    // Octave 5 (C5–B5)
    72, 74, 76, 77, 79, 81, 83,     // White
    73, 75, 78, 80, 82              // Black
};


    /// Initialize the singleton instance and ensure persistence across scenes.
    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            Debug.Log("InputManager created and marked as persistent");
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Duplicate InputManager destroyed");
            Destroy(gameObject);
        }
    }


    /// Polls MIDI input every frame and forwards note presses to KeyControl.

    private void Update()
    {
        foreach (int midiNote in supportedMidiNotes)
        {
            if (MidiMaster.GetKeyDown(midiNote))
            {
                string noteNumber = midiNote.ToString();

                var keyControl = FindObjectOfType<KeyControl>();
                if (keyControl != null)
                {
                    keyControl.Press(noteNumber);
                }

                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
        }


        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

}