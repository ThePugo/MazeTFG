using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject player;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (pauseMenu.activeSelf)
        {
            Resume();
        }
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            player.GetComponent<FirstPersonController>().enabled = false;
            AudioListener.pause = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        player.GetComponent<FirstPersonController>().enabled = true;
        AudioListener.pause = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        player.GetComponent<FirstPersonController>().enabled = true;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
