using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLoader : MonoBehaviour
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
