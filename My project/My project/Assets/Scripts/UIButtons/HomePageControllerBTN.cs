using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomePageControllerBTN : MonoBehaviour
{
    public void LoadHomePage()
    {
        SceneManager.LoadScene("HomePageScene");
    }

    //add points and reward update
}
