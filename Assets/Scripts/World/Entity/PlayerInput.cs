using Assets.Scripts.GameMenu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public bool isKeyboard = false;

    private Uxml_GameMenu gameMenu;

    private void Start()
    {
        gameMenu = FindAnyObjectByType<Uxml_GameMenu>();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (isKeyboard)
                return;

            isKeyboard = true;
            //Debug.Log("[Input manager]: Keyboard input detected, input mode switched to 'Keyboard' mode.");
        }
        else
        {
            if (!isKeyboard)
                return;

            isKeyboard = false;
            //Debug.Log("[Input manager]: Gamepad input detected, input mode switched to 'Gamepad' mode.");
        }
    }

    public float GetVerticalInput()
    {
        return Input.GetAxis("Vertical");
    }

    public float GetHorizontalInput()
    {
        return Input.GetAxis("Horizontal");
    }

    public bool isPressingUp()
    {
        // RETURNS UP BUTTON STATE
        return Input.GetKey(KeyCode.W);
    }

    public bool isPressingDown()
    {
        // RETURNS DOWN BUTTON STATE
        return Input.GetKey(KeyCode.S);
    }

    public bool isPressingLeft()
    {
        // RETURNS LEFT BUTTON STATE
        return Input.GetKey(KeyCode.A);
    }

    public bool isPressingRight()
    {
        // RETURNS RIGHT BUTTON STATE
        return Input.GetKey(KeyCode.D);
    }

    public bool isPressingTurnButton()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }

    public bool isPressingMenuButton()
    {
        return Input.GetKey(KeyCode.Escape);
    }

    public bool isGameMenuOpen()
    { 
        return gameMenu.open;
    }
}
