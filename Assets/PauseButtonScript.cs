using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButtonScript : MonoBehaviour
{
    private SystemController systemController;
    public bool isButtonPressed;

    void Update()
    {
        components();
        if (systemController != null)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    // Check if the touch position overlaps with the button's RectTransform
                    if (!isButtonPressed && RectTransformUtility.RectangleContainsScreenPoint(
                        (RectTransform)transform, touch.position, null))
                    {
                        Debug.Log("Pause pressed!");
                        isButtonPressed = true;
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
                    if (isButtonPressed && RectTransformUtility.RectangleContainsScreenPoint(
                        (RectTransform)transform, touch.position, null))
                    {
                        isButtonPressed = false;
                        // Perform the action for when the button is released
                    }
                }
            }
        }
    }

    private void components()
    {
        if (systemController == null)
        {
            systemController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SystemController>();
        }
    }
}
