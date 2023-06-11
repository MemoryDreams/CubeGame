using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject Player;
    public GameObject Finish;
    public GameObject WallObj;
    public GameObject NodeObj;
    public GameObject Mine;
    public GameObject cubeParticle;
    public int MazeGridX;
    public int MazeGridY;
    public int wallIntencity;
    private System.Random random;
    public int mineChance = 90;
    public MazeCellData[,] MazeGrid;
    public MazeCellData[,] MineGrid;
    public bool ProperMaze = false;
    public int recursiveCounter = 0;
    public int recursiveLimit = 10;
    public int paths = 0;
    public float animationWait = 0;
    public float animationLimit = 5;
    public bool cubeDestroyed = false;
    public AlphaChangerScript blackScreen;
    public float alphaChangeValue = 1;
    public bool blackScreenSequence = false;
    public bool alphaUp = true;
    public bool nextLevel = false;
    public SystemController SystemController;
    
    public enum MazeCell
    {
        Empty,
        Wall,
        Edge,
        Start,
        End,
        Visited,
        DeadEnd,
        FastestRoute,
        Node,
        Mine,
        Traversed
    }

    public class MazeCellData
    {
        public MazeCell contents { get; set; }
    }


    void Start()
    {
        MazeGrid = new MazeCellData[MazeGridX, MazeGridY];
        MineGrid = new MazeCellData[MazeGridX, MazeGridY];
        BuildMaze();
    }

    void Update()
    {
        if (!SystemController.paused && cubeDestroyed)
            {
                animationWait+=Time.deltaTime;
                if (animationWait>animationLimit)
                {
                    cleanUpCubes();
                    spawnCube();
                    cubeDestroyed = false;
                }
            }
        if (blackScreenSequence)
            {
                if (alphaUp)
                {
                    blackScreen.alphaValue += alphaChangeValue;
                    if (blackScreen.alphaValue >= 1f)
                    {
                        blackScreenSequence = false;
                        if (nextLevel)
                        {
                            nextLevel = false;
                            restartGame();
                        }
                    }
                } 
                else
                {
                    blackScreen.alphaValue -= alphaChangeValue;
                    if (blackScreen.alphaValue <= 0)
                    {
                        blackScreenSequence = false;
                    }
                }
            }
    }

    void cleanWalls()
    {
        GameObject[] WallObjects = GameObject.FindGameObjectsWithTag("Wall");

        foreach (GameObject obj in WallObjects)
        {
            Destroy(obj);
        }
    }

    void cleanMines()
    {
        GameObject[] MineObjects = GameObject.FindGameObjectsWithTag("Mine");

        foreach (GameObject obj in MineObjects)
        {
            Destroy(obj);
        }
    }

    void BuildMaze()
    {
        // Initialize the grid with walls
        for (int x = 0; x < MazeGridX; x++)
        {
            for (int y = 0; y < MazeGridY; y++)
            {
                MazeGrid[x, y] = new MazeCellData();
                MazeGrid[x, y].contents = MazeCell.Wall;

                MineGrid[x, y] = new MazeCellData();
                MineGrid[x, y].contents = MazeCell.Wall;
            }
        }

        // Start carving the maze from a random cell
        CarveMazeFromCell(1, 1);
        ProperMaze = false;

        // Add start and finish cells
        MazeGrid[1, 1].contents = MazeCell.Start;
        MazeGrid[MazeGridX - 2, MazeGridY - 2].contents = MazeCell.End;
        spawnEntities();
        BuildTheEdge();
        ensurePath();
        spawnCube();
        spawnFinish();
        putMines();
        clearMines();

        for (int x = 0; x < MazeGridX; x++)
        {
            for (int y = 0; y < MazeGridY; y++)
            {
                if (MazeGrid[x, y].contents == MazeCell.Wall)
                {
                    Vector3 position = new Vector3(x, 0.25f, y);
                    Quaternion rotation = Quaternion.identity;
                    GameObject instantiatedObject = Instantiate(WallObj, position, rotation);
                }
                
                if (MineGrid[x, y].contents == MazeCell.Mine)
                {
                    Vector3 position = new Vector3(x, 0.25f, y);
                    Quaternion rotation = Quaternion.identity;
                    GameObject instantiatedObject = Instantiate(Mine, position, rotation);
                }
                
                // if (MazeGrid[x, y].contents == MazeCell.Visited)
                // {
                //     Vector3 position = new Vector3(x, 1, y);
                //     Quaternion rotation = Quaternion.identity;
                //     GameObject instantiatedObject = Instantiate(NodeObj, position, rotation);
                // }
            }
        }

    }

    void putMines()
    {
        for (int x = 0; x < MazeGridX; x++)
        {
            for (int y=0;y< MazeGridY; y++)
            {
                if (MineGrid[x, y].contents == MazeCell.Empty)
                {
                    random = new System.Random();
                    int mineNumber = random.Next(0, 100);
                    if (mineNumber > mineChance && (MineGrid[x + 1, y].contents != MazeCell.Mine && MazeGrid[x - 1, y].contents != MazeCell.Mine && MineGrid[x, y + 1].contents != MazeCell.Mine && MazeGrid[x, y - 1].contents != MazeCell.Mine))
                    {
                        MineGrid[x, y].contents = MazeCell.Mine;
                    }
                }
            }
        }
    }

    void CarveMazeFromCell(int x, int y)
    {
        MazeGrid[x, y].contents = MazeCell.Empty;
        MineGrid[x, y].contents = MazeCell.Empty;

        int[] directions = { 0, 1, 2, 3 };
        for (int j = 0; j < directions.Length; j++)
        {
            random = new System.Random();
            directions[j] = random.Next(0, 3);
            
        }
        for (int i = 0; i < directions.Length; i++)
        {
            int dx = 0, dy = 0;

            switch (directions[i])
            {
                case 0: 
                    dy = -2;
                    break;
                case 1: 
                    dx = 2;
                    break;
                case 2:
                    dy = 2;
                    break;
                case 3:
                    dx = -2;
                    break;
            }

            int nx = x + dx;
            int ny = y + dy;

            if (nx >= 1 && ny >= 1 && nx < MazeGridX - 1 && ny < MazeGridY - 1)
            {
                if (MazeGrid[nx, ny].contents == MazeCell.Wall)
                {
                    MazeGrid[x + dx / 2, y + dy / 2].contents = MazeCell.Empty;
                    MineGrid[x + dx / 2, y + dy / 2].contents = MazeCell.Empty;
                    CarveMazeFromCell(nx, ny);
                }
            }
        }
    }

    public void restartGame()
    {
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        Destroy(GameObject.FindGameObjectWithTag("Finish"));
        cubeDestroyed = false;
        cleanWalls();
        cleanMines();
        cleanUpCubes();
        BuildMaze();
        alphaUp = false;
        blackScreenSequence = true;
    }

    void ensurePath()
    {
        FindPath(1,1);
        if (!ProperMaze)
        {
            BuildMaze();
            return;
        } 
        else 
        {
            return;
        }
    }

    void MakeItEasier()
    {
        for (int x = 1; x < MazeGridX-1; x++)
        {
            for (int y = 1; y < MazeGridY-1; y++)
            {
                if (MazeGrid[x, y].contents == MazeCell.Wall && !HorizontallyBlocked(x,y))
                {   
                    random = new System.Random();
                    int randomNumber = random.Next(0, 10);
                    if (randomNumber > wallIntencity)
                    {
                        MazeGrid[x, y].contents = MazeCell.Empty;
                    }
                }
            }
        }
    }

    void enlargePath()
    {
        for (int x = 1; x < MazeGridX-1; x++)
        {
            for (int y = 1; y < MazeGridY-1; y++)
            {
                if (MazeGrid[x, y].contents == MazeCell.Empty && (HorizontallyBlocked(x,y) && VerticallyBlocked(x,y)))
                {
                    MazeGrid[x-1, y].contents = MazeCell.Empty;
                    MazeGrid[x+1, y].contents = MazeCell.Empty;
                    MazeGrid[x, y+1].contents = MazeCell.Empty;
                    MazeGrid[x, y-1].contents = MazeCell.Empty;
                }
            }
        }
    }

    void FindPath(int x, int y)
    {
        if (x > MazeGridX - 1 || y > MazeGridY - 1 || x < 1 || y < 1)
            return; // Out of bounds, return

        if (MazeGrid[x,y].contents == MazeCell.Wall || MazeGrid[x,y].contents == MazeCell.Visited || MazeGrid[x,y].contents == MazeCell.Node)
        {
            return;
        }

        if (MazeGrid[x, y].contents == MazeCell.End)
        {
            ProperMaze = true;
            Debug.Log("Path found!"); // Exit found, do something
            return;
        }


        // if (MazeGrid[x, y + 1].contents == MazeCell.Empty)
        // {
        //     paths++;
        // }             
        // if (MazeGrid[x + 1, y].contents == MazeCell.Empty) 
        // {
        //     paths++;
        // }
        // if (MazeGrid[x - 1, y].contents == MazeCell.Empty) 
        // {
        //     paths++;
        // }
        // if (MazeGrid[x, y - 1].contents == MazeCell.Empty) 
        // {
        //     paths++;
        // } 
        
        // if (paths>=2){
        //     Debug.Log("Node found!");
        //     MazeGrid[x,y].contents = MazeCell.Node;
        // }
        
        paths = 0;
        if (MazeGrid[x, y].contents != MazeCell.Node)
            MazeGrid[x, y].contents = MazeCell.Visited; // Mark the cell as visited
        if (!ProperMaze)
            FindPath(x, y + 1);
        if (!ProperMaze)
            FindPath(x + 1, y);
        if (!ProperMaze)
            FindPath(x - 1, y);
        if (!ProperMaze)
            FindPath(x, y - 1);
        if (!ProperMaze)
            MazeGrid[x,y].contents = MazeCell.DeadEnd;
    }

    void BuildTheEdge()
    {
        for (int x=0; x<MazeGridX; x++)
        {
            MazeGrid[x, 0].contents = MazeCell.Wall;
            MazeGrid[x, MazeGridY-1].contents = MazeCell.Wall;
        }

        for (int y=0; y<MazeGridY; y++)
        {
            MazeGrid[0, y].contents = MazeCell.Wall;
            MazeGrid[MazeGridX-1, y].contents = MazeCell.Wall;
        }
    }

    bool HorizontallyBlocked(int x,int y)
    {
        if (MazeGrid[x+1, y].contents == MazeCell.Empty || MazeGrid[x-1, y].contents == MazeCell.Empty)
        {
            return false;
        }
        return true;
    }

    bool VerticallyBlocked(int x,int y)
    {
        if (MazeGrid[x, y+1].contents == MazeCell.Empty || MazeGrid[x, y-1].contents == MazeCell.Empty)
        {
            return false;
        }
        return true;
    }

    MazeCell randomChoice()
    {   
        random = new System.Random();
        int randomNumber = random.Next(0, 10);
        if (randomNumber > wallIntencity)
        {
            return MazeCell.Empty;
        }
        else
        {
            return MazeCell.Wall;
        }
    }

    void spawnEntities()
    {
        MazeGrid[1, 1].contents = MazeCell.Start;
        MazeGrid[1, 2].contents = MazeCell.Empty;
        MazeGrid[2, 1].contents = MazeCell.Empty;
        MazeGrid[2, 2].contents = MazeCell.Empty;

        MazeGrid[MazeGridX-2, MazeGridY-2].contents = MazeCell.End;
        MazeGrid[MazeGridX-2, MazeGridY-3].contents = MazeCell.Empty;
        MazeGrid[MazeGridX-3, MazeGridY-2].contents = MazeCell.Empty;
        MazeGrid[MazeGridX-3, MazeGridY-3].contents = MazeCell.Empty;
        Debug.Log("Instantiate Player and Finish");
    }

    void spawnCube()
    {
        Vector3 PlayerPos = new Vector3(1, 0.125f, 1);
        Quaternion rotation = Quaternion.identity;
        GameObject InstantiatePlayer = Instantiate(Player, PlayerPos, rotation);
    }

    void spawnFinish()
    {
        Vector3 EndPos = new Vector3(MazeGridX-2, 0.125f, MazeGridY-2);
        Quaternion rotation = Quaternion.identity;
        GameObject InstantiateEnding = Instantiate(Finish, EndPos, rotation);
    }

    void clearMines()
    {
        MineGrid[1, 1].contents = MazeCell.Start;
        MineGrid[1, 2].contents = MazeCell.Empty;
        MineGrid[2, 1].contents = MazeCell.Empty;
        MineGrid[2, 2].contents = MazeCell.Empty;
        
        MineGrid[MazeGridX-2, MazeGridY-2].contents = MazeCell.End;
        MineGrid[MazeGridX-2, MazeGridY-3].contents = MazeCell.Empty;
        MineGrid[MazeGridX-3, MazeGridY-2].contents = MazeCell.Empty;
        MineGrid[MazeGridX-3, MazeGridY-3].contents = MazeCell.Empty;
    }

    public void restartPath()
    {
        for (int x = 1; x < MazeGridX-1; x++)
        {
            for (int y = 1; y < MazeGridY-1; y++)
            {
                if (MazeGrid[x, y].contents == MazeCell.Traversed)
                {
                    MazeGrid[x, y].contents = MazeCell.Visited;
                }
            }
        }
    }

    public void BlowUpCube()
    {
        GameObject Cube = GameObject.FindGameObjectWithTag("Player");
        
        spawnParticleCubes(Cube.transform.position.x, Cube.transform.position.y, Cube.transform.position.z);

        Destroy(Cube);
        cubeDestroyed = true;
        animationWait = 0;
    }

    public void cleanUpCubes()
    {
        GameObject[] littleCubes = GameObject.FindGameObjectsWithTag("PlayerParticle");

        foreach (GameObject obj in littleCubes)
        {
            Destroy(obj);
        }
    }

    public void spawnParticleCubes(float x, float y, float z)
    {
        for (int i = 0; i<3; i++)
        {
            for (int j = 0; j<3; j++)
            {
                for (int p = 0; p<3; p++)
                {
                    
                    random = new System.Random();
                    int offsetX = random.Next(-1, 1);
                    int offsetZ = random.Next(-1, 1);
                    
                    Vector3 particlePos = new Vector3(x+(i*0.3f)-0.125f+((float)offsetX * 0.2f), y+(j*0.3f), z+(p*0.3f)-0.125f+((float)offsetZ * 0.2f));
                    Quaternion rotation = Quaternion.identity;
                    GameObject InstantiateEnding = Instantiate(cubeParticle, particlePos, rotation);
                }
            }
        }
    }

    public void winninCondition()
    {
        alphaUp = true;
        blackScreenSequence = true;
        nextLevel = true;
    }

}
