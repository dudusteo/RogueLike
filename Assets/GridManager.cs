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

    private Functions functionsScript;

    private Tile floorTile;
    private Tile wallTile;

    public Tilemap groundTilemap;
    public Tilemap collisionTilemap;

    private List<RectInt> roomRects = new List<RectInt>();

    public int minRoomSize = 5;
    public int maxRoomSize = 11;
    public int splitIterations = 3;

    private RectInt rootRect;

    public void SetupLevel(int level)
    {
        functionsScript = GetComponent<Functions>();
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

                    collisionTilemap.SetTile(tilePosition, wallTile);
                }
                else

                    groundTilemap.SetTile(tilePosition, floorTile);

            }
        }
    }

    private void ConnectRooms(RectInt _start, RectInt _end)
    {
        Vector2Int current = new Vector2Int(_start.x + _start.width / 2, _start.y + _start.height / 2);
        Vector2Int goal = new Vector2Int(_end.x + _end.width / 2, _end.y + _end.height / 2);

        List<Vector2Int> path = functionsScript.AStar(current, goal);

        foreach (Vector2Int pos in path)
        {
            Vector3Int tilePosition = new Vector3Int(pos.x, pos.y, 0);

            groundTilemap.SetTile(tilePosition, floorTile);

        }
    }
}