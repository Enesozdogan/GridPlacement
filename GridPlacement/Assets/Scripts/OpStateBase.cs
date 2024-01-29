using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpStateBase : MonoBehaviour
{
    [SerializeField]
    protected Builder builder;



    protected virtual void OnEnterState()
    {

    }

    protected virtual void OnExitState()
    {

    }
    protected virtual void HandleOnCancel()
    {

    }

    protected virtual void HandleOnClick()
    {

    }

    public virtual void UpdateInState()
    {

    }
    public virtual void FixedUpdateInState()
    {

    }
    public void Enter()
    {
        builder.GetMousePos.Onclick += HandleOnClick;
        builder.GetMousePos.OnCancel += HandleOnCancel;
        OnEnterState();
    }



    public void Exit()
    {
       
        builder.GetMousePos.Onclick -= HandleOnClick;
        builder.GetMousePos.OnCancel -= HandleOnCancel;
        OnExitState();
        
    }
}
