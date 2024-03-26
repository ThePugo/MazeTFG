using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{

    // Update is called once per frame
    public void Quit()
    {
        Application.Quit();
    }

    public void Retry()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
