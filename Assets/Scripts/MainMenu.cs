using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{

    public TMP_Dropdown widthDropdown;
    public TMP_Dropdown heightDropdown;
    public int minWidth = 5;
    public int maxWidth = 100;

    void Start()
    {
        SetupDropdown(widthDropdown);
        SetupDropdown(heightDropdown);
    }

    void SetupDropdown(TMP_Dropdown dropdown)
    {
        dropdown.options.Clear();
        for (int i = minWidth; i <= maxWidth; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
        }
        dropdown.value = 0;

        // Actualiza el valor mostrado en la interfaz de usuario
        dropdown.RefreshShownValue();
        SendValues();
    }

    public void SendValues()
    {
        if (widthDropdown.options.Count > 0 && heightDropdown.options.Count > 0) 
        {
            PlayerPrefs.SetInt("MazeWidth", int.Parse(widthDropdown.options[widthDropdown.value].text));
            PlayerPrefs.SetInt("MazeHeight", int.Parse(heightDropdown.options[heightDropdown.value].text));
            // No olvides guardar las preferencias
            PlayerPrefs.Save();
        }
    }
    public void OnBinaryTreeButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.BinaryTree;
        SceneManager.LoadScene("escena");
    }

    public void OnRecursiveBacktrackingButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.RecursiveBacktracking;
        SceneManager.LoadScene("escena");
    }
    public void OnKruskalButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.Kruskal;
        SceneManager.LoadScene("escena");
    }

    public void OnPrimButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.Prim;
        SceneManager.LoadScene("escena");
    }

    public void OnAldousBroderButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.AldousBroder;
        SceneManager.LoadScene("escena");
    }

    public void OnGrowingTreeButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.GrowingTree;
        SceneManager.LoadScene("escena");
    }
    public void OnHuntAndKillButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.HuntAndKill;
        SceneManager.LoadScene("escena");
    }

    public void OnWilsonButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.Wilson;
        SceneManager.LoadScene("escena");
    }

    public void OnRecursiveDivisionButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.RecursiveDivision;
        SceneManager.LoadScene("escena");
    }

    public void OnSidewinderButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.Sidewinder;
        SceneManager.LoadScene("escena");
    }

    public void OnEllerButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.Eller;
        SceneManager.LoadScene("escena");
    }
}
