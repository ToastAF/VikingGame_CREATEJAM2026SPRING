using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomFirstDungeonGenerator : SimpleWalkGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;

    [SerializeField]
    [Range(0,10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;

    [SerializeField] private Grid grid;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject exitPrefab;
    private GameObject spawnedExit;

    [SerializeField] private GameObject middlePrefab;
    private GameObject spawnedMiddle;

    [SerializeField] private int corridorBrushSize = 3;

    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] private int minEnemiesPerRoom = 4;
    [SerializeField] private int maxEnemiesPerRoom = 8;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    [SerializeField] private List<GameObject> decorationPrefabs = new List<GameObject>();
    [SerializeField] private int decorationCount = 25;

    private List<GameObject> spawnedDecorations = new List<GameObject>();

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        if (spawnedExit != null)
        {
        #if UNITY_EDITOR
            DestroyImmediate(spawnedExit);
        #else
            Destroy(spawnedExit);
        #endif
            spawnedExit = null;
        }

        if (spawnedMiddle != null)
        {
        #if UNITY_EDITOR
            DestroyImmediate(spawnedMiddle);
        #else
            Destroy(spawnedMiddle);
        #endif
            spawnedMiddle = null;
        }

        if (spawnedEnemies.Count > 0)
        {
            foreach (GameObject enemy in spawnedEnemies)
            {
                if (enemy != null)
                {
                #if UNITY_EDITOR
                    DestroyImmediate(enemy);
            #else
                Destroy(enemy);
            #endif
                }
            }
            spawnedEnemies.Clear();
        }

        if (spawnedDecorations.Count > 0)
        {
            foreach (GameObject decoration in spawnedDecorations)
            {
                if (decoration != null)
                {
            #if UNITY_EDITOR
                    DestroyImmediate(decoration);
            #else
                Destroy(decoration);
            #endif
                }
            }
            spawnedDecorations.Clear();
        }

        var roomsList = Procedural.BinarySpacePartioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)),
            minRoomWidth,
            minRoomHeight);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        if (randomWalkRooms)
            floor = CreateRoomsRandomly(roomsList);
        else
            floor = CreateSimpleRooms(roomsList);

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add(Vector2Int.RoundToInt(room.center));
        }

        Vector2Int startRoomCenter = roomCenters[0];

        Dictionary<Vector2Int, List<Vector2Int>> roomGraph = new Dictionary<Vector2Int, List<Vector2Int>>();
        HashSet<Vector2Int> corridors = ConnectRooms(new List<Vector2Int>(roomCenters), roomGraph);

        floor.UnionWith(corridors);

        floorVisualiser.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, floorVisualiser);

        SpawnPlayerInFirstRoom(roomsList);

        List<Vector2Int> pathToFurthest = FindPathToFurthestRoom(startRoomCenter, roomGraph);
        Vector2Int furthestRoom = pathToFurthest[pathToFurthest.Count - 1];
        Vector2Int middleRoom = FindMiddleRoomOnPath(pathToFurthest);

        SpawnMiddle(middleRoom);
        SpawnExit(furthestRoom);

        BoundsInt playerBoundsRoom = roomsList[0];
        BoundsInt middleBoundsRoom = FindRoomContainingPosition(roomsList, middleRoom);
        BoundsInt exitBoundsRoom = FindRoomContainingPosition(roomsList, furthestRoom);
        SpawnEnemiesInRooms(roomsList, floor, playerBoundsRoom, middleBoundsRoom, exitBoundsRoom);
        SpawnDecorations(floor);
    }

    private HashSet<Vector2Int> IncreaseCorridorBrushSize(List<Vector2Int> corridor)
    {
        HashSet<Vector2Int> newCorridor = new HashSet<Vector2Int>();

        int radius = corridorBrushSize / 2;

        foreach (var position in corridor)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    newCorridor.Add(position + new Vector2Int(x, y));
                }
            }
        }

        return newCorridor;
    }



    private void AddConnection(Dictionary<Vector2Int, List<Vector2Int>> graph, Vector2Int a, Vector2Int b)
    {
        if (!graph.ContainsKey(a))
            graph[a] = new List<Vector2Int>();

        if (!graph.ContainsKey(b))
            graph[b] = new List<Vector2Int>();

        if (!graph[a].Contains(b))
            graph[a].Add(b);

        if (!graph[b].Contains(a))
            graph[b].Add(a);
    }

    private void SpawnPlayerInFirstRoom(List<BoundsInt> roomsList)
    {
        if (roomsList == null || roomsList.Count == 0 || grid == null || playerTransform == null)
            return;

        BoundsInt firstRoom = roomsList[0];

        int centerX = firstRoom.xMin + firstRoom.size.x / 2;
        int centerY = firstRoom.yMin + firstRoom.size.y / 2;

        Vector3Int cellPosition = new Vector3Int(centerX, centerY, 0);
        Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);
        spawnPosition.z = 0f;

        Debug.Log("First room: " + firstRoom);
        Debug.Log("Cell position: " + cellPosition);
        Debug.Log("World position: " + spawnPosition);

        playerTransform.position = spawnPosition;
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0;i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) &&
                    position.y >= (roomBounds.yMin + offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters, Dictionary<Vector2Int, List<Vector2Int>> graph)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();

        var currentRoomCenter = roomCenters[0];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);

            List<Vector2Int> corridor = CreateCorridor(currentRoomCenter, closest);
            HashSet<Vector2Int> widenedCorridor = IncreaseCorridorBrushSize(corridor);
            corridors.UnionWith(widenedCorridor);

            AddConnection(graph, currentRoomCenter, closest);

            currentRoomCenter = closest;
        }

        return corridors;
    }

    private Vector2Int FindFurthestRoom(Vector2Int startRoom, Dictionary<Vector2Int, List<Vector2Int>> graph)
    {
        Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();
        List<Vector2Int> unvisited = new List<Vector2Int>();

        foreach (var room in graph.Keys)
        {
            distances[room] = int.MaxValue;
            unvisited.Add(room);
        }

        distances[startRoom] = 0;

        while (unvisited.Count > 0)
        {
            Vector2Int current = unvisited[0];
            foreach (var room in unvisited)
            {
                if (distances[room] < distances[current])
                    current = room;
            }

            unvisited.Remove(current);

            if (distances[current] == int.MaxValue)
                break;

            foreach (var neighbor in graph[current])
            {
                int newDistance = distances[current] + 1; // each corridor connection costs 1
                if (newDistance < distances[neighbor])
                {
                    distances[neighbor] = newDistance;
                }
            }
        }

        Vector2Int furthestRoom = startRoom;
        int maxDistance = 0;

        foreach (var pair in distances)
        {
            if (pair.Value != int.MaxValue && pair.Value > maxDistance)
            {
                maxDistance = pair.Value;
                furthestRoom = pair.Key;
            }
        }

        return furthestRoom;
    }
    private BoundsInt FindRoomContainingPosition(List<BoundsInt> roomsList, Vector2Int position)
    {
        foreach (var room in roomsList)
        {
            if (position.x >= room.xMin && position.x < room.xMax &&
                position.y >= room.yMin && position.y < room.yMax)
            {
                return room;
            }
        }

        return new BoundsInt();
    }

    private void SpawnDecorations(HashSet<Vector2Int> floor)
    {
        if (decorationPrefabs == null || decorationPrefabs.Count == 0 || grid == null || floor == null || floor.Count == 0)
            return;

        List<Vector2Int> availableTiles = new List<Vector2Int>(floor);

        int spawnCount = Mathf.Min(decorationCount, availableTiles.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            int randomTileIndex = UnityEngine.Random.Range(0, availableTiles.Count);
            Vector2Int spawnCell = availableTiles[randomTileIndex];
            availableTiles.RemoveAt(randomTileIndex);

            GameObject decorationPrefab = decorationPrefabs[UnityEngine.Random.Range(0, decorationPrefabs.Count)];
            if (decorationPrefab == null)
                continue;

            Vector3Int cellPosition = new Vector3Int(spawnCell.x, spawnCell.y, 0);
            Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);
            spawnPosition.z = 0f;

            GameObject spawnedDecoration = Instantiate(decorationPrefab, spawnPosition, Quaternion.identity);
            spawnedDecoration.name = decorationPrefab.name;

            spawnedDecorations.Add(spawnedDecoration);
        }
    }
    private void SpawnEnemiesInRooms(List<BoundsInt> roomsList, HashSet<Vector2Int> floor, BoundsInt playerRoom, BoundsInt middleRoom, BoundsInt exitRoom)
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0 || grid == null || floor == null)
            return;

        foreach (BoundsInt room in roomsList)
        {
            if (room == playerRoom || room == middleRoom || room == exitRoom)
                continue;

            List<Vector2Int> validSpawnTiles = new List<Vector2Int>();

            for (int x = room.xMin + offset; x < room.xMax - offset; x++)
            {
                for (int y = room.yMin + offset; y < room.yMax - offset; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    if (!floor.Contains(pos))
                        continue;

                    validSpawnTiles.Add(pos);
                }
            }

            if (validSpawnTiles.Count == 0)
                continue;

            int enemyCount = UnityEngine.Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);
            enemyCount = Mathf.Min(enemyCount, validSpawnTiles.Count);

            for (int i = 0; i < enemyCount; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, validSpawnTiles.Count);
                Vector2Int spawnCell = validSpawnTiles[randomIndex];
                validSpawnTiles.RemoveAt(randomIndex);

                GameObject enemyPrefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)];
                if (enemyPrefab == null)
                    continue;

                Vector3Int cellPosition = new Vector3Int(spawnCell.x, spawnCell.y, 0);
                Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);
                spawnPosition.z = 0f;

                GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                spawnedEnemy.name = enemyPrefab.name;

                spawnedEnemies.Add(spawnedEnemy);
            }
        }
    }
    private List<Vector2Int> FindPathToFurthestRoom(Vector2Int startRoom, Dictionary<Vector2Int, List<Vector2Int>> graph)
    {
        Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, Vector2Int> previous = new Dictionary<Vector2Int, Vector2Int>();
        List<Vector2Int> unvisited = new List<Vector2Int>();

        foreach (var room in graph.Keys)
        {
            distances[room] = int.MaxValue;
            unvisited.Add(room);
        }

        if (!graph.ContainsKey(startRoom))
            return new List<Vector2Int> { startRoom };

        distances[startRoom] = 0;

        while (unvisited.Count > 0)
        {
            Vector2Int current = unvisited[0];
            foreach (var room in unvisited)
            {
                if (distances[room] < distances[current])
                    current = room;
            }

            unvisited.Remove(current);

            if (distances[current] == int.MaxValue)
                break;

            foreach (var neighbor in graph[current])
            {
                int newDistance = distances[current] + 1;
                if (newDistance < distances[neighbor])
                {
                    distances[neighbor] = newDistance;
                    previous[neighbor] = current;
                }
            }
        }

        Vector2Int furthestRoom = startRoom;
        int maxDistance = 0;

        foreach (var pair in distances)
        {
            if (pair.Value != int.MaxValue && pair.Value > maxDistance)
            {
                maxDistance = pair.Value;
                furthestRoom = pair.Key;
            }
        }

        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int pathCurrent = furthestRoom;
        path.Add(pathCurrent);

        while (previous.ContainsKey(pathCurrent))
        {
            pathCurrent = previous[pathCurrent];
            path.Add(pathCurrent);
        }

        path.Reverse();
        return path;
    }
    private Vector2Int FindMiddleRoomOnPath(List<Vector2Int> path)
    {
        if (path == null || path.Count == 0)
            return Vector2Int.zero;

        return path[path.Count / 2];
    }

    private void SpawnMiddle(Vector2Int roomCenter)
    {
        if (middlePrefab == null || grid == null)
            return;

        if (spawnedMiddle != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(spawnedMiddle);
#else
        Destroy(spawnedMiddle);
#endif
            spawnedMiddle = null;
        }

        Vector3Int cellPosition = new Vector3Int(roomCenter.x, roomCenter.y, 0);
        Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);
        spawnPosition.z = 0f;

        spawnedMiddle = Instantiate(middlePrefab, spawnPosition, Quaternion.identity);
        spawnedMiddle.name = "Middle";
    }

    private void SpawnExit(Vector2Int roomCenter)
    {
        if (exitPrefab == null || grid == null)
            return;

        if (spawnedExit != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(spawnedExit);
#else
        Destroy(spawnedExit);
#endif
            spawnedExit = null;
        }

        Vector3Int cellPosition = new Vector3Int(roomCenter.x, roomCenter.y, 0);
        Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);
        spawnPosition.z = 0f;

        spawnedExit = Instantiate(exitPrefab, spawnPosition, Quaternion.identity);
        spawnedExit.name = "Exit";
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2Int.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;

            }
        }
        return closest;
    }

    private List<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);

        while (position.y != destination.y)
        {
            if (destination.y > position.y)
                position += Vector2Int.up;
            else if (destination.y < position.y)
                position += Vector2Int.down;

            corridor.Add(position);
        }

        while (position.x != destination.x)
        {
            if (destination.x > position.x)
                position += Vector2Int.right;
            else if (destination.x < position.x)
                position += Vector2Int.left;

            corridor.Add(position);
        }

        return corridor;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}
