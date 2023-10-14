using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    [SerializeField] private Vector2 gridSize; // (x, y) 

    [SerializeField] private float nodeRadius; // radius of each node
    private float nodeDiameter;

    private int gridXLength, gridYLength; 

    private Node[,] grid;

    private Vector3 gridBottomLeft;

    // A higher value results in more drastic direction changes between nearby nodes 
    [Range(1, 15)] 
    [SerializeField] private float directionChangeMultiplier = 8; 

    private void Start()
    {
        nodeDiameter = nodeRadius * 2; 
        gridXLength = Mathf.RoundToInt(gridSize.x / nodeDiameter); 
        gridYLength = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        CreateGrid(); 
    }

    private void Update()
    {
        foreach (Node node in grid)
        {
            node.Direction = GetNodeDirection(node.GridX, node.GridY);
        }
    }

    private void CreateGrid()
    {
        grid = new Node[gridXLength, gridYLength];
        gridBottomLeft = transform.position; // begins in the middle 
        gridBottomLeft.x -= gridSize.x / 2; 
        gridBottomLeft.y -= gridSize.y / 2; 

        for(int x = 0; x < gridXLength; x++)
        {
            for(int y = 0; y < gridYLength; y++)
            {
                Vector2 nodeWorldCoordinate = gridBottomLeft;
                nodeWorldCoordinate.x += x * nodeDiameter + nodeRadius;  
                nodeWorldCoordinate.y += y * nodeDiameter + nodeRadius; // coordinate is now in the middle of the node

                var node = new Node(nodeWorldCoordinate, x, y);
                grid[x, y] = node;

                node.Direction = GetNodeDirection(x, y);
            }
        }
    }

    private Vector2 GetNodeDirection(int x, int y)
    {
        // Reworked from: https://youtu.be/na7LuZsW2UM?si=9KrNbO8zcOwoBz33 
        float perlinNoise = Mathf.PerlinNoise(x / (float)gridXLength, y / (float)gridYLength) * Mathf.PI * directionChangeMultiplier; 
        //Debug.Log($"Perlin noise value: {perlinNoise}"); 

        return new Vector2(Mathf.Cos(perlinNoise), Mathf.Sin(perlinNoise)).normalized; 
    }

    public Node GetNodeFromWorldPoint(Vector3 worldPoint)
    {
        // Get the position relative to the grid's bottom left corner 
        float gridRelativeX = worldPoint.x  - gridBottomLeft.x; 
        float gridRelativeY = worldPoint.y - gridBottomLeft.y;

        // Check how many nodes "fit" in the relative position to calculate the array indexes 
        int x = Mathf.Clamp(Mathf.RoundToInt((gridRelativeX / nodeDiameter) - nodeRadius), 0, gridXLength - 1); 
        int y = Mathf.Clamp(Mathf.RoundToInt((gridRelativeY / nodeDiameter) - nodeRadius), 0, gridYLength - 1);

        return grid[x, y];
    }

    public HashSet<Node> GetNeighbours(Node node)
    {
        HashSet<Node> neighbours = new();

        for(int x = -1; x <= 1; x++) // 1 step left, middle and 1 step right 
        {
            for(int y = -1; y <= 1; y++) // 1 step down, middle and 1 step up 
            {
                if (x == 0 && y == 0) // itself 
                    continue;

                int xCoord = node.GridX + x; // grid indexes 
                int yCoord = node.GridY + y;

                // if coordinate is within array bounds 
                if (xCoord > 0 && xCoord < gridXLength && yCoord > 0 && yCoord < gridYLength)
                    neighbours.Add(grid[xCoord, yCoord]); 
            }
        }

        return neighbours; 
    }

    public Node GetNode(int x, int y)
    {
        return grid[x, y];
    }

    // DEBUGGING
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 1)); 

        if (grid == null)
            return;

        foreach (Node node in grid)
        {
            Gizmos.color = Color.black;
            
            // Draw node 
            Gizmos.DrawWireCube(node.WorldCoordinate, new Vector3(nodeDiameter, nodeDiameter, 1));
            
            // Draw direction 
            Gizmos.DrawLine(node.WorldCoordinate - node.Direction * nodeRadius, node.WorldCoordinate + node.Direction * nodeRadius);
        } 
    }
    
}
