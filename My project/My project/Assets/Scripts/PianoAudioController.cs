using UnityEngine;

public class PianoAudioController : MonoBehaviour
{
    [Header("Existing GameObjects (can be inactive at start)")]
    public GameObject keyControlObject;
    public GameObject inputManagerObject;
    public GameObject audioManagerObject;

    private void Awake()
    {
        // Activate KeyControl if not already active
        if (keyControlObject != null && !keyControlObject.activeInHierarchy)
        {
            keyControlObject.SetActive(true);
        }

        // Activate InputManager if not already active
        if (inputManagerObject != null && !inputManagerObject.activeInHierarchy)
        {
            inputManagerObject.SetActive(true);
        }

        // Activate AudioManager if not already active
        if (audioManagerObject != null && !audioManagerObject.activeInHierarchy)
        {
            audioManagerObject.SetActive(true);
        }
    }
}
