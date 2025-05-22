using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // Assign the Player in the Inspector
    public float smoothSpeed = 5f; // Adjust camera smoothness
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Default 2D camera offset

    void LateUpdate()
    {
        if (player == null) return; // Prevent errors if no player is assigned

        // Smoothly interpolate camera position towards player's position
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}

