﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public Vector2 InputVector { get; private set; }

    public Vector3 MousePosition { get; private set; }

    public bool isChangeMenuOpen = false;
    public bool isInGamePaused = false;
    // Update is called once per frame
    void Update()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        InputVector = new Vector2(h, v);

        MousePosition = Input.mousePosition;
        
        //this was previously just Space which wouldn't work and was causing an error, so I changed it to "Jump" - John
        if (Input.GetButtonDown("Jump"))
        {
           if (!isInGamePaused)
           {
                pauseInGame();
           }
           else
           {
                unpauseInGame();
           }
        }

    }

    private void pauseInGame()
    {

    }

    private void unpauseInGame()
    {

    }
}
