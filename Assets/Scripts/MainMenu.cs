using TMPro;
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
        LoadMaze();
    }

    public void OnRecursiveBacktrackingButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.RecursiveBacktracking;
        LoadMaze();
    }
    public void OnKruskalButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.Kruskal;
        LoadMaze();
    }

    public void OnPrimButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.Prim;
        LoadMaze();
    }

    public void OnAldousBroderButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.AldousBroder;
        LoadMaze();
    }

    public void OnGrowingTreeButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.GrowingTree;
        LoadMaze();
    }
    public void OnHuntAndKillButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.HuntAndKill;
        LoadMaze();
    }

    public void OnWilsonButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.Wilson;
        LoadMaze();
    }

    public void OnRecursiveDivisionButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.RecursiveDivision;
        LoadMaze();
    }

    public void OnSidewinderButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.Sidewinder;
        LoadMaze();
    }

    public void OnEllerButtonPressed()
    {
        MazeGenerator.SelectedAlgorithm = MazeGenerator.MazeAlgorithm.Eller;
        LoadMaze();
    }

    private void LoadMaze()
    {
        GameTimer.instance.StartTimer();
        SceneManager.LoadScene("escena");
    }
}
