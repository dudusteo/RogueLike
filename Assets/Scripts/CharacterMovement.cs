using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;


public class CharacterMovement : MonoBehaviour
{

    public Tilemap map;

    public Camera mainCamera;

    private Functions functionsScript;

    private List<Vector2Int> path;
    private int currentPathIndex = 0;
    private Vector3 currentTarget;

    private MouseInput mouseInput;
    private void Awake()
    {
        mouseInput = new MouseInput();
    }

    private void OnEnable()
    {
        mouseInput.Enable();
    }
    private void OnDisable()
    {
        mouseInput.Disable();
    }

    private void Start()
    {
        path = new List<Vector2Int>();
        functionsScript = GetComponent<Functions>();
        mouseInput.Mouse.MouseClick.performed += _ => MouseClick();
        currentTarget = transform.position;
    }

    void Update()
    {
        if (transform.position == currentTarget)
        {
            currentPathIndex++;

            if (currentPathIndex >= path.Count)
            {
                return;
            }


            currentTarget = new Vector3(path[currentPathIndex].x + 0.5f, path[currentPathIndex].y + 0.5f, transform.position.z);
        }

        transform.position = Vector2.MoveTowards(transform.position, currentTarget, 10f * Time.deltaTime);
    }

    void MouseClick()
    {
        Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
        Vector3 mouseScreenPosition = new Vector3(mousePosition.x, mousePosition.y, -1f * Camera.main.transform.position.z);
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        Vector3Int gridPosition = map.WorldToCell(mouseWorldPosition);


        if (map.HasTile(gridPosition))
        {
            Vector3Int _current = map.WorldToCell(transform.position);
            Vector2Int current = new Vector2Int(_current.x, _current.y);

            Vector2Int destination = new Vector2Int(gridPosition.x, gridPosition.y);

            path = functionsScript.AStar(current, destination);
            Debug.Log(destination);
            currentPathIndex = 0;
        }

    }


}
