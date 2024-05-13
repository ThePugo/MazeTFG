using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

public class MazeRender : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] GameObject mazeCellPrefab;
    [SerializeField] GameObject doorPrefab;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject exitWall;
    [SerializeField] GameObject key;

    private MazeCellObject topLeftCell;
    private MazeCellObject topRightCell;
    private MazeCellObject bottomRightCell;

    public float CellSize = 4f;

    private void Start()
    {
        Cursor.visible = false;
        MazeCell[,] maze = mazeGenerator.GetMaze();
        WinMenu.mazeHeight = mazeGenerator.mazeHeight;
        WinMenu.mazeWidth = mazeGenerator.mazeWidth;
        System.Random random = new System.Random();
        int randomi = random.Next(mazeGenerator.mazeWidth);
        int randomj = random.Next(mazeGenerator.mazeHeight);

        for (int i = 0;  i < mazeGenerator.mazeWidth; i++)
        {
            for (int j = 0; j < mazeGenerator.mazeHeight; j++)
            {
                GameObject newCell = Instantiate(mazeCellPrefab, new Vector3((float)i * CellSize, 0f, (float)j * CellSize), Quaternion.identity, transform);
                MazeCellObject mazeCell = newCell.GetComponent<MazeCellObject>();

                bool top = maze[i, j].topWall;
                bool left = maze[i, j].leftWall;

                bool right = false;
                bool bottom = false;
                
                if (i == mazeGenerator.mazeWidth - 1)
                {
                    right = true;
                }
                if (j == 0)
                {
                    bottom = true;
                }

                mazeCell.Init(top, bottom, right, left);

                // Asignar esquinas específicas basado en la posición
                if (i == 0 && j == mazeGenerator.mazeHeight - 1) // Esquina superior izquierda
                {
                    topLeftCell = mazeCell;
                }
                else if (i == mazeGenerator.mazeWidth - 1 && j == mazeGenerator.mazeHeight - 1) // Esquina superior derecha
                {
                    topRightCell = mazeCell;
                }
                else if (i == mazeGenerator.mazeWidth - 1 && j == 0) // Esquina inferior derecha
                {
                    bottomRightCell = mazeCell;
                }

                //Asignar llave
                if (i == randomi && j==randomj)
                {
                    Instantiate(key, new Vector3((float)i * CellSize, 0f, (float)j * CellSize), Quaternion.identity, transform);
                }
            }
        }
        NavMeshSurface surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
        Vector3[] patrolPoints = GenerateStrategicPatrolPoints();
        enemy.GetComponent<EnemyAI>().SetPatrolPoints(patrolPoints);
        PlaceExit();
    }

    private Vector3[] GenerateStrategicPatrolPoints()
    {
        List<Vector3> patrolPoints = new List<Vector3>
    {
        // Esquina superior izquierda
        new Vector3(0, 0, (mazeGenerator.mazeHeight - 1) * CellSize),

        // Esquina superior derecha
        new Vector3((mazeGenerator.mazeWidth - 1) * CellSize, 0, (mazeGenerator.mazeHeight - 1) * CellSize),

        // Esquina inferior derecha
        new Vector3((mazeGenerator.mazeWidth - 1) * CellSize, 0, 0),

        // Esquina inferior izquierda
        new Vector3(0, 0, 0),

        // Centro superior
        new Vector3((mazeGenerator.mazeWidth - 1) / 2f * CellSize, 0, (mazeGenerator.mazeHeight - 1) * CellSize),

        // Centro inferior
        new Vector3((mazeGenerator.mazeWidth - 1) / 2f * CellSize, 0, 0),
    };

        // Mezclar los puntos de patrulla
        Shuffle(patrolPoints);

        return patrolPoints.ToArray();
    }

    private void Shuffle(List<Vector3> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Vector3 value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void PlaceExit()
    {
        // Definir las cuatro esquinas del laberinto
        MazeCellObject[] corners = new MazeCellObject[]
        {
        topLeftCell,
        topRightCell,
        bottomRightCell,
        };

        // Seleccionar una esquina aleatoria
        MazeCellObject cellToReplace = corners[Random.Range(0, corners.Length)];
        Vector3 pos= new Vector3(0,0,0);

        // Crear una instancia de la clase Random
        System.Random random = new System.Random();

        // Obtener un número aleatorio que sea 0 o 1
        int randomNumber = random.Next(2); // Genera 0 o 1

        if (cellToReplace == topLeftCell)
        {
            pos = topLeftCell.transform.position;
            if (randomNumber == 0)
            {
                topLeftCell.disableTop();
                GameObject exit = Instantiate(exitWall, new Vector3(-0.46f, 0f, 2f), Quaternion.Euler(0,180,0));
                exit.transform.SetParent(topLeftCell.transform, false);
            }
            else
            {
                topLeftCell.disableLeft();
                GameObject exit = Instantiate(exitWall, new Vector3(-2f, 0f, -0.55f), Quaternion.Euler(0, 90, 0));
                exit.transform.SetParent(topLeftCell.transform, false);
            }
        }
        else if (cellToReplace == topRightCell)
        {
            pos = topRightCell.transform.position;
            if (randomNumber == 0)
            {
                topRightCell.disableTop();
                GameObject exit = Instantiate(exitWall, new Vector3(-0.55f, 0f, 2f), Quaternion.Euler(0, 180, 0));
                exit.transform.SetParent(topRightCell.transform, false);
            }
            else
            {
                topRightCell.disableRight();
                GameObject exit = Instantiate(exitWall, new Vector3(2f,0f,0.46f), Quaternion.Euler(0,-90,0));
                exit.transform.SetParent(topRightCell.transform, false);
            }
        }
        else if (cellToReplace == bottomRightCell)
        {
            pos = bottomRightCell.transform.position;
            if (randomNumber == 0)
            {
                bottomRightCell.disableBottom();
                GameObject exit = Instantiate(exitWall, new Vector3(0.46f, 0f, -2f), Quaternion.identity);
                exit.transform.SetParent(bottomRightCell.transform, false);
            }
            else
            {
                bottomRightCell.disableRight();
                GameObject exit = Instantiate(exitWall, new Vector3(2f, 0f, 0.55f), Quaternion.Euler(0, -90, 0));
                exit.transform.SetParent(bottomRightCell.transform, false);
            }
        }
        Instantiate(enemy, pos, Quaternion.identity);
    }



}
