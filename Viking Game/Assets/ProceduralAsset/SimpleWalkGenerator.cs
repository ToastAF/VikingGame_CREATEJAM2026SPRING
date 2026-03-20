using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleWalkGenerator : AbstractDungeonGenerator
{

    [SerializeField]
    private int iterations = 10;
    [SerializeField]
    public int walkLength = 10;
    [SerializeField]
    public bool startRandomly = true;


    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk();
        floorVisualiser.Clear();
        floorVisualiser.PaintFloorTiles(floorPositions);

    }

    protected HashSet<Vector2Int> RunRandomWalk()
    {
        var currentPosition = startPosition;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < iterations; i++)
        {
            var path = Procedural.simpleRandomWalk(currentPosition, walkLength);
            floorPositions.UnionWith(path);
            if (startRandomly)
            {
                currentPosition = floorPositions.ElementAt(UnityEngine.Random.Range(0, floorPositions.Count));
            }
        }
        return floorPositions;
    }

}
