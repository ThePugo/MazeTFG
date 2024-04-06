using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
