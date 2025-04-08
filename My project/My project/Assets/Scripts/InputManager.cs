using UnityEngine;
using MidiJack;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    private Dictionary<int, string> midiNoteMap;

    void Start()
    {
        midiNoteMap = new Dictionary<int, string>
    {
        { 48, "Low C" },
        { 49, "Low Cs" },
        { 50, "Low D" },
        { 51, "Low Ds" },
        { 52, "Low E" },
        { 53, "Low F" },
        { 54, "Low Fs" },
        { 55, "Low G" },
        { 56, "Low Gs" },
        { 57, "Low A" },
        { 58, "Low As" },
        { 59, "Low B" },

        { 60, "Up C" },
        { 61, "Up Cs" },
        { 62, "Up D" },
        { 63, "Up Ds" },
        { 64, "Up E" },
        { 65, "Up F" },
        { 66, "Up Fs" },
        { 67, "Up G" },
        { 68, "Up Gs" },
        { 69, "Up A" },
        { 70, "Up As" },
        { 71, "Up B" },
    };
    }


    void Update()
    {
        foreach (var entry in midiNoteMap)
        {
            int midiNote = entry.Key;
            string note = entry.Value;

            if (MidiMaster.GetKeyDown(midiNote))
            {
                GameObject buttonObj = GameObject.Find(note);
                if (buttonObj == null)
                {
                    Debug.LogError("Could not find key: " + note);
                    continue;
                }

                Button button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.Invoke(); // Press(note) function call
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
        }

    }
}
