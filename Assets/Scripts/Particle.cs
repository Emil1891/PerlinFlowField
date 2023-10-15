using UnityEngine;

public class Particle : MonoBehaviour
{
    [Header("Design")]
    
    [SerializeField] private Color startColor = Color.white;
    [SerializeField] private Color endColor = Color.white;

    [SerializeField] private SpriteRenderer ren;

    // If a random color should be assigned between start and end color 
    [SerializeField] private bool randomStartColorBetweenStartEnd = false; 
    
    // Gives a totally random color 
    [SerializeField] private bool totallyRandomStartColor = false; 
    
    // If the color should lerp between start and end 
    [SerializeField] private bool lerpColor = true;

    // How much color lerp should increment each second 
    [Range(0f, 0.5f)] 
    [SerializeField] private float colorLerpSpeed = 0.1f; 
    
    [Header("Movement")]
    
    [SerializeField] private float maxSpeed = 0.5f;
    
    [SerializeField] private float acceleration = 1f; 
    
    private Vector3 velocity = Vector2.zero;
    
    private static MapGrid grid; 

    private Camera cam;

    // Screen bounds 
    private float xLeftBound, xRightBound, yBottomBound, yTopBound;

    private float colorLerpValue = 0f; 
    
    private void Start()
    {
        if(!grid)
            grid = FindObjectOfType<MapGrid>(); 
        
        cam = Camera.main;

        // Set camera bounds so particles can "loop" the screen 
        xLeftBound = cam.ViewportToWorldPoint(Vector3.zero).x;
        xRightBound = cam.ViewportToWorldPoint(Vector3.right).x;
        yBottomBound = cam.ViewportToWorldPoint(Vector3.zero).y;
        yTopBound = cam.ViewportToWorldPoint(Vector3.up).y;

        if (totallyRandomStartColor)
        {
            ren.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)); 
        }
        else if (randomStartColorBetweenStartEnd)
        {
            colorLerpValue = Random.Range(0f, 1f); // Set start lerp value to match the randomly assigned color 
            ren.color = Color.Lerp(startColor, endColor, colorLerpValue);
        } else 
            ren.color = startColor;
    }

    private void Update()
    {
        UpdatePosition(); 

        UpdateColor(); 
        
        CheckScreenBounds(); 
    }

    private void UpdatePosition()
    {
        Vector3 direction = grid.GetNodeFromWorldPoint(transform.position).Direction;

        velocity += direction * (acceleration * Time.deltaTime); 
        
        // clamp velocity (add friction/viscosity instead?)
        if (velocity.magnitude > maxSpeed)
            velocity = velocity.normalized * maxSpeed;

        transform.position += velocity * Time.deltaTime;
    }

    private void UpdateColor()
    {
        if (!lerpColor)
            return;

        ren.color = Color.Lerp(startColor, endColor, colorLerpValue);

        colorLerpValue += colorLerpSpeed * Time.deltaTime; 
        
        // If we have achieved the end color, then it should reverse 
        if (colorLerpValue >= 1)
        {
            colorLerpValue = 0;
            // Fancy C# swap without a temp variable 
            (startColor, endColor) = (endColor, startColor);
        }
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
