using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.Rendering.VirtualTexturing;
using System.Text;

public class MazeGenerator : MonoBehaviour
{
    [Range (5, 100)]
    public int mazeWidth= 20, mazeHeight = 20; //dimensiones
    private int startX, startY; //posición de inicio del algoritmo
    MazeCell[,] maze; //mapa del laberinto

    private Vector2Int startCorner;
    private List<Direction> biasDirections;

    Vector2Int currentCell; //la celda que se analiza en cada momento

    public enum MazeAlgorithm
    {
        RecursiveBacktracking,
        BinaryTree,
        Kruskal,
        Prim
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
        switch (SelectedAlgorithm)
        {
            case MazeAlgorithm.RecursiveBacktracking:
                RecursiveBacktracking(startX, startY);
                break;
            case MazeAlgorithm.BinaryTree:
                SelectStartCornerAndBias();
                BinaryTreeAlgorithm();
                break;
            case MazeAlgorithm.Kruskal:
                KruskalAlgorithm();
                break;
            case MazeAlgorithm.Prim:
                PrimAlgorithm(startX, startY);
                break;
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

            if (IsCellValid(newX, newY))
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

    void KruskalAlgorithm()
    {
        //Lista para almacenar todos los posibles muros a derribar
        List<KeyValuePair<Vector2Int, Vector2Int>> walls = new List<KeyValuePair<Vector2Int, Vector2Int>>();

        //Rellenar la lista de muros posibles
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                //por cada celda, se añade el muro posible tanto vertical como el horizontal
                if (x < mazeWidth - 1)
                {
                    walls.Add(new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(x, y), new Vector2Int(x + 1, y)));
                }
                if (y < mazeHeight - 1)
                {
                    walls.Add(new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(x, y), new Vector2Int(x, y + 1)));
                }
            }
        }

        //Se cogen todos los muros aleatorios
        System.Random rng = new System.Random();
        int n = walls.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var value = walls[k];
            walls[k] = walls[n];
            walls[n] = value;
        }

        // Inicializa las estructuras para el seguimiento de conjuntos disjuntos
        UnionFind unionFind = new UnionFind(mazeWidth * mazeHeight);

        foreach (var wall in walls)
        {
            Vector2Int cell1 = wall.Key;
            Vector2Int cell2 = wall.Value;

            //pasamos los índices de cada celda de representación bidimensional a unidimensional
            int cell1Index = cell1.x + cell1.y * mazeWidth;
            int cell2Index = cell2.x + cell2.y * mazeWidth;

            //si las celdas no están ya conectadas, derriba el muro entre ellas
            if (!unionFind.Connected(cell1Index, cell2Index))
            {
                //une las celdas de forma unidimensional y de forma bidimensional corta las paredes entre las celdas
                unionFind.Union(cell1Index, cell2Index);
                BreakWalls(cell1, cell2);
            }
        }
    }

    void PrimAlgorithm(int x, int y)
    {
        //lista de celdas frontera
        List<Vector2Int> frontierCells = new List<Vector2Int>();
        Vector2Int currentCell = new Vector2Int(x, y);
        maze[currentCell.x, currentCell.y].visited = true;
        //añade las fronteras de la celda actual a la lista de fronteras
        AddFrontierCells(currentCell, ref frontierCells);

        while (frontierCells.Count > 0)
        {
            //coge una frontera aleatoria
            int rndIndex = UnityEngine.Random.Range(0, frontierCells.Count);
            Vector2Int frontierCell = frontierCells[rndIndex];
            //coge una celda conectada a la celda frontera
            Vector2Int nextCell = GetConnectedCell(frontierCell);

            //rompe la pared entre la celda frontera y la escogida
            BreakWalls(frontierCell, nextCell);

            maze[frontierCell.x, frontierCell.y].visited = true;
            //añade las celdas frontera de la celda frontera ya visitada a la lista de fronteras
            AddFrontierCells(frontierCell, ref frontierCells);
            //quita de la lista de celdas frontera la celda frontera ya visitada
            frontierCells.RemoveAt(rndIndex);
        }
    }

    List<Vector2Int> direcciones = new List<Vector2Int>
    {
        new Vector2Int(0, 1), // Arriba
        new Vector2Int(0, -1), // Abajo
        new Vector2Int(-1, 0), // Izquierda
        new Vector2Int(1, 0) // Derecha
    };

    //método que añade a la lista de celdas frontera todas las fronteras de la celda pasada por parámetro
    void AddFrontierCells(Vector2Int cell, ref List<Vector2Int> frontierCells)
    {
        foreach (var direction in direcciones)
        {
            //se miran todas las fronteras de la celda pasada por parámetro y se añaden a la lista
            Vector2Int adjacentCell = cell + direction;
            if (IsCellValid(adjacentCell.x, adjacentCell.y) && !frontierCells.Contains(adjacentCell))
            {
                frontierCells.Add(adjacentCell);
            }
        }
    }

    //método que consigue una celda aleatoria conectada a la celda pasada por parámetro
    Vector2Int GetConnectedCell(Vector2Int frontierCell)
    {
        List<Vector2Int> validCells = new List<Vector2Int>();
        foreach (var direction in direcciones)
        {
            Vector2Int adjacentCell = frontierCell + direction;
            if (IsWithinBounds(adjacentCell) && maze[adjacentCell.x, adjacentCell.y].visited)
            {
                validCells.Add(adjacentCell);
            }
        }

        if (validCells.Count > 0)
        {
            int rndIndex = UnityEngine.Random.Range(0, validCells.Count);
            return validCells[rndIndex];
        }
        else
        {
            //devuelve un valor que indica claramente que no se encontró ninguna celda válida.
            return new Vector2Int(-1, -1);
        }
    }

    //método que comprueba si la celda pasada por parámetro está dentro de los límites del laberinto
    bool IsWithinBounds(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < mazeWidth && cell.y >= 0 && cell.y < mazeHeight;
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