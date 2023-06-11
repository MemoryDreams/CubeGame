using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ButtonsScript : MonoBehaviour
{
    public PlayerScript playerScript;
    public SystemController systemController; 
    public Image fill;
    public bool shieldPressed;
    public bool isButtonPressed;

    void Update()
    {
        components();
        if (playerScript!=null)
        {
            fill.fillAmount = playerScript.shieldCooldown * 0.5f;
            if (isButtonPressed)
            {
                pressShield();
            }
            else
            {
                playerScript.shieldCooldown = playerScript.shieldLimit;
                playerScript.shielded = false;
                playerScript.Mesh.material = playerScript.unshieldedPlayer;
            }
        }
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
                    // Perform the action for when the button is pressed
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

    public void pressShield()
    {
        playerScript.shieldController();
        shieldPressed = true;
    }

    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     isButtonPressed = true;
    //     // Perform the action for when the button is pressed
    // }

    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     isButtonPressed = false;
    //     // Perform the action for when the button is released
    // }

    private void components()
    {
        if (playerScript==null)
        {
            isButtonPressed = false;
            playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        }
        if (systemController==null)
        {
            systemController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SystemController>();
        }
    }
}
