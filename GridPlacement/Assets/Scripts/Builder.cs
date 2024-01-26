using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public GetMousePos GetMousePos;
    public TileSO tiles;
    private int selectedIndex;

   
    public int[,] collisionMat = new int[10, 10];
    private void Start()
    {
        StopPlacing();

        for(int i = 0; i < collisionMat.GetLength(0); i++)
        {
            for (int j = 0; j < collisionMat.GetLength(1); j++)
                collisionMat[i, j] = -1;
        }
    }
    public void StartPlacing(int id)
    {
        selectedIndex = tiles.buildings.FindIndex(building => building.Id == id);
        if (selectedIndex == -1)
        {
            Debug.LogError("No Such Building");
            return;
        }

        GetMousePos.Onclick += PlaceBuilding;
        GetMousePos.OnCancel += StopPlacing;

    }

    public void StopPlacing()
    {
        selectedIndex = -1;
        GetMousePos.Onclick -= PlaceBuilding;
        GetMousePos.OnCancel -= StopPlacing;
    }

    private void PlaceBuilding()
    {
        if (GetMousePos.IsOverUI())
        {
            return;
        }

        Vector3 placePos = GetMousePos.cursor.transform.position;
        int i = (int)(placePos.z + 4.5f);
        int j = (int)(placePos.x + 4.5f);

        if (!CheckCollision(i,j, tiles.buildings[selectedIndex].Size))
        {
            Debug.LogError("Cannot place on this cell");
            return;
        }
            

        GameObject building = Instantiate(tiles.buildings[selectedIndex].Prefab);
        building.transform.position = placePos;

        MakeCellDirty(i, j, tiles.buildings[selectedIndex].Size, tiles.buildings[selectedIndex].Id);
        
    }

    private void MakeCellDirty(int indexI, int indexJ, Vector2 buildingSize, int buildingId)
    {
        for (int i = indexI; i < indexI+buildingSize.y; i++)
        {
            for (int j = indexJ; j < indexJ+buildingSize.x; j++)
            {
                collisionMat[i, j] = buildingId;
            }
        }
    }

    private bool CheckCollision(int indexI,int indexJ, Vector2 buildingSize)
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
}
