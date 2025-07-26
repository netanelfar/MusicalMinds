using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Manages speaker volume control UI.
public class SpeakerVolume : MonoBehaviour
{
    public Button speakerButton;
    public GameObject volumePanel; // Container holding the slider
    public Slider volumeSlider;
    public GameObject muteLine;          
    private bool isMuted = false;
    private Coroutine hideCoroutine;

    // Initialize volume settings from current user.
    void Start()
    {
        if (UserManager.CurrentUser != null)
        {
            AudioListener.volume = UserManager.CurrentUser.volume;
            volumeSlider.value = UserManager.CurrentUser.volume;
            Debug.Log(" speaker valume script: CurrentUser.preferredPianoSize " + UserManager.CurrentUser.preferredPianoSize);         
        }
        volumePanel.SetActive(false);
        UpdateMuteVisual(Mathf.Approximately(AudioListener.volume, 0f));
    }

    // Toggles volume panel visibility and auto-hide timer.
    public void OnSpeakerClick()
    {
        bool isCurrentlyVisible = volumePanel.activeSelf;

        if (isCurrentlyVisible)
        {
            volumePanel.SetActive(false);
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
                hideCoroutine = null;
            }
        }
        else
        {
            volumePanel.SetActive(true);
            if (hideCoroutine != null) StopCoroutine(hideCoroutine);
            hideCoroutine = StartCoroutine(HidePanelAfterDelay());
        }
    }

    // Updates audio volume, saves to user profile.
    public void VolumeChanged(float value)
    {
        AudioListener.volume = value;
        if (UserManager.CurrentUser != null)
        {
            UserManager.CurrentUser.volume = value; 
            UserManager.SaveCurrentUserToFile();
        }

        isMuted = Mathf.Approximately(value, 0f);
        UpdateMuteVisual(isMuted);

        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HidePanelAfterDelay());
    }

    // Waits 2 seconds then hides the volume panel.
    IEnumerator HidePanelAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        volumePanel.SetActive(false);
    }

    // Shows or hides the mute line UI.
    void UpdateMuteVisual(bool showMuteLine)
    {
        if (muteLine != null)
            muteLine.SetActive(showMuteLine);
    }
}
