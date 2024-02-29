using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpStatePreview : OpStateBase
{
    [SerializeField]
    private Material previewMat;

    [SerializeField]
    private float offsetY;


    private Material previewMatInstance;

    
    private GameObject previewObj;

    [SerializeField]
    private GameObject cursor;

    [SerializeField]
    private OpStateBase opBuildState;
    protected override void OnEnterState()
    {
        builder.GetMousePos.cursorIndex = CursorIndex.flagC;
        builder.GetMousePos.ChangeCursorObject(CursorIndex.sphereC);
        builder.GetMousePos.isUsingGrid = true;
       StartPreview(builder.tiles.buildings[builder.selectedIndex].Prefab);
    }
    protected override void OnExitState()
    {
        builder.GetMousePos.ChangeCursorObject(CursorIndex.flagC);
        builder.GetMousePos.OnCancel -= HandleOnCancel;
        builder.GetMousePos.Onclick -= HandleOnClick;
        ClosePreview();
    }
    protected override void HandleOnClick()
    {
        if(builder.canPlace && !builder.GetMousePos.IsOverUI())
            builder.ShiftToState(opBuildState);
    }
    protected override void HandleOnCancel()
    {
        ClosePreview();
        builder.GetMousePos.OnCancel -= HandleOnCancel;
        builder.GetMousePos.Onclick -= HandleOnClick;
        builder.currOpState = null;
    }
    public void StartPreview(GameObject previewPrefab)
    {
        previewMatInstance = new Material(previewMat);
        CreateObject(previewPrefab);
        HandleMaterial();


    }
    private void CreateObject(GameObject prefab)
    {
        previewObj = Instantiate(prefab);
    }

    public void ClosePreview()
    {

        if (previewObj != null)
            Destroy(previewObj);
    }

    public void UpdatePreview(bool canPlace)
    {
        previewObj.transform.position = builder.GetMousePos.cursorObj.transform.position + new Vector3(0, offsetY, 0);
        ChangeMatColor(canPlace);

    }

    private void ChangeMatColor(bool canPlace)
    {
        Color matColor;


        if (canPlace)
            matColor = Color.white;
        else
            matColor = Color.red;
        matColor.a = 0.5f;

        previewMatInstance.color = matColor;
    }

    private void HandleMaterial()
    {
        Renderer[] renderers = previewObj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] mats = renderer.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = previewMatInstance;
            }
            renderer.materials = mats;
        }
    }

    public override void UpdateInState()
    {
        Vector3 placePos;
        int i, j;
        builder.GetDecimalCoordinateIndex(out placePos, out i, out j);

        
        builder.canPlace = builder.CheckCollision(i, j, builder.tiles.buildings[builder.selectedIndex].Size);
        UpdatePreview(builder.canPlace);

    }


}
