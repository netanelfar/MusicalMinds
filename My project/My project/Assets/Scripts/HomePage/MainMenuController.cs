using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void LoadFreePlay()
    {
        SceneManager.LoadScene("freePlay");
    }

    public void LoadNoteRecognition()
    {
        SceneManager.LoadScene("NoteRecognition");
    }

    
}
