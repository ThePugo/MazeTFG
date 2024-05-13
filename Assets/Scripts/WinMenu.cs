using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class WinMenu : MonoBehaviour
{
    public TMP_Text timeText;
    public static int mazeWidth;
    public static int mazeHeight;

    private string formUrl = "https://docs.google.com/forms/u/0/d/e/1FAIpQLScBXpXxOZlK6rdQldgH4FEJdNksn-qDBZtM5M3iOsleE3ahHQ/formResponse";

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "VictoryScreen")
        {
            timeText.text = "Escape time: " + GameTimer.instance.elapsedTime.ToString("F2") + " seconds";
            StartCoroutine(Post(GameTimer.instance.elapsedTime.ToString("F2").ToString()));
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

    private IEnumerator Post(string time)
    {
        string pc = System.Environment.MachineName.ToString();
        string alg = MazeGenerator.SelectedAlgorithm.ToString();
        string size = "("+mazeWidth + ", " + mazeHeight+")";

        WWWForm form = new WWWForm();
        form.AddField("entry.395578895", pc);
        form.AddField("entry.1966906101", alg);
        form.AddField("entry.559258213", time);
        form.AddField("entry.1934648074", size);

        using (UnityWebRequest www = UnityWebRequest.Post(formUrl, form))
        {
            yield return www.SendWebRequest();
        }
    }
}
