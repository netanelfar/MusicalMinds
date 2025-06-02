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

    public void LoadTEMPscne()
    {
        SceneManager.LoadScene("TEMPgame");
    }

    public void LoadTEMPGamescne()
    {
        SceneManager.LoadScene("TEMPgame");
    }
}
