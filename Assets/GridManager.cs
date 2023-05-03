using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.Tilemaps;
public class GridManager : MonoBehaviour
{

    public Sprite floorSprite;
    public Sprite wallSprite;

    private Tile floorTile;
    private Tile wallTile;

    public Tilemap tilemap;

    private List<RectInt> roomRects = new List<RectInt>();

    public int minRoomSize = 5;
    public int maxRoomSize = 11;
    public int splitIterations = 3;

    private RectInt rootRect;

    public void SetupLevel(int level)
    {

        roomRects.Clear();
        rootRect = new RectInt(0, 0, 50, 50);
        setupTiles();
        SplitSpace(rootRect, splitIterations);

        RectInt prevRoom = roomRects[0];
        foreach (RectInt room in roomRects)
        {
            if ((prevRoom.x != room.x) || (prevRoom.y != room.y))
            {
                ConnectRooms(prevRoom, room);
            }
            prevRoom = room;

        }

    }


    public Vector3Int GetStartPosition()
    {
        int posX = roomRects[0].x + roomRects[0].width / 2;
        int posY = roomRects[0].y + roomRects[0].height / 2;
        return new Vector3Int(posX, posY, 0);
    }

    private void setupTiles()
    {
        floorTile = ScriptableObject.CreateInstance<Tile>(); ;
        floorTile.sprite = floorSprite;

        wallTile = ScriptableObject.CreateInstance<Tile>(); ;
        wallTile.sprite = wallSprite;
    }


    private void SplitSpace(RectInt space, int iterations)
    {
        if (iterations == 0)
        {
            GenerateRoom(space);
            return;
        }

        bool splitVertically = Random.value > 0.5f;
        float splitPosition = Random.Range(0.3f, 0.7f);

        RectInt leftSpace, rightSpace;

        if (splitVertically)
        {
            int splitX = Mathf.RoundToInt(space.x + space.width * splitPosition);
            leftSpace = new RectInt(space.x, space.y, splitX - space.x, space.height);
            rightSpace = new RectInt(splitX, space.y, space.width - (splitX - space.x), space.height);
        }
        else
        {
            int splitY = Mathf.RoundToInt(space.y + space.height * splitPosition);
            leftSpace = new RectInt(space.x, space.y, space.width, splitY - space.y);
            rightSpace = new RectInt(space.x, splitY, space.width, space.height - (splitY - space.y));
        }

        SplitSpace(leftSpace, iterations - 1);
        SplitSpace(rightSpace, iterations - 1);
    }

    private void GenerateRoom(RectInt space)
    {
        int roomWidth = Random.Range(minRoomSize, Mathf.Min(maxRoomSize, Mathf.RoundToInt(space.width)));
        roomWidth = Mathf.FloorToInt(roomWidth / 2) * 2 + 1;

        int roomHeight = Random.Range(minRoomSize, Mathf.Min(maxRoomSize, Mathf.RoundToInt(space.height)));
        roomHeight = Mathf.FloorToInt(roomHeight / 2) * 2 + 1;

        space.x += (space.width - roomWidth) / 2;
        space.y += (space.height - roomHeight) / 2;

        space.width = roomWidth;
        space.height = roomHeight;

        roomRects.Add(space);

        GameObject room = new GameObject("Room");
        room.transform.position = new Vector3(space.x, space.y, 0);
        PopulateRoom(space);
    }

    private void PopulateRoom(RectInt room)
    {
        for (int x = room.x; x <= room.x + room.width; x++)
        {
            for (int y = room.y; y <= room.y + room.height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if ((room.x == x) || (room.y == y) || (room.x + room.width == x) || (room.y + room.height == y))
                {

                    tilemap.SetTile(tilePosition, wallTile);
                }
                else

                    tilemap.SetTile(tilePosition, floorTile);

            }
        }
    }




    private void ConnectRooms(RectInt _start, RectInt _end)
    {
        Vector2Int current = new Vector2Int(_start.x + _start.width / 2, _start.y + _start.height / 2);
        Vector2Int goal = new Vector2Int(_end.x + _end.width / 2, _end.y + _end.height / 2);

        List<Vector2Int> path = AStar(current, goal);

        foreach (Vector2Int pos in path)
        {
            Vector3Int tilePosition = new Vector3Int(pos.x, pos.y, 0);

            tilemap.SetTile(tilePosition, floorTile);

        }
    }

    List<Vector2Int> AStar(Vector2Int _current, Vector2Int _goal)
    {
        List<Node> openList = new List<Node> { new Node(_current, 0, ManhattanDistance(_current, _goal), null) };
        List<Node> closedList = new List<Node>();

        while (openList.Count > 0)
        {
            // Szukamy wierzchołka o najniższym fScore na liście otwartych wierzchołków
            Node currentNode = openList.OrderBy(node => node.FScore).First();

            // Jeśli znaleźliśmy cel, zwracamy znalezioną ścieżkę
            if (currentNode.Position == _goal)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                Node node = currentNode;
                while (node != null)
                {
                    path.Add(node.Position);
                    node = node.Parent;
                }
                path.Reverse();
                return path;
            }

            // Przenosimy wierzchołek z listy otwartych na zamkniętej
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Dla każdego sąsiada wierzchołka aktualizujemy jego koszt dojścia, jeśli droga przez currentNode jest krótsza
            foreach (Vector2Int neighbor in GetNeighbors(currentNode.Position))
            {
                // Ignorujemy sąsiadów, którzy są na zamkniętej liście
                if (closedList.Any(node => node.Position == neighbor))
                {
                    continue;
                }

                // Obliczamy koszt dojścia do sąsiada
                int gScore = currentNode.GScore + 1;

                // Dodajemy sąsiada do listy otwartych wierzchołków, jeśli nie ma na niej
                Node neighborNode = openList.FirstOrDefault(node => node.Position == neighbor);
                if (neighborNode == null)
                {
                    neighborNode = new Node(neighbor, gScore, ManhattanDistance(neighbor, _goal), currentNode);
                    openList.Add(neighborNode);
                }
                else if (gScore < neighborNode.GScore)
                {
                    // Aktualizujemy sąsiada, jeśli droga przez currentNode jest krótsza
                    neighborNode.GScore = gScore;
                    neighborNode.Parent = currentNode;
                }
            }
        }
        return new List<Vector2Int>();
    }





    private List<Vector2Int> GetNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // Sąsiedzi po bokach
        neighbors.Add(new Vector2Int(position.x + 1, position.y));
        neighbors.Add(new Vector2Int(position.x - 1, position.y));
        neighbors.Add(new Vector2Int(position.x, position.y + 1));
        neighbors.Add(new Vector2Int(position.x, position.y - 1));

        return neighbors;
    }

    int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        return xDistance + yDistance;
    }


    private class Node
    {
        public Vector2Int Position { get; set; }
        public int GScore { get; set; }
        public int HScore { get; set; }
        public Node Parent { get; set; }

        public int FScore => GScore + HScore;

        public Node(Vector2Int position, int gScore, int hScore, Node parent)
        {
            Position = position;
            GScore = gScore;
            HScore = hScore;
            Parent = parent;
        }
    }





}