using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.Rendering.VirtualTexturing;
using System.Text;
using System.Linq;

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
        Prim,
        AldousBroder,
        GrowingTree,
        HuntAndKill,
        Wilson,
        RecursiveDivision,
        Sidewinder,
        Eller
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
                BinaryTree();
                break;
            case MazeAlgorithm.Kruskal:
                Kruskal();
                break;
            case MazeAlgorithm.Prim:
                Prim(startX, startY);
                break;
            case MazeAlgorithm.AldousBroder:
                AldousBroder(startX, startY);
                break;
            case MazeAlgorithm.GrowingTree:
                GrowingTree(startX, startY);
                break;
            case MazeAlgorithm.HuntAndKill:
                HuntAndKill(startX, startY);
                break;
            case MazeAlgorithm.Wilson:
                Wilson(startX, startY);
                break;
            case MazeAlgorithm.RecursiveDivision:
                InitializeMazeOpen();
                //Comienza la división desde todo el espacio del laberinto.
                RecursiveDivision(0, 0, mazeWidth, mazeHeight, ChooseOrientation(mazeWidth, mazeHeight));
                break;
            case MazeAlgorithm.Sidewinder:
                Sidewinder();
                break;
            case MazeAlgorithm.Eller:
                Eller();
                break;
        }
        /*
        if (IsMazePerfect())
        {
            print("Maze is perfect");
        }
        else
        {
            print("Maze is not perfect");
        }
        */
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

    void BinaryTree()
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

    void Kruskal()
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

    void Prim(int x, int y)
    {
        //lista de celdas frontera
        List<Vector2Int> frontierCells = new List<Vector2Int>();
        currentCell = new Vector2Int(x, y);
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

    void AldousBroder(int x, int y)
    {
        int remaining = mazeWidth * mazeHeight - 1; //Total de celdas menos la celda inicial.
        currentCell = new Vector2Int(x, y);
        // Marca la celda inicial como visitada.
        maze[x, y].visited = true;

        // Continúa mientras haya celdas no visitadas.
        while (remaining > 0)
        {
            // Obtiene direcciones en orden aleatorio.
            List<Direction> randomDirections = GetRandomDirections();

            foreach (var direction in randomDirections)
            {
                // Calcula la próxima celda basada en la dirección actual.
                Vector2Int delta = GetDirectionDelta(direction);
                Vector2Int nextCell = new Vector2Int(currentCell.x + delta.x, currentCell.y + delta.y);

                // Verifica si la celda está dentro de los límites.
                if (IsWithinBounds(nextCell))
                {
                    // Si la celda no ha sido visitada.
                    if (!maze[nextCell.x, nextCell.y].visited)
                    {
                        // Rompe las paredes entre la celda actual y la próxima celda.
                        BreakWalls(currentCell, nextCell);

                        // Marca la celda como visitada.
                        maze[nextCell.x, nextCell.y].visited = true;

                        // Decrementa el contador de celdas restantes.
                        remaining--;
                    }

                    // Mueve a la nueva celda.
                    currentCell = nextCell;
                    break; // Sal de la iteración de direcciones una vez que te mueves.
                }
            }
        }
    }

    void GrowingTree(int x, int y)
    {
        //Lista de celdas activas.
        List<Vector2Int> activeCells = new List<Vector2Int>();

        //selecciona una celda aleatoria y la añade a la lista.
        currentCell = new Vector2Int(x, y);
        activeCells.Add(currentCell);
        maze[x, y].visited = true;

        // Continúa mientras la lista no esté vacía.
        while (activeCells.Count > 0)
        {
            //Selecciona una celda de la lista
            int index = ChooseIndex(activeCells.Count);
            currentCell = activeCells[index];

            // Obtiene direcciones aleatorias para buscar vecinos no visitados.
            List<Direction> randomDirections = GetRandomDirections();
            bool foundUnvisitedNeighbor = false;

            foreach (var direction in randomDirections)
            {
                Vector2Int delta = GetDirectionDelta(direction);
                Vector2Int nextCell = new Vector2Int(currentCell.x + delta.x, currentCell.y + delta.y);

                // Verifica si el vecino está dentro de los límites y no ha sido visitado.
                if (IsWithinBounds(nextCell) && !maze[nextCell.x, nextCell.y].visited)
                {
                    // Rompe las paredes y marca el vecino como visitado.
                    BreakWalls(currentCell, nextCell);
                    maze[nextCell.x, nextCell.y].visited = true;

                    // Añade el vecino a la lista de celdas activas.
                    activeCells.Add(nextCell);
                    foundUnvisitedNeighbor = true;
                    break;
                }
            }

            // Si no se encontró ningún vecino no visitado, elimina la celda actual de la lista.
            if (!foundUnvisitedNeighbor)
            {
                activeCells.RemoveAt(index);
            }
        }
    }

    // Método para elegir el índice de la celda activa a procesar.
    int ChooseIndex(int count)
    {
        // Siempre elige la última celda añadida (comportamiento similar al Backtracking recursivo).
        return count - 1;
        //Se podría hacer return Random(count) o return 0 para coger la celda añadida más antigua (similar a árbol binario)
    }

    void HuntAndKill(int x, int y)
    {

        maze[x, y].visited = true;

        while (true)
        {
            Vector2Int? walkResult = Walk(x, y);

            if (walkResult.HasValue)
            {
                x = walkResult.Value.x;
                y = walkResult.Value.y;
            }
            else
            {
                Vector2Int? huntResult = Hunt();
                if (!huntResult.HasValue)
                {
                    // Termina el algoritmo si Hunt no encuentra un nuevo punto de inicio.
                    break;
                }
                else
                {
                    x = huntResult.Value.x;
                    y = huntResult.Value.y;
                }
            }
        }
    }

    Vector2Int? Walk(int x, int y)
    {
        List<Direction> randomDirections = GetRandomDirections();
        foreach (var direction in randomDirections)
        {
            Vector2Int delta = GetDirectionDelta(direction);
            int nx = x + delta.x;
            int ny = y + delta.y;

            if (IsWithinBounds(new Vector2Int(nx, ny)) && !maze[nx, ny].visited)
            {
                BreakWalls(new Vector2Int(x, y), new Vector2Int(nx, ny));
                maze[nx, ny].visited = true;
                return new Vector2Int(nx, ny);
            }
        }

        return null;
    }

    Vector2Int? Hunt()
    {
        for (int y = 0; y < mazeHeight; y++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                if (!maze[x, y].visited && HasVisitedNeighbors(x, y))
                {
                    // Encuentra y conecta con un vecino visitado.
                    ConnectToVisitedNeighbor(x, y);
                    maze[x, y].visited = true;
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }

    void ConnectToVisitedNeighbor(int x, int y)
    {
        List<Direction> directions = GetRandomDirections();
        foreach (var direction in directions)
        {
            Vector2Int delta = GetDirectionDelta(direction);
            int nx = x + delta.x;
            int ny = y + delta.y;

            if (IsWithinBounds(new Vector2Int(nx, ny)) && maze[nx, ny].visited)
            {
                // Conecta con el primer vecino visitado encontrado y sale del método.
                BreakWalls(new Vector2Int(x, y), new Vector2Int(nx, ny));
                return; 
            }
        }
    }

    bool HasVisitedNeighbors(int x, int y)
    {
        foreach (var direction in directions)
        {
            Vector2Int delta = GetDirectionDelta(direction);
            int nx = x + delta.x;
            int ny = y + delta.y;
            if (IsWithinBounds(new Vector2Int(nx, ny)) && maze[nx, ny].visited)
            {
                return true;
            }
        }
        return false;
    }

    void Wilson(int x, int y)
    {
        maze[x, y].visited = true;

        // Repite hasta que todas las celdas estén visitadas
        while (AnyCellNotVisited())
        {
            Vector2Int cellNotVisited = ChooseRandomCellNotVisited();
            List<Vector2Int> path = PerformRandomWalk(cellNotVisited);

            // Añade el camino y elimina las paredes correspondientes
            for (int i = 0; i < path.Count - 1; i++)
            {
                currentCell = path[i];
                Vector2Int nextCell = path[i + 1];

                maze[currentCell.x, currentCell.y].visited = true;
                BreakWalls(currentCell, nextCell);
            }
            //se marca la última celda también como visitada
            Vector2Int lastCell = path[path.Count - 1];
            maze[lastCell.x, lastCell.y].visited = true;
        }
    }

    bool AnyCellNotVisited()
    {
        for (int i = 0; i < mazeWidth; i++)
        {
            for (int j = 0; j < mazeHeight; j++)
            {
                if (!maze[i, j].visited) return true;
            }
        }
        return false;
    }

    Vector2Int ChooseRandomCellNotVisited()
    {
        System.Random rand = new System.Random();
        int x, y;
        do
        {
            x = rand.Next(mazeWidth);
            y = rand.Next(mazeHeight);
        } while (maze[x, y].visited);

        return new Vector2Int(x, y);
    }

    List<Vector2Int> PerformRandomWalk(Vector2Int start)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = start;
        path.Add(current);

        while (!maze[current.x, current.y].visited)
        {
            List<Direction> possibleDirections = GetRandomDirections();
            System.Random rand = new System.Random();
            Direction chosenDirection = possibleDirections[rand.Next(possibleDirections.Count)];
            Vector2Int next = current + GetDirectionDelta(chosenDirection);
            if (IsWithinBounds(next))
            {
                if (!path.Contains(next)) //Si no forma un bucle
                {
                    path.Add(next);
                }
                else //Si forma un bucle, elimina el bucle
                {
                    int loopStartIndex = path.IndexOf(next);
                    path = path.GetRange(0, loopStartIndex + 1);
                }
                current = next;
            }
        }

        return path;
    }

    void RecursiveDivision(int x, int y, int width, int height, Orientation orientation)
    {
        if (width < 2 || height < 2)
        {
            //El área es demasiado pequeña para dividir más.
            return;
        }

        bool horizontal = orientation == Orientation.Horizontal;

        //Decide dónde dibujar la nueva pared divisoria.
        int wallPosition = horizontal ? UnityEngine.Random.Range(y, y + height - 1) : UnityEngine.Random.Range(x, x + width - 1);

        //Decide dónde será el pasaje a través de la pared.
        int passagePosition = horizontal ? UnityEngine.Random.Range(x, x + width) : UnityEngine.Random.Range(y, y + height);

        //Dibuja la nueva pared y asegura un pasaje a través de ella.
        if (horizontal)
        {
            for (int i = x; i < x + width; i++)
            {
                if (i != passagePosition)
                {
                    BuildWalls(new Vector2Int(i, wallPosition), new Vector2Int(i, wallPosition + 1));
                }
            }
        }
        else
        {
            for (int i = y; i < y + height; i++)
            {
                if (i != passagePosition)
                {
                    BuildWalls(new Vector2Int(wallPosition, i), new Vector2Int(wallPosition + 1, i));
                }
            }
        }

        //Recursividad para dividir las dos nuevas secciones creadas.
        if (horizontal)
        {
            RecursiveDivision(x, y, width, wallPosition - y + 1, ChooseOrientation(width, wallPosition - y + 1));
            RecursiveDivision(x, wallPosition + 1, width, y + height - wallPosition - 1, ChooseOrientation(width, y + height - wallPosition - 1));
        }
        else
        {
            RecursiveDivision(x, y, wallPosition - x + 1, height, ChooseOrientation(wallPosition - x + 1, height));
            RecursiveDivision(wallPosition + 1, y, x + width - wallPosition - 1, height, ChooseOrientation(x + width - wallPosition - 1, height));
        }
    }

    void InitializeMazeOpen()
    {
        for (int i = 0; i < mazeWidth; i++)
        {
            for (int j = 0; j < mazeHeight; j++)
            {
                maze[i, j].topWall = false;
                maze[i, j].leftWall = false;
                maze[0, j].leftWall = true;
                maze[i, mazeHeight - 1].topWall = true;
            }
        }
    }

    Orientation ChooseOrientation(int width, int height)
    {
        if (width < height)
        {
            return Orientation.Horizontal;
        }
        else if (height < width)
        {
            return Orientation.Vertical;
        }
        else
        {
            return UnityEngine.Random.Range(0, 2) == 0 ? Orientation.Horizontal : Orientation.Vertical;
        }
    }

    void Sidewinder()
    {
        // Iterar a través de cada fila
        for (int y = 0; y < mazeWidth; y++)
        {
            int runStart = 0;

            // Iterar a través de cada columna
            for (int x = 0; x < mazeHeight; x++)
            {
                // Se determina si se va a excavar hacia el este
                bool carveEast = x < mazeHeight - 1 && (y == 0 || UnityEngine.Random.Range(0, 2) == 0);

                if (carveEast)
                {
                    // Excavar hacia el este
                    BreakWalls(new Vector2Int(x, y), new Vector2Int(x + 1, y));
                }

                // Se determina si se va a excavar hacia el norte
                bool carveNorth = !carveEast || x == mazeHeight - 1;

                if (carveNorth && y > 0)
                {
                    // Elegir una celda aleatoria del conjunto actual (run) para excavar hacia el norte
                    int northCarveX = runStart + UnityEngine.Random.Range(0, x - runStart + 1);
                    BreakWalls(new Vector2Int(northCarveX, y), new Vector2Int(northCarveX, y - 1));
                    runStart = x + 1;
                }
            }
        }
    }

    void Eller()
    {
        EllerSet setManager = new EllerSet();

        for (int y = 0; y < mazeHeight; y++)
        {
            //Procesar fusiones horizontales en la fila actual
            for (int x = 0; x < mazeWidth - 1; x++)
            {
                if (!setManager.IsInSet(new Vector2Int(x,y)))
                {
                    setManager.AddCell(new Vector2Int(x, y));
                }
                if (!setManager.IsInSet(new Vector2Int(x+1, y)))
                {
                    setManager.AddCell(new Vector2Int(x + 1, y));
                }
                //si es la última fila, se junta todo (no de forma aleatoria)
                if (y == mazeHeight-1)
                {
                    if (!setManager.AreInSameSet(new Vector2Int(x, y), new Vector2Int(x + 1, y)))
                    {
                        BreakWalls(new Vector2Int(x, y), new Vector2Int(x + 1, y));
                        setManager.MergeSets(new Vector2Int(x, y), new Vector2Int(x + 1, y));
                    }
                }
                else
                {
                    bool shouldMerge = UnityEngine.Random.Range(0, 2) == 0;
                    if (shouldMerge && !setManager.AreInSameSet(new Vector2Int(x, y), new Vector2Int(x + 1, y)))
                    {
                        BreakWalls(new Vector2Int(x, y), new Vector2Int(x + 1, y));
                        setManager.MergeSets(new Vector2Int(x, y), new Vector2Int(x + 1, y));
                    }
                }
            }
            List<Vector2Int> cellsToKeep = new List<Vector2Int>();
            if (y < mazeHeight - 1)
            {
                foreach (var setId in setManager.GetSetIds())
                {
                    var cellsInSet = setManager.GetCellsInSet(setId).ToList();

                    // Asegurar al menos una conexión vertical por conjunto
                    Vector2Int cellToExtend = cellsInSet[UnityEngine.Random.Range(0, cellsInSet.Count)];
                    Vector2Int cellBelow = cellToExtend + Vector2Int.up; // Celda directamente debajo

                    //Romper pared hacia abajo para asegurar conexión
                    BreakWalls(cellToExtend, cellBelow);
                    setManager.AddCell(cellBelow);
                    setManager.MergeSets(cellToExtend, cellBelow); //Agregar la celda de abajo al conjunto
                    cellsToKeep.Add(cellBelow);

                    //Crear conexiones verticales adicionales (aleatorio)
                    foreach (var cell in cellsInSet)
                    {
                        if (UnityEngine.Random.Range(0, 2) == 0 && cell != cellToExtend)
                        {
                            Vector2Int additionalCellBelow = cell + Vector2Int.up;

                            //Romper pared hacia abajo para las conexiones verticales adicionales
                            BreakWalls(cell, additionalCellBelow);
                            setManager.AddCell(additionalCellBelow);
                            setManager.MergeSets(cell, additionalCellBelow); //Añadir celda adicional de abajo al conjunto
                            cellsToKeep.Add(additionalCellBelow);
                        }
                    }
                }
            }
            setManager.PrepareNextRow(cellsToKeep);
        }
    }

    void BuildWalls(Vector2Int primaryCell, Vector2Int secondaryCell)
    {
        if (primaryCell.x > secondaryCell.x)
        {
            maze[primaryCell.x, primaryCell.y].leftWall = true;
        }
        else if (primaryCell.x < secondaryCell.x)
        {
            maze[secondaryCell.x, secondaryCell.y].leftWall = true;
        }
        else if (primaryCell.y < secondaryCell.y)
        {
            maze[primaryCell.x, primaryCell.y].topWall = true;
        }
        else if (primaryCell.y > secondaryCell.y)
        {
            maze[secondaryCell.x, secondaryCell.y].topWall = true;
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

    public bool IsMazePerfect()
    {
        // Reinicia el estado visitado de todas las celdas
        for (int i = 0; i < mazeWidth; i++)
        {
            for (int j = 0; j < mazeHeight; j++)
            {
                maze[i, j].visited = false;
            }
        }

        // Inicia DFS desde la esquina superior izquierda (o cualquier otra celda que prefieras)
        DFS(0, 0);

        // Verifica si todas las celdas han sido visitadas
        for (int i = 0; i < mazeWidth; i++)
        {
            for (int j = 0; j < mazeHeight; j++)
            {
                if (!maze[i, j].visited)
                {
                    return false; // Si alguna celda no fue visitada, el laberinto no es perfecto
                }
            }
        }

        return true; // Todas las celdas fueron visitadas
    }

    void DFS(int x, int y)
    {
        // Marca la celda actual como visitada
        maze[x, y].visited = true;

        // Lista de todas las direcciones posibles
        List<Direction> directions = new List<Direction> { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        foreach (var direction in directions)
        {
            Vector2Int nextCell = new Vector2Int(x, y) + GetDirectionDelta(direction);

            // Comprueba si la celda vecina está dentro de los límites y si hay un camino hacia ella
            if (IsWithinBounds(nextCell) && CanMove(x, y, direction) && !maze[nextCell.x, nextCell.y].visited)
            {
                DFS(nextCell.x, nextCell.y); // Continúa la búsqueda en profundidad desde la celda vecina
            }
        }
    }

    bool CanMove(int x, int y, Direction direction)
    {
        // Este método debe verificar si se puede mover en la dirección dada desde la celda actual
        // Esto implica comprobar si no hay paredes bloqueando el movimiento en esa dirección
        // La implementación específica dependerá de cómo estén definidas tus paredes
        // Aquí tienes un esquema básico:
        switch (direction)
        {
            case Direction.Up:
                return !maze[x, y].topWall;
            case Direction.Down:
                // Necesitarás verificar la celda debajo de la actual
                return y > 0 && !maze[x, y - 1].topWall;
            case Direction.Left:
                return !maze[x, y].leftWall;
            case Direction.Right:
                // Necesitarás verificar la celda a la derecha de la actual
                return x < mazeWidth - 1 && !maze[x + 1, y].leftWall;
            default:
                return false;
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

public enum Orientation
{
    Horizontal,
    Vertical
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