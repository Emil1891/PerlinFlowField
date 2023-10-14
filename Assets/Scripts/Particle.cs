using UnityEngine;

public class Particle : MonoBehaviour
{
    private MapGrid grid;

    private Vector3 velocity = Vector2.zero;

    [SerializeField] private float acceleration = 1f; 
    
    [SerializeField] private float maxSpeed = 5f;

    private Camera cam;

    private float xLeftBound, xRightBound, yBottomBound, yTopBound; 

    private void Start()
    {
        grid = FindObjectOfType<MapGrid>(); 
        
        cam = Camera.main;

        // Set camera bounds so particles can "loop" the screen 
        xLeftBound = cam.ViewportToWorldPoint(Vector3.zero).x;
        xRightBound = cam.ViewportToWorldPoint(Vector3.right).x;
        yBottomBound = cam.ViewportToWorldPoint(Vector3.zero).y;
        yTopBound = cam.ViewportToWorldPoint(Vector3.up).y;
    }

    private void Update()
    {
        Vector3 direction = grid.GetNodeFromWorldPoint(transform.position).Direction;

        velocity += direction * (acceleration * Time.deltaTime); 
        
        // clamp velocity (add friction/viscosity instead?)
        if (velocity.magnitude > maxSpeed)
            velocity = velocity.normalized * maxSpeed;

        transform.position += velocity * Time.deltaTime; 
        
        CheckScreenBounds(); 
    }

    // Adjusts the particle's position if outside the screen to "loop" it 
    private void CheckScreenBounds()
    {
        // Ranges from 0 to 1 so off screen < 0 and > 1
        Vector3 startPos = transform.position;
        Vector3 viewportPos = cam.WorldToViewportPoint(startPos);
        
        Vector3 newPos = startPos;
        
        if (viewportPos.x > 1)
            newPos.x = xLeftBound; 
        else if (viewportPos.x < 0)
            newPos.x = xRightBound; 
        
        if (viewportPos.y > 1)
            newPos.y = yBottomBound; 
        else if(viewportPos.y < 0)
            newPos.y = yTopBound; 
        
        transform.position = newPos;
    }
}
