using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Reloads home page.
public class HomePageControllerBTN : MonoBehaviour
{
    public void LoadHomePage()
    {
        SceneManager.LoadScene("HomePageScene");
        UserManager.SaveUserDetailsAfterGame();
    }

}
