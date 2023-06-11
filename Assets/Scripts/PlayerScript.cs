using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public MazeGenerator MazeGenerator;
    public int speed;
    bool outOfBounds;
    private Vector3 target = new Vector3(1,0.125f,1);
    public bool targetReached = false;
    public bool moving = false;
    public bool shielded = false;
    public bool inMine = false;
    public float waitCounter = 0;
    public float waitLimit = 0.05f;
    public float startCounter = 0;
    public float startLimit = 2;
    public float shieldCooldown = 2;
    public float shieldLimit = 2;
    public MeshRenderer Mesh;
    public Material shieldedPlayer;
    public Material unshieldedPlayer;
    public ParticleSystemController particle1;
    public ParticleSystemController particle2;
    public ParticleSystemController particle3;
    public bool hasWon;
    public SystemController SystemController;
    

    void Awake()
    {
        SystemController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SystemController>();
        MazeGenerator = GameObject.FindGameObjectWithTag("MazeGrid").GetComponent<MazeGenerator>();
        Mesh = gameObject.GetComponent<MeshRenderer>();
    }

    void Start()
    {
        startCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!SystemController.paused)
        {
            if (!hasWon)
            {
                if (inMine)
                {
                    if (!shielded) {
                        restartMaze();
                    }
                }
                if (moving)
                {
                    // if (Input.GetKey(KeyCode.Space))
                    // {
                    //     shieldController();
                    // }
                    // else
                    // {
                    //     shieldCooldown = 0;
                    //     shielded = false;
                    //     Mesh.material = unshieldedPlayer;
                    // }
                    if (MazeGenerator.MazeGrid[Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)].contents == MazeGenerator.MazeCell.End)
                    {
                        Winning();
                    }
                    if ((Vector3.Distance(transform.position, target) < 0.1f) && !targetReached)
                    {   
                        targetReached = true;
                        FindPath(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
                    }
                    else
                    {
                        moveCube();
                        if ((Vector3.Distance(transform.position, target) > 0.1f))
                        {
                            targetReached = false;
                        }   
                    }
                }
                else
                {
                    if (startCounter>startLimit)
                    {
                        startCounter = 0f;
                        moving=true;
                    } else {
                        startCounter += Time.deltaTime;
                    }
                }
            } 
            else
            {
                startCounter += Time.deltaTime;
                if (startCounter>startLimit)
                {
                    startCounter = 0f;
                    MazeGenerator.winninCondition();
                }
            }
        }
    }

    public void shieldController()
    {
        if (shieldCooldown > 0)
        {
            shieldCooldown-=Time.deltaTime;
            shielded = true;
            Mesh.material = shieldedPlayer;
        } 
        else
        {
            shielded = false;
            Mesh.material = unshieldedPlayer;
        }
    }

    void moveCube()
    {
        Vector3 direction = (target - transform.position).normalized;

        transform.Translate(direction * speed * Time.deltaTime);
    }

    void FindPath(int x, int y)
    {
        
        outOfBounds = (x > MazeGenerator.MazeGridX - 1 || y > MazeGenerator.MazeGridY - 1 || x < 1 || y < 1);
        MazeGenerator.MazeGrid[x, y].contents = MazeGenerator.MazeCell.Traversed;
        // if (MazeGenerator.MazeGrid[x, y].contents == MazeGenerator.MazeCell.End)
        // {
        //     ProperMaze = true;
        //     Debug.Log("Path found!"); // Exit found, do something
        //     return;
        // }
        if (!outOfBounds && (MazeGenerator.MazeGrid[x, y + 1].contents == MazeGenerator.MazeCell.Visited || MazeGenerator.MazeGrid[x, y + 1].contents == MazeGenerator.MazeCell.End || MazeGenerator.MazeGrid[x, y + 1].contents == MazeGenerator.MazeCell.Mine))
        {
            y=y + 1;
        } else if (!outOfBounds && (MazeGenerator.MazeGrid[x + 1, y].contents == MazeGenerator.MazeCell.Visited || MazeGenerator.MazeGrid[x + 1, y].contents == MazeGenerator.MazeCell.End || MazeGenerator.MazeGrid[x + 1, y].contents == MazeGenerator.MazeCell.Mine))
        {
            x=x + 1;
        } else if (!outOfBounds && (MazeGenerator.MazeGrid[x - 1, y].contents == MazeGenerator.MazeCell.Visited || MazeGenerator.MazeGrid[x - 1, y].contents == MazeGenerator.MazeCell.End || MazeGenerator.MazeGrid[x - 1, y].contents == MazeGenerator.MazeCell.Mine))
        {
            x=x - 1;
        } else if (!outOfBounds && (MazeGenerator.MazeGrid[x, y - 1].contents == MazeGenerator.MazeCell.Visited || MazeGenerator.MazeGrid[x, y - 1].contents == MazeGenerator.MazeCell.End || MazeGenerator.MazeGrid[x, y - 1].contents == MazeGenerator.MazeCell.Mine))
        {
            y=y - 1;
        }
        target = new Vector3(x, transform.position.y, y);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mine"))
        {
            inMine = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Mine"))
        {
            inMine = false;
        }
    }

    void restartMaze()
    {
        MazeGenerator.restartPath();
        MazeGenerator.BlowUpCube();
    }

    void Winning()
    {
        hasWon = true;
        moving = false;
        particle1.turnOn();
        particle1.turnedOn = true;
        particle2.turnOn();
        particle2.turnedOn = true;
        particle3.turnOn();
        particle3.turnedOn = true;
        Debug.Log("You win!");
        startCounter = 0;
    }
}
