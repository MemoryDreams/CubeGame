using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SystemController : MonoBehaviour
{
    public bool paused = true;
    public MazeGenerator MazeGenerator;
    public PlayerScript player;
    public TextMeshProUGUI pauseText;
    public GameObject restartButton;
    public GameObject pauseScreen;
    public GameObject gameUI;

    private void Update()
    {
        components();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePressed();
        }
    }

    void components()
    {
        if (player==null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        }
    }

    public void pausePressed()
    {
            if (paused)
            {
                paused = false;
                pauseScreen.SetActive(false);
                gameUI.SetActive(true);
                MazeGenerator.alphaUp = false;
                MazeGenerator.blackScreenSequence = true;
                //player.moving = false;
                //Time.timeScale = 0;
            }
            else 
            {
                paused = true;
                pauseScreen.SetActive(true);
                gameUI.SetActive(false);
                restartButton.SetActive(true);
                pauseText.text = "Continue?";
                MazeGenerator.alphaUp = true;
                MazeGenerator.blackScreenSequence = true;
                player.moving = false;
                player.startCounter = 1;
                //Time.timeScale = 1;
            }
    }
}
