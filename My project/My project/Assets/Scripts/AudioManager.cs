using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioSource audioSource;
    public AudioClip[] noteClips;


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