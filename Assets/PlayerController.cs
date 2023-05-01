using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{

    public Tilemap groundTilemap;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Move()
    {

    }

    private bool CanMove(Vector2 direction)
    {
        Vector3Int gridPosition = groundTilemap.WorldToCell(transform.position + (Vector3)direction);
        if (!groundTilemap.HasTile(gridPosition))
            return false;

        return true;
    }
}
