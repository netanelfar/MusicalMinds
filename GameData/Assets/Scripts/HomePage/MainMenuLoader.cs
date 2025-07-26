using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Handles navigation to different game modes for a connectd user.
public class MainMenuLoader : MonoBehaviour
{
    public TextMeshProUGUI noUserWarningText;

    void Start()
    {
        noUserWarningText.gameObject.SetActive(UserManager.CurrentUser == null);

        // Subscribe to user change updates
        UserManager.OnCurrentUserChanged += OnUserChanged;
    }

    private void OnUserChanged(UserProfile user)
    {
        noUserWarningText.gameObject.SetActive(user == null);
    }

    void OnDestroy()
    {
        UserManager.OnCurrentUserChanged -= OnUserChanged;
    }



    // Check for connected user.
    private bool CheckUserExists()
    {
        if (UserManager.CurrentUser == null)
        {
            Debug.LogWarning("No user selected");
            return false;
        }
        return true;
    }

    // Load free play mode.
    public void LoadFreePlay()
    {
        if (!CheckUserExists()) return;
        GameSettings.CurrentGameMode = GameSettings.GameMode.FreePlay;
        UserManager.UpdatePlayModeCounter();
        SceneManager.LoadScene("freePlay");
    }

    // Load Note Recognition/ Sound Match mode.
    public void LoadNoteRecognition()
    {
        if (!CheckUserExists()) return;
        GameSettings.CurrentGameMode = GameSettings.GameMode.SingleNoteRecognition;
        UserManager.UpdatePlayModeCounter();
        SceneManager.LoadScene("NoteRecognition");
    }

    // Load Melody play mode.
    public void LoadMelodyPlay()
    {
        if (!CheckUserExists()) return;
        GameSettings.CurrentGameMode = GameSettings.GameMode.MelodyPlay;
        UserManager.UpdatePlayModeCounter();
        SceneManager.LoadScene("MelodyPlay");
    }
}
