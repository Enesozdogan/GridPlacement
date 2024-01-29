using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpStateBuild : OpStateBase
{

    [SerializeField]
    private OpStateBase opPreviewState;
    protected override void OnEnterState()
    {

        PlaceBuilding();
        builder.GetMousePos.OnCancel += StopPlacing;
        
    }
    protected override void OnExitState()
    {
        builder.GetMousePos.OnCancel -= StopPlacing;
    }
 
   
    public void StopPlacing()
    {
        builder.GetMousePos.isUsingGrid = false;
        builder.selectedIndex = -1;

        builder.GetMousePos.OnCancel -= StopPlacing;
        builder.currOpState = null;
    }


    private void PlaceBuilding()
    {
        if (builder.GetMousePos.IsOverUI())
        {
            return;
        }

        Vector3 placePos;
        int i, j;
        builder.GetDecimalCoordinateIndex(out placePos, out i, out j);

        if (!builder.canPlace)
        {
            Debug.LogError("Cannot place on this cell");
            return;
        }


        GameObject building = Instantiate(builder.tiles.buildings[builder.selectedIndex].Prefab);
        building.transform.position = placePos;

        builder.MakeCellDirty(i, j, builder.tiles.buildings[builder.selectedIndex].Size, builder.tiles.buildings[builder.selectedIndex].Id);
        builder.ShiftToState(opPreviewState);
    }
}
