using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{

    public Sprite borderSprite;

    private Tile borderTile;
    public Tilemap groundTilemap;
    public Tilemap overlayTilemap;

    private Vector3Int previousTilePosition;
    private Tile previousTile = null;

    void Start()
    {
        borderTile = ScriptableObject.CreateInstance<Tile>();
        borderTile.sprite = borderSprite;
    }

    void Update()
    {
        Vector3 mouseScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f * Camera.main.transform.position.z);
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);


        Vector3Int tilePosition = groundTilemap.WorldToCell(mouseWorldPosition);

        if (previousTilePosition != tilePosition)
        {


            overlayTilemap.SetTile(previousTilePosition, previousTile);


            if (groundTilemap.HasTile(tilePosition))
            {
                previousTile = overlayTilemap.GetTile<Tile>(tilePosition);
                overlayTilemap.SetTile(tilePosition, borderTile);
            }

            previousTilePosition = tilePosition;
        }

    }
}