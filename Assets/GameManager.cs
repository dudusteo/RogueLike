using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public GridManager levelScript;
    public GameObject player;

    public Tilemap tilemap;

    void Awake()
    {
        levelScript = GetComponent<GridManager>();
        InitGame();

    }

    void InitGame()
    {
        levelScript.SetupLevel(1);

        player.transform.position = tilemap.GetCellCenterWorld(levelScript.GetStartPosition());

    }

}
