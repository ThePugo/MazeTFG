using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class WinMenu : MonoBehaviour
{
    public TMP_Text timeText;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "VictoryScreen")
        {
            timeText.text = "Escape time: " + GameTimer.instance.elapsedTime.ToString("F2") + " seconds";
        }
    }

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
