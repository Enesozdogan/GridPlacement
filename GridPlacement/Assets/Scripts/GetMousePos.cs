using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public class GetMousePos : MonoBehaviour
{
    public Vector3 mousePos;

    private Vector3 prevMouseInputPos;
    [SerializeField]
    private LayerMask planeLayer,objectLayer;


    [Header("Cursor Settings")]
    public Dictionary<CursorIndex, GameObject> cursorEnToInd = new Dictionary<CursorIndex, GameObject>();
    [SerializeField]
    private List<GameObject> cursorReferences;
    [SerializeField]
    private Transform cursorParent;
    [SerializeField]
    private float cursorOffsetY;
    public GameObject cursorObj;
    public bool isUpdatingCursor;
    
    private Grid grid;

    public bool isUsingGrid;
    public bool isDeleting;

    public GameObject targetObject;
    public event Action Onclick, OnCancel;

    public CursorIndex cursorIndex;
    private void Awake()
    {
        grid = GetComponentInChildren<Grid>();
        int i = 0;
        foreach (var obj in cursorReferences)
        {
            GameObject go = Instantiate(obj, cursorParent);
            go.SetActive(false);
            cursorEnToInd.Add((CursorIndex)i, go);
            i++;
        }
        cursorIndex = CursorIndex.sphereC;
        cursorEnToInd[cursorIndex].SetActive(true);
        cursorObj = cursorEnToInd[cursorIndex];
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
        if (Input.GetMouseButtonDown(1))
            OnCancel?.Invoke();

        MouseCoordinate();

    }

    private void MouseCoordinate()
    {
        if (isUsingGrid && Input.mousePosition != prevMouseInputPos)
        {
            isUpdatingCursor = true;
            GetMousePositionWorld();
            cursorObj.transform.position = grid.WorldToCell(mousePos);
            cursorObj.transform.position = grid.GetCellCenterWorld(Vector3Int.CeilToInt(grid.WorldToCell(mousePos))) - new Vector3(0, 0.5f, 0);
        }
        else if (isUpdatingCursor && Input.mousePosition == prevMouseInputPos)
        {
            isUpdatingCursor = false;
        }
    }

    public void ChangeCursorObject(CursorIndex toCursorIndex)
    {
        cursorEnToInd[toCursorIndex].SetActive(true);
        cursorObj = cursorEnToInd[toCursorIndex];
        cursorObj.transform.position = cursorEnToInd[cursorIndex].transform.position;
        cursorEnToInd[cursorIndex].SetActive(false);
        cursorIndex = toCursorIndex;
    }
    private void GetMousePositionWorld()
    {
        //Iki raycasti tek kolda yaz ki hem mouse hareketi hem de obje tespiti yapilsin.
        prevMouseInputPos = Input.mousePosition;
        Ray cameraRay = Camera.main.ScreenPointToRay(prevMouseInputPos);
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

public enum CursorIndex
{
    sphereC,
    flagC
}
