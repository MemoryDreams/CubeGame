using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButtonScript : MonoBehaviour
{
    private MazeGenerator mazeGenerator;
    private SystemController systemController;
    private bool isButtonPressed = false;

    // Update is called once per frame
    void Update()
    {
        components();
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Check if the touch position overlaps with the button's RectTransform
                if (RectTransformUtility.RectangleContainsScreenPoint(
                    (RectTransform)transform, touch.position, null))
                {
                    isButtonPressed = true;
                    mazeGenerator.restartGame();
                    systemController.pausePressed();
                    
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (!isButtonPressed)
                {
                    return; // Ignore touch release if the button was not previously pressed
                }

                // Check if the touch position overlaps with the button's RectTransform
                if (RectTransformUtility.RectangleContainsScreenPoint(
                    (RectTransform)transform, touch.position, null))
                {
                    isButtonPressed = false;
                    // Perform the action for when the button is released
                }
            }
        }
    }

    void components()
    {
        if (mazeGenerator == null)
        {
            mazeGenerator = GameObject.FindGameObjectWithTag("MazeGrid").GetComponent<MazeGenerator>();
        }
        if (systemController == null)
        {
            systemController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SystemController>();
        }
    }
}
