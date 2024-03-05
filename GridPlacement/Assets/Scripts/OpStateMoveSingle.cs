using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OpStateMoveSingle : OpStateBase
{
    [SerializeField]
    private SoldierMove soldier;


    protected override void OnEnterState()
    {
        builder.GetMousePos.ChangeCursorObject(CursorIndex.flagC);
        builder.GetMousePos.isUsingGrid = true;
        builder.GetMousePos.OnClick += StartMovingUnit;

    }

    protected override void OnExitState()
    {
        builder.GetMousePos.ChangeCursorObject(CursorIndex.sphereC);
        builder.GetMousePos.isUsingGrid = false;
        builder.GetMousePos.OnClick -= StartMovingUnit;

    }

    protected override void HandleOnCancel()
    {
        TerminateState();
    }

    protected override void HandleOnClick()
    {
        StartMovingUnit();
    }

  
    public override void TerminateState()
    {
        builder.GetMousePos.ChangeCursorObject(CursorIndex.sphereC);
        builder.GetMousePos.isUsingGrid = false;
        base.TerminateState();

    }


    private void StartMovingUnit()
    {
        //  birim yoksa coroutine'i 
       if(soldier.isSprinting) return;
       if (builder.GetMousePos.IsOverUI()) return;

        Vector2 goalP = new Vector2(builder.GetMousePos.cursorObj.transform.position.x, builder.GetMousePos.cursorObj.transform.position.z);
        Vector2 startP = new Vector2(soldier.transform.position.x, soldier.transform.position.z);
        soldier.StopAllCoroutines();
        soldier.AstarWithVectorsQueue(startP, goalP);
    }




   
}
