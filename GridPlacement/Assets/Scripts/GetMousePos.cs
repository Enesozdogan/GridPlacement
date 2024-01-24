using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GetMousePos : MonoBehaviour
{
    private Vector3 mousePos;
    [SerializeField]
    private LayerMask planeLayer;

    [SerializeField]
    private GameObject cursor;
    [SerializeField]
    private float cursorOffsetY;
    private Grid grid;
    private void Awake()
    {
        grid = GetComponentInChildren<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        GetMousePositionWorld();
        cursor.transform.position = grid.WorldToCell(mousePos);
        cursor.transform.position = grid.GetCellCenterWorld(Vector3Int.CeilToInt(grid.WorldToCell(mousePos))) - new Vector3(0,0.5f,0);
        

        Debug.Log(mousePos);
    }

    private void GetMousePositionWorld()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(cameraRay, out hit, 1000, planeLayer))
        {
            mousePos = hit.point;
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(Camera.main.transform.position, (mousePos - Camera.main.transform.position), Color.red);
    }
}
