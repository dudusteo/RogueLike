using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelGenerator levelScript;
    public GameObject player;

    void Awake()
    {
        levelScript = GetComponent<LevelGenerator>();
        InitGame();

    }

    void InitGame()
    {
        levelScript.SetupLevel(1);
        GameObject player_instance = Instantiate(player);
        player_instance.transform.position = levelScript.GetStartPosition();

    }

}
