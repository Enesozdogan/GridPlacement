using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpStateDelete : OpStateBase
{

    private GameObject targetObject;

    protected override void OnEnterState()
    {
        builder.GetMousePos.isDeleting = true;
        builder.GetMousePos.isUsingGrid = true;
        builder.GetMousePos.OnClick += DeleteObject;
        builder.GetMousePos.OnCancel += StopDeleting;
    }
    protected override void OnExitState()
    {
        builder.GetMousePos.isUsingGrid = false;
        builder.GetMousePos.isDeleting = false;

        builder.GetMousePos.targetObject = null;

        builder.GetMousePos.OnClick -= DeleteObject;
        builder.GetMousePos.OnCancel -= StopDeleting;
    }

    private void StopDeleting()
    {
        builder.GetMousePos.isUsingGrid = false;
        builder.GetMousePos.isDeleting = false;

        builder.GetMousePos.targetObject = null;

        builder.GetMousePos.OnClick -= DeleteObject;
        builder.GetMousePos.OnCancel -= StopDeleting;
        builder.currOpState = null;
    }

    private void DeleteObject()
    {
        if (builder.GetMousePos.IsOverUI())
        {
            return;
        }
        if (targetObject == null)
        {
            Debug.LogError("No Target Object Detected");
            return;
        }

        if (targetObject.TryGetComponent(out ObjectDestroyer objectDestroyer))
        {
            Vector3 objPos = targetObject.transform.position;
            int i, j;

            builder.GetObjDecimalCoordinateIndex(objPos, out i, out j);
            Vector2 buildingSize = builder.tiles.buildings[builder.collisionMat[i, j]].Size;

            builder.MakeCellClear(i, j, buildingSize);
            objectDestroyer.Destroy();
        }
        else
        {
            Debug.LogError("Can't Destroy Object");
            return;
        }


    }

    public override void UpdateInState()
    {
        targetObject = builder.GetMousePos.GetRaycastedObject();
        
    }
}
