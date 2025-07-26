using UnityEngine;

/// Ensures that the core gameplay components (KeyControl, InputManager, AudioManager)
/// are active when the scene starts, even if they were initially inactive in the editor.
public class PianoAudioController : MonoBehaviour
{
    [Header("Existing GameObjects (can be inactive at start)")]
    public GameObject keyControlObject;
    public GameObject inputManagerObject;
    public GameObject audioManagerObject;

    /// Activates necessary components if they are assigned and currently inactive.
    private void ActivateIfInactive(GameObject obj)
    {
        if (obj != null && !obj.activeInHierarchy)
        {
            obj.SetActive(true);
        }
    }

    private void Awake()
    {
        ActivateIfInactive(keyControlObject);
        ActivateIfInactive(inputManagerObject);
        ActivateIfInactive(audioManagerObject);
    }

}
