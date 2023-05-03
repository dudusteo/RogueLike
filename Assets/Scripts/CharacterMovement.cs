using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class CharacterMovement : MonoBehaviour
{

    public Tilemap map;

    public Camera mainCamera;

    private MouseInput mouseInput;
    // Start is called before the first frame update
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
        mouseInput.Mouse.MouseClick.performed += _ => MouseClick();
    }


    // Update is called once per frame
    void MouseClick()
    {
        // Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
        // Vector3 mouseScreenPosition = new Vector3(mousePosition.x, mousePosition.y, Camera.main.transform.position.z);
        // Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        // Vector3Int gridPosition = map.WorldToCell(mouseWorldPosition);


        // if (map.HasTile(gridPosition))
        // {
        //     Debug.Log((mouseScreenPosition, mouseWorldPosition, gridPosition));

        // }

    }


}
