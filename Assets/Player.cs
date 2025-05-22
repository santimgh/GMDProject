using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 movement;
    private float speed = 5;
    public TextMeshPro text;

    void Update()
    {
        transform.Translate(movement * speed * Time.deltaTime);
    }
    
    public void OnMovement(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    
    public void OnA()
    {
        text.text = "A";
    }
    
    public void OnB()
    {
        text.text = "B";
    }
    
    public void OnX()
    {
        text.text = "X";
    }
    
    public void OnY()
    {
        text.text = "Y";
    }
    
    public void OnLeftTrigger()
    {
        text.text = "L Trigger";
    }
    
    public void OnRightTrigger()
    {
        text.text = "R Trigger";
    }
    
    public void OnStart()
    {
        text.text = "Start";
    }
}
