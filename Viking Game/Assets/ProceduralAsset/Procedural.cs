using System.Collections.Generic;
using UnityEngine;

public class Procedural : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static HashSet<Vector2Int> simpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        path.Add(startPosition);
        Vector2Int previousPosition = startPosition;
        
        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }
        return path;
    }
}

public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), // Up
        new Vector2Int(1, 0), // Right
        new Vector2Int(0, -1), // Down
        new Vector2Int(-1, 0) // Left
    };
    public static Vector2Int GetRandomDirection()
    {
        int randomIndex = Random.Range(0, cardinalDirectionsList.Count);
        return cardinalDirectionsList[randomIndex];
    }
}
