using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpStateMove : OpStateBase
{
   
    protected override void OnEnterState()
    {
        builder.GetMousePos.ChangeCursorObject(CursorIndex.flagC);
        builder.GetMousePos.isUsingGrid = true;
    }

    protected override void OnExitState()
    {
        builder.GetMousePos.ChangeCursorObject(CursorIndex.sphereC);
        builder.GetMousePos.isUsingGrid = false;
    }

    protected override void HandleOnCancel()
    {
        TerminateState();
    }

    protected override void HandleOnClick()
    {
       
    }

    public override void UpdateInState()
    {
        
    }
    public override void FixedUpdateInState()
    {
        
    }
    public override void TerminateState()
    {
        builder.GetMousePos.ChangeCursorObject(CursorIndex.sphereC);
        builder.GetMousePos.isUsingGrid = false;
        base.TerminateState();

    }

}
