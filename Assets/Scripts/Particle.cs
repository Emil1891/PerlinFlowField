using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    private MapGrid Grid;

    private Vector3 velocity = Vector2.zero;

    [SerializeField] private float acceleration = 1f; 
    
    [SerializeField] private float maxSpeed = 5f;

    private Camera cam; 

    private void Start()
    {
        Grid = FindObjectOfType<MapGrid>(); 
        
        cam = Camera.main;
    }

    private void Update()
    {
        Vector3 direction = Grid.GetNodeFromWorldPoint(transform.position).Direction;

        velocity += direction * acceleration;
        
        // clamp velocity (add friction instead?)
        if (velocity.magnitude > maxSpeed)
            velocity = velocity.normalized * maxSpeed; 
        
        transform.position += velocity * Time.deltaTime; 
        
        CheckScreenBounds(); 
    }

    // Adjusts the particle's position if outside the screen to "loop" it 
    private void CheckScreenBounds()
    {
        // Ranges from 0 to 1 so off screen < 0 and > 1
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);

        Vector3 newPos = transform.position; 
        
        if (viewportPos.x > 1)
            newPos.x = cam.ViewportToWorldPoint(Vector3.zero).x; // Only after the world coord at 0 viewport 
        else if (viewportPos.x < 0)
            newPos.x = cam.ViewportToWorldPoint(Vector3.right).x; 
        
        if (viewportPos.y > 1)
            newPos.y = cam.ViewportToWorldPoint(Vector3.zero).y; 
        else if(viewportPos.y < 0)
            newPos.y = cam.ViewportToWorldPoint(Vector3.up).y; 
        
        transform.position = newPos;
    }
}
