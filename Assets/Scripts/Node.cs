using UnityEngine;

public class Node 
{
    public Vector2 WorldCoordinate { get; }

    public int GridX { get; }
    
    public int GridY { get; }
    
    public Vector2 Direction { get; set;  } 

    public Node(Vector2 worldCoordinate, int gridX, int gridY)
    {
        WorldCoordinate = worldCoordinate;
        GridX = gridX;
        GridY = gridY;
    }
}
