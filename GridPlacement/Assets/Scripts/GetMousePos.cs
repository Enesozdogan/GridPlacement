using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GetMousePos : MonoBehaviour
{
    public Vector3 mousePos;
    [SerializeField]
    private LayerMask planeLayer,objectLayer;

    public GameObject cursor;
    [SerializeField]
    private float cursorOffsetY;
    private Grid grid;

    public bool isUsingGrid;
    public bool isDeleting;

    public GameObject targetObject;
    public event Action Onclick, OnCancel;
    private void Awake()
    {
        grid = GetComponentInChildren<Grid>();
    
    }
    
    public bool IsOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Onclick?.Invoke();
        if(Input.GetMouseButtonDown(1))
            OnCancel?.Invoke();

        if (isUsingGrid)
        {
            if (isDeleting)
            {
                targetObject = GetRaycastedObject();
            }
            GetMousePositionWorld();
            cursor.transform.position = grid.WorldToCell(mousePos);
            cursor.transform.position = grid.GetCellCenterWorld(Vector3Int.CeilToInt(grid.WorldToCell(mousePos))) - new Vector3(0, 0.5f, 0);
        }
       
        

        
    }

    private void GetMousePositionWorld()
    {
        //Iki raycasti tek kolda yaz ki hem mouse hareketi hem de obje tespiti yapilsin.
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
     
        if (Physics.Raycast(cameraRay, out hit, 1000, planeLayer))
        {
            mousePos = hit.point;
        }


    }

    //Mouse pozisyonu raycast edilen objenin pozisyonuna getirilmeli ki matrix dogru coordinattan temizlensin.
    public GameObject GetRaycastedObject()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit2;
        if (Physics.Raycast(cameraRay, out hit2, 1000, objectLayer))
        {
            return hit2.collider.gameObject;
        }
        return null;
    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(Camera.main.transform.position, (mousePos - Camera.main.transform.position), Color.red);
    }
}
