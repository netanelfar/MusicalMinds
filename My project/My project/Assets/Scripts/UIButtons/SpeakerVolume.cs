using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpeakerVolume : MonoBehaviour
{
    public Button speakerButton;
    public GameObject volumePanel;       // Container holding the slider
    public Slider volumeSlider;
    public GameObject muteLine;          // The red line over the icon

    private bool isMuted = false;
    private Coroutine hideCoroutine;

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

    public void VolumeChanged(float value)
    {
        AudioListener.volume = value;

        if (UserManager.CurrentUser != null)
        {
            UserManager.CurrentUser.volume = value;

            var users = UserDataManager.LoadUsers();
            foreach (var user in users)
            {
                if (user.username == UserManager.CurrentUser.username)
                {
                    user.volume = value;
                    break;
                }
            }
            UserDataManager.SaveUsers(users);
        }

        isMuted = Mathf.Approximately(value, 0f);
        UpdateMuteVisual(isMuted);

        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HidePanelAfterDelay());
    }

    IEnumerator HidePanelAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        volumePanel.SetActive(false);
    }

    void UpdateMuteVisual(bool showMuteLine)
    {
        if (muteLine != null)
            muteLine.SetActive(showMuteLine);
    }
}
