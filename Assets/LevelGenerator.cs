using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class LevelGenerator : MonoBehaviour
{

    public GameObject floorTile;
    public GameObject wallTile;
    GameObject root;

    private List<RectInt> roomRects = new List<RectInt>();

    public int minRoomSize = 5;
    public int maxRoomSize = 11;
    public int splitIterations = 3;

    private RectInt rootRect;

    public void SetupLevel(int level)
    {
        roomRects.Clear();
        root = gameObject;
        rootRect = new RectInt(0, 0, 50, 50);
        SplitSpace(rootRect, splitIterations);
        ConnectRooms();
    }

    public Vector3 GetStartPosition()
    {
        int posX = roomRects[0].x + roomRects[0].width / 2;
        int posY = roomRects[0].y + roomRects[0].height / 2;
        return new Vector3(posX, posY, 0);
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
        PopulateRoom(room, space);
    }

    private void PopulateRoom(GameObject parent, RectInt room)
    {
        for (int x = room.x; x < room.x + room.width + 1; x++)
        {
            for (int y = room.y; y < room.y + room.height + 1; y++)
            {
                GameObject instance;
                if ((room.x == x) || (room.y == y) || (room.x + room.width == x) || (room.y + room.height == y))
                {
                    instance = Instantiate(wallTile, parent.transform);
                }
                else
                {
                    instance = Instantiate(floorTile, parent.transform);

                }
                instance.transform.position = new Vector3(x, y, 0);
            }
        }
    }

    private void ConnectRooms()
    {

    }


}