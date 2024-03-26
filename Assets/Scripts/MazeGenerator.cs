using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.Rendering.VirtualTexturing;

public class MazeGenerator : MonoBehaviour
{
    [Range (5, 100)]
    public int mazeWidth= 50, mazeHeight = 50; //dimensiones
    private int startX, startY; //posición de inicio del algoritmo
    MazeCell[,] maze; //mapa del laberinto

    private Vector2Int startCorner;
    private List<Direction> biasDirections;

    Vector2Int currentCell; //la celda que se analiza en cada momento

    public enum MazeAlgorithm
    {
        RecursiveBacktracking,
        BinaryTree
    }
    public static MazeAlgorithm SelectedAlgorithm { get; set; }

    public MazeCell[,] GetMaze()
    {
        maze = new MazeCell[mazeWidth, mazeHeight];

        for (int i = 0; i < mazeWidth; i++)
        {
            for (int j = 0; j < mazeHeight; j++)
            {
                maze[i, j] = new MazeCell(i, j);
            }
        }
        System.Random random = new System.Random();
        startX = random.Next(mazeWidth);
        startY = random.Next(mazeHeight);
        if (SelectedAlgorithm == MazeAlgorithm.RecursiveBacktracking)
        {
            RecursiveBacktracking(startX, startY);
        }
        else if (SelectedAlgorithm == MazeAlgorithm.BinaryTree)
        {
            SelectStartCornerAndBias();
            BinaryTreeAlgorithm();
        }
        return maze;
    }
    List<Direction> directions = new List<Direction>() {
    Direction.Up, Direction.Down, Direction.Left, Direction.Right,
    };

    List<Direction> GetRandomDirections()
    {
        List<Direction> dir = new List<Direction>(directions);

        List<Direction> rndDir = new List<Direction>();

        while (dir.Count > 0)
        {
            int rnd = UnityEngine.Random.Range(0, dir.Count);
            rndDir.Add(dir[rnd]);
            dir.RemoveAt(rnd);
        }
        //se devuelven 4 direcciones aleatorias
        return rndDir;
    }

    List<Direction> Get2RandomDirections()
    {
        List<Direction> dir = new List<Direction>(directions);

        List<Direction> rndDir = new List<Direction>();

        while (dir.Count > 2)
        {
            int rnd = UnityEngine.Random.Range(0, dir.Count);
            if (!rndDir.Contains(dir[rnd]))
            {
                rndDir.Add(dir[rnd]);
                dir.RemoveAt(rnd);
            }
        }
        //se devuelven 2 direcciones aleatorias
        return rndDir;
    }
    bool IsCellValid (int x, int y)
    { //si está fuera del mapa o ya se ha visitado, no es válida
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight -1 || maze[x,y].visited)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void SelectStartCornerAndBias()
    {
        System.Random rand = new System.Random();
        int corner = rand.Next(4); //0: top-left, 1: top-right, 2: bottom-right, 3: bottom-left

        switch (corner)
        {
            case 0: //Top-left
                startCorner = new Vector2Int(0, mazeHeight - 1);
                biasDirections = new List<Direction> { Direction.Down, Direction.Right };
                break;
            case 1: //Top-right
                startCorner = new Vector2Int(mazeWidth - 1, mazeHeight - 1);
                biasDirections = new List<Direction> { Direction.Down, Direction.Left };
                break;
            case 2: //Bottom-right
                startCorner = new Vector2Int(mazeWidth - 1, 0);
                biasDirections = new List<Direction> { Direction.Up, Direction.Left };
                break;
            case 3: //Bottom-left
                startCorner = new Vector2Int(0, 0);
                biasDirections = new List<Direction> { Direction.Up, Direction.Right };
                break;
        }
    }

    void BreakWalls(Vector2Int primaryCell, Vector2Int secondaryCell)
    {
        if (primaryCell.x > secondaryCell.x)
        {
            maze[primaryCell.x, primaryCell.y].leftWall = false;
        }
        else if (primaryCell.x < secondaryCell.x)
        {
            maze[secondaryCell.x, secondaryCell.y].leftWall = false;
        }
        else if (primaryCell.y < secondaryCell.y)
        {
            maze[primaryCell.x, primaryCell.y].topWall = false;
        }
        else if (primaryCell.y > secondaryCell.y)
        {
            maze[secondaryCell.x, secondaryCell.y].topWall = false;
        }
    }

    void RecursiveBacktracking(int x, int y)
    {
        maze[x, y].visited = true;

        List<Direction> randomDirections = GetRandomDirections();

        foreach (var direction in randomDirections)
        {
            //calcula la próxima celda basada en la dirección actual
            int newX = x + GetDirectionDelta(direction).x;
            int newY = y + GetDirectionDelta(direction).y;

            if (IsCellValid(newX, newY) && !maze[newX, newY].visited)
            {
                //rompe las paredes entre la celda actual y la próxima celda
                BreakWalls(new Vector2Int(x, y), new Vector2Int(newX, newY));

                //recursivamente explora la próxima celda
                RecursiveBacktracking(newX, newY);
            }
        }
    }

    void BinaryTreeAlgorithm()
    {
        currentCell = startCorner;
        int xStart=0, yStart=0, xEnd = 0, yEnd = 0, xStep = 0, yStep = 0;
        if (currentCell.x == 0 && currentCell.y == 0)
        {
            xStart = 0;
            yStart = 0;
            xEnd = mazeWidth;
            yEnd = mazeHeight;
            xStep = 1;
            yStep=1;
        }
        else if (currentCell.x == mazeWidth-1 && currentCell.y == mazeHeight - 1)
        {
            xStart = mazeWidth - 1;
            yStart = mazeHeight - 1;
            xEnd = -1;
            yEnd = -1;
            xStep = -1;
            yStep = -1;
        }
        else if (currentCell.x == mazeWidth - 1 && currentCell.y == 0)
        {
            xStart = mazeWidth - 1;
            yStart = 0;
            xEnd = -1;
            yEnd = mazeHeight;
            xStep = -1;
            yStep = 1;
        }
        else if (currentCell.x == 0 && currentCell.y == mazeHeight - 1)
        {
            xStart = 0;
            yStart = mazeHeight - 1;
            xEnd = mazeWidth;
            yEnd = -1;
            xStep = 1;
            yStep = -1;
        }
        print(currentCell.x +","+currentCell.y);

        for (int x = xStart; (startX < xEnd) ? x < xEnd : x > xEnd; x += xStep)
        {
            for (int y = yStart; (yStart < yEnd) ? y < yEnd : y > yEnd; y += yStep)
            {
                Vector2Int cell = new Vector2Int(x, y);
                List<Vector2Int> validNeighbours = new List<Vector2Int>();

                foreach (var biasDirection in biasDirections)
                {
                    Vector2Int delta = GetDirectionDelta(biasDirection);
                    Vector2Int neighbour = cell + delta;

                    if (IsCellValid(neighbour.x, neighbour.y))
                    {
                        validNeighbours.Add(neighbour);
                    }
                }

                if (validNeighbours.Count > 0)
                {
                    Vector2Int selectedNeighbour = validNeighbours[UnityEngine.Random.Range(0, validNeighbours.Count)];
                    BreakWalls(cell, selectedNeighbour);
                    maze[cell.x, cell.y].visited = true;
                }
            }
        }
    }



    // Función auxiliar para obtener el desplazamiento basado en la dirección
    Vector2Int GetDirectionDelta(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return new Vector2Int(0, 1);
            case Direction.Down:
                return new Vector2Int(0, -1);
            case Direction.Left:
                return new Vector2Int(-1, 0);
            case Direction.Right:
                return new Vector2Int(1, 0);
            default:
                return new Vector2Int(0, 0);
        }
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class MazeCell
{
    public bool visited;
    public int x, y;

    public bool topWall;
    public bool leftWall;

    public Vector2Int position
    {
        get
        {
            return new Vector2Int(x, y);
        }
    }

    public MazeCell(int x, int y)
    {
        //coordenadas de la celda
        this.x = x;
        this.y = y;

        visited = false;

        topWall = true;
        leftWall = true;
    }
}