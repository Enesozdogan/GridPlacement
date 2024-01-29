using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ObjectChangeEventStream;

public class Builder : MonoBehaviour
{
    public GetMousePos GetMousePos;
    public TileSO tiles;
    public int selectedIndex;

    [SerializeField]
    public OpStateBase currOpState, previewOpState,deleteOpState;

    public bool canPlace;
    [SerializeField]
    public GameObject gridPlane;

    [SerializeField]
    public Preview preview;

    public int[,] collisionMat = new int[10, 10];
    private void Start()
    {
        //StopPlacing();

        for(int i = 0; i < collisionMat.GetLength(0); i++)
        {
            for (int j = 0; j < collisionMat.GetLength(1); j++)
                collisionMat[i, j] = -1;
        }
    }

    //public void StartDeleting()
    //{
    //    StopPlacing();
    //    GetMousePos.isDeleting= true;
    //    GetMousePos.isUsingGrid = true;

    //    GetMousePos.Onclick += DeleteObject;
    //    GetMousePos.OnCancel += StopDeleting;
    //}

    //private void StopDeleting()
    //{
    //    GetMousePos.isUsingGrid = false;
    //    GetMousePos.isDeleting = false;

    //    GetMousePos.targetObject = null; 

    //    GetMousePos.Onclick -= DeleteObject;
    //    GetMousePos.OnCancel -= StopDeleting;
    //}

    //private void DeleteObject()
    //{
    //    if (GetMousePos.IsOverUI())
    //    {
    //        return;
    //    }
    //    if (GetMousePos.targetObject == null)
    //    {
    //        Debug.LogError("No Target Object Detected");
    //        return;
    //    }

    //    if(GetMousePos.targetObject.TryGetComponent(out ObjectDestroyer objectDestroyer))
    //    {
    //        Vector3 objPos = GetMousePos.targetObject.transform.position;
    //        int i, j;
        
    //        GetObjDecimalCoordinateIndex(objPos, out i , out j);
    //        Vector2 buildingSize = tiles.buildings[collisionMat[i, j]].Size;

    //        MakeCellClear(i,j,buildingSize);
    //        objectDestroyer.Destroy();
    //    }
    //    else
    //    {
    //        Debug.LogError("Can't Destroy Object");
    //        return;
    //    }
            

    //}

    //public void StartPlacing(int id)
    //{
    //    StopDeleting();
    //    GetMousePos.isUsingGrid = true;
        
    //    selectedIndex = tiles.buildings.FindIndex(building => building.Id == id);
    //    if (selectedIndex == -1)
    //    {
    //        Debug.LogError("No Such Building");
    //        return;
    //    }
    //    preview.StartPreview(tiles.buildings[selectedIndex].Prefab);

    //    GetMousePos.Onclick += PlaceBuilding;
    //    GetMousePos.OnCancel += StopPlacing;

    //}

    //public void StopPlacing()
    //{
    //    GetMousePos.isUsingGrid = false;
    //    preview.ClosePreview();
    //    selectedIndex = -1;
    //    GetMousePos.Onclick -= PlaceBuilding;
    //    GetMousePos.OnCancel -= StopPlacing;
    //}

    //private void PlaceBuilding()
    //{
    //    if (GetMousePos.IsOverUI())
    //    {
    //        return;
    //    }

    //    Vector3 placePos;
    //    int i, j;
    //    GetDecimalCoordinateIndex(out placePos, out i, out j);

    //    if (!canPlace)
    //    {
    //        Debug.LogError("Cannot place on this cell");
    //        return;
    //    }


    //    GameObject building = Instantiate(tiles.buildings[selectedIndex].Prefab);
    //    building.transform.position = placePos;

    //    MakeCellDirty(i, j, tiles.buildings[selectedIndex].Size, tiles.buildings[selectedIndex].Id);

    //}

    public void GetDecimalCoordinateIndex(out Vector3 placePos, out int i, out int j)
    {
        placePos = GetMousePos.cursor.transform.position;
        i = (int)(placePos.z + 4.5f);
        j = (int)(placePos.x + 4.5f);
    }
    public void GetObjDecimalCoordinateIndex(Vector3 objPos, out int i, out int j)
    { 
        i = (int)(objPos.z + 4.5f);
        j = (int)(objPos.x + 4.5f);
    }
    public void MakeCellDirty(int indexI, int indexJ, Vector2 buildingSize, int buildingId)
    {
        for (int i = indexI; i < indexI+buildingSize.y; i++)
        {
            for (int j = indexJ; j < indexJ+buildingSize.x; j++)
            {
                collisionMat[i, j] = buildingId;
            }
        }
    }
    public void MakeCellClear(int indexI, int indexJ, Vector2 buildingSize)
    {
        for (int i = indexI; i < indexI + buildingSize.y; i++)
        {
            for (int j = indexJ; j < indexJ + buildingSize.x; j++)
            {
                collisionMat[i, j] = -1;
            }
        }
    }

    public  bool CheckCollision(int indexI,int indexJ, Vector2 buildingSize)
    {
        for(int i= indexI; i< indexI + buildingSize.y; i++)
        {
            for(int j= indexJ; j< indexJ + buildingSize.x; j++)
            {
                if (collisionMat[i, j] != -1)
                    return false;
            }
        }
        return true;
    }

    private void Update()
    {
        //    if (GetMousePos.isUsingGrid)
        //    {
        //        if (!gridPlane.activeSelf)
        //        {
        //            gridPlane.SetActive(true);
        //            GetMousePos.cursor.SetActive(true);
        //        }
        //        if (!GetMousePos.isDeleting)
        //        {
        //            Vector3 placePos;
        //            int i, j;

        //            GetDecimalCoordinateIndex(out placePos, out i, out j);
        //            canPlace = CheckCollision(i, j, tiles.buildings[selectedIndex].Size);
        //            preview.UpdatePreview(canPlace);
        //            Debug.LogWarning(canPlace);

        //        }
        //    }
        //    else if(!GetMousePos.isUsingGrid && gridPlane.activeSelf)
        //    {

        //        gridPlane.SetActive(false);
        //        GetMousePos.cursor.SetActive(false);
        //    }
        if(currOpState != null) 
            currOpState.UpdateInState();

    }

    public void ShiftToState(OpStateBase opState)
    {
        if (currOpState != null)
            currOpState.Exit();

        currOpState = opState;
        currOpState.Enter();

    }

    public void StartPreview(int id)
    {
        selectedIndex = id;
        ShiftToState(previewOpState);
    }
    public void StartDeleting()
    {
        ShiftToState(deleteOpState);
    }
}
