using System;
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

    private MazeCell topLeftCellL;
    private MazeCell topRightCellL;
    private MazeCell bottomRightCellL;
    private MazeCell endCell;

    public float CellSize = 4f;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
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
                    topLeftCellL = maze[i, j];
                }
                else if (i == mazeGenerator.mazeWidth - 1 && j == mazeGenerator.mazeHeight - 1) // Esquina superior derecha
                {
                    topRightCell = mazeCell;
                    topRightCellL = maze[i, j];
                }
                else if (i == mazeGenerator.mazeWidth - 1 && j == 0) // Esquina inferior derecha
                {
                    bottomRightCell = mazeCell;
                    bottomRightCellL = maze[i, j];
                }

                //Asignar llave
                if (i == randomi && j==randomj)
                {
                    //Instantiate(key, new Vector3((float)i * CellSize, 0f, (float)j * CellSize), Quaternion.identity, transform);
                }
            }
        }
        NavMeshSurface surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
        Vector3[] patrolPoints = GenerateStrategicPatrolPoints();
        enemy.GetComponent<EnemyAI>().SetPatrolPoints(patrolPoints);
        PlaceExit();
        //PARA PRUEBAS:
        //AnalyzeMaze(maze);
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
        MazeCellObject cellToReplace = corners[UnityEngine.Random.Range(0, corners.Length)];
        if (cellToReplace == topLeftCell)
        {
            endCell = topLeftCellL;
        }
        else if (cellToReplace == bottomRightCell)
        {
            endCell = bottomRightCellL;
        }
        else
        {
            endCell = topRightCellL;
        }
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

    private void AnalyzeMaze(MazeCell[,] maze)
    {
        for (int i=0; i < mazeGenerator.mazeWidth; i++)
        {
            for (int j=0 ; j < mazeGenerator.mazeHeight; j++)
            {
                maze[i, j].visited = false;
            }
        }
        IdentifyDeadEnds(maze);
        IdentifyIntersections(maze);
        PrintMazeStatistics(maze);
    }

    private void IdentifyDeadEnds(MazeCell[,] maze)
    {
        int deadEndCount = 0;

        for (int x = 0; x < mazeGenerator.mazeWidth; x++)
        {
            for (int y = 0; y < mazeGenerator.mazeHeight; y++)
            {
                int wallsCount = 0;

                // Check top wall
                if (maze[x, y].topWall) wallsCount++;

                // Check left wall
                if (maze[x, y].leftWall) wallsCount++;

                // Check right wall
                if (x == mazeGenerator.mazeWidth - 1 || maze[x + 1, y].leftWall) wallsCount++;

                // Check bottom wall
                if (y == 0 || maze[x, y - 1].topWall) wallsCount++;

                if (wallsCount == 3)
                {
                    maze[x, y].isDeadEnd = true;
                    deadEndCount++;
                }
                else
                {
                    maze[x, y].isDeadEnd = false;
                }
            }
        }
    }

    private void IdentifyIntersections(MazeCell[,] maze)
    {
        for (int i = 0; i < mazeGenerator.mazeWidth; i++)
        {
            for (int j = 0; j < mazeGenerator.mazeHeight; j++)
            {
                int openWalls = 0;
                MazeCell cell = maze[i, j];
                if (i > 0 && !maze[i - 1, j].leftWall) openWalls++;
                if (j > 0 && !maze[i, j - 1].topWall) openWalls++;
                if (i < mazeGenerator.mazeWidth - 1 && !maze[i + 1, j].leftWall) openWalls++;
                if (j < mazeGenerator.mazeHeight - 1 && !maze[i, j + 1].topWall) openWalls++;

                if (openWalls > 2) cell.isThreeWayIntersection = true;
                if (openWalls > 3) cell.isFourWayIntersection = true;
            }
        }
    }
    List<Direction> directions = new List<Direction>() {
    Direction.Up, Direction.Down, Direction.Left, Direction.Right,
    };

    private float CalculateAverageCorridorLength(MazeCell[,] maze)
    {
        int totalLength = 0;
        int corridorCount = 0;

        for (int i = 0; i < mazeGenerator.mazeWidth; i++)
        {
            for (int j = 0; j < mazeGenerator.mazeHeight; j++)
            {

                    foreach (Direction direction in directions)
                    {
                        if (mazeGenerator.CanMove(i, j, direction))
                        {
                            int length = FollowCorridor(maze, i, j, direction);
                            if (length > 1)
                            {
                                totalLength += length;
                                corridorCount++;
                            }
                        }
                    }
                
            }
        }
        float averageLength = corridorCount > 0 ? (float)totalLength / corridorCount : 0;
        return averageLength;
    }

    private int FollowCorridor(MazeCell[,] maze, int x, int y, Direction direction)
    {
        int length = 1;

        while (true)
        {
            if (!mazeGenerator.CanMove(x, y, direction))
                break;

            Vector2Int delta = mazeGenerator.GetDirectionDelta(direction);
            x += delta.x;
            y += delta.y;

            if (x < 0 || x >= mazeGenerator.mazeWidth || y < 0 || y >= mazeGenerator.mazeHeight)
                break;

            if (maze[x, y].visited)
                break;

            maze[x, y].visited = true;

            length++;
        }
        return length;
    }

    private int FindLongestPathLength(MazeCell[,] maze, int startX, int startY)
    {
        Queue<(int x, int y, int length)> queue = new Queue<(int x, int y, int length)>();
        bool[,] visited = new bool[mazeGenerator.mazeWidth, mazeGenerator.mazeHeight];
        queue.Enqueue((startX, startY, 1));
        visited[startX, startY] = true;

        while (queue.Count > 0)
        {
            (int x, int y, int length) = queue.Dequeue();

            if (x == endCell.x && y == endCell.y)
            {
                return length;
            }

            // Explorar vecinos
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                int newX = x + mazeGenerator.GetDirectionDelta(direction).x;
                int newY = y + mazeGenerator.GetDirectionDelta(direction).y;
                if (newX >= 0 && newX < mazeGenerator.mazeWidth && newY >= 0 && newY < mazeGenerator.mazeHeight && !visited[newX, newY] && mazeGenerator.CanMove(x, y, direction))
                {
                    visited[newX, newY] = true;
                    queue.Enqueue((newX, newY, length + 1));
                }
            }
        }

        return 0;
    }

    private void PrintMazeStatistics(MazeCell[,] maze)
    {
        int totalCells = mazeGenerator.mazeWidth * mazeGenerator.mazeHeight;
        int deadEnds = 0;
        int threewayintersections = 0;
        int fourwayintersections = 0;

        foreach (MazeCell cell in maze)
        {
            if (cell.isDeadEnd) deadEnds++;
            if (cell.isThreeWayIntersection) threewayintersections++;
            if (cell.isFourWayIntersection) fourwayintersections++;
        }

        Debug.Log("Dead End Percentage: " + (deadEnds / (float)totalCells) * 100 + "%");
        Debug.Log("Average Corridor Length: " + CalculateAverageCorridorLength(maze));
        Debug.Log("Number of Three-Way Intersections: " + (threewayintersections));
        Debug.Log("Number of Four-Way Intersections: " + (fourwayintersections));
        Debug.Log("Solution Percentage: " + (FindLongestPathLength(maze, 0, 0)) / (float)totalCells * 100+"%");
    }



}
