using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class inputProvider
{
    private static PlayerInput input = new();

    public void Enable()
    {
        input.Player.walk.Enable();
    }
    public void Disable()
    {
        input.Player.walk.Disable();
    }
    /* Walk */
    public Vector2 Walk()
    {
        return input.Player.walk.ReadValue<Vector2>();
    }
    public Vector2 MousePos()
    {
        return input.Player.mousePos.ReadValue<Vector2>();
    }
}
