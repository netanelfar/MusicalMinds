using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// Singleton AudioManager responsible for playing note audio clips from the Resources folder.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioSource audioSource;
    public AudioClip[] noteClips;

    /// Ensures only one instance of AudioManager exists (Singleton logic).
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


    }
    /// Plays a note audio clip by its name. The clip must be located in the "Resources/Notes" folder.
    /// param name="clipName" Name of the clip without extension (e.g., "C4", "G#3")
    public void PlayNote(string clipName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Notes/" + clipName);
        if (clip == null)
        {
            Debug.LogError("Clip not found: " + clipName);
            return;
        }
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned.");
            return;
        }
        audioSource.PlayOneShot(clip);
    }
}