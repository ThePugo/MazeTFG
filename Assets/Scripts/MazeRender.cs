using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class MazeRender : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] GameObject mazeCellPrefab;
    [SerializeField] GameObject doorPrefab;
    [SerializeField] GameObject player;
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
        PlaceExit();
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

        // Crear una instancia de la clase Random
        System.Random random = new System.Random();

        // Obtener un número aleatorio que sea 0 o 1
        int randomNumber = random.Next(2); // Genera 0 o 1

        if (cellToReplace == topLeftCell)
        {
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
    }



}
