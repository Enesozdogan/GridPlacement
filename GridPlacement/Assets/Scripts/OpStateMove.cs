using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OpStateMove : OpStateBase
{
    /// <summary>
    /// To Activate this script Attach it to states objects under GridParent and fill the fields
    /// </summary>

    [SerializeField]
    private RectTransform rectTransform;


    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2 boxCenter;

    public bool isSelecting = false;
    RectTransform parent;
 
    private HashSet<SoldierMove> soldiers = new();

    [SerializeField]
    private LayerMask objectMask;


    private void Awake()
    {
        parent = rectTransform.parent as RectTransform;
        rectTransform.sizeDelta = Vector2.zero;
    }
    protected override void OnEnterState()
    {
        builder.GetMousePos.ChangeCursorObject(CursorIndex.flagC);
        builder.GetMousePos.isUsingGrid = true;
        builder.GetMousePos.OnClick += StartSelectingUnit;
        builder.GetMousePos.OnLetGo += StopSelectingUnit;
    }

    protected override void OnExitState()
    {
        builder.GetMousePos.ChangeCursorObject(CursorIndex.sphereC);
        builder.GetMousePos.isUsingGrid = false;
       // builder.GetMousePos.OnClick -= StartSelectingUnit;
        builder.GetMousePos.OnLetGo -= StopSelectingUnit;
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
        if (isSelecting)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, Input.mousePosition, null, out endPos);
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            for (int i = 0; i < 4; i++)
            {
                Ray ray = Camera.main.ScreenPointToRay(corners[i]);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit,200))
                {
                    corners[i] = hit.point;
                }
            }

            Vector3 boxCenter = (corners[0] + corners[2]) / 2;
            Vector3 boxSize = new Vector3(
                Mathf.Abs(corners[0].x - corners[2].x),
                Mathf.Abs(corners[0].y - corners[2].y),
                Mathf.Abs(corners[0].z - corners[2].z)
            );


            Collider[] colliders = Physics.OverlapBox(boxCenter, boxSize / 2,Quaternion.identity,objectMask);
 
            foreach (Collider collider in colliders)
            {
                if(collider.TryGetComponent(out SoldierMove soldier))
                {
                    Debug.LogError(soldier.name);
                    soldiers.Add(soldier);
                }
            }

            UpdateSelectionBox();
        }

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
    private void StopSelectingUnit()
    {
        if (!isSelecting) return;

        isSelecting = false;
        rectTransform.sizeDelta = Vector2.zero;
      

    }

    private void StartSelectingUnit()
    {
        if (builder.GetMousePos.IsOverUI()) return;


        builder.GetMousePos.OnClick -= StartSelectingUnit;
        builder.GetMousePos.OnLetGo += StopSelectingUnit;
        builder.GetMousePos.OnClick += StartMovingUnit;

        soldiers.Clear();
         isSelecting = true;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, Input.mousePosition, null, out startPos);
    }

    private void StartMovingUnit()
    {
        //  birim yoksa coroutine'i 
        if (soldiers.Count == 0)
        {
            Debug.LogError("Secemedi");
            isSelecting = true;
            return;

        }

        builder.GetMousePos.OnClick += StartSelectingUnit;
        builder.GetMousePos.OnLetGo -= StopSelectingUnit;
        builder.GetMousePos.OnClick -= StartMovingUnit;
        Vector2 goalP = new Vector2(builder.GetMousePos.cursorObj.transform.position.x, builder.GetMousePos.cursorObj.transform.position.z);
        Debug.LogWarning(soldiers);
        foreach (var soldier in soldiers)
        {
            Vector2 startP = new Vector2(soldier.transform.position.x, soldier.transform.position.z);
            soldier.StopAllCoroutines();
            soldier.AstarWithVectorsQueue(startP, goalP);

           
        }
        //StartCoroutine(SequentialPathfinding(goalP));
    }

    IEnumerator SequentialPathfinding(Vector2 goalP)
    {
        foreach (var soldier in soldiers)
        {
            Vector2 startP = new Vector2(soldier.transform.position.x, soldier.transform.position.z);
            soldier.StopAllCoroutines();
            soldier.isPathGenerated = false;
            soldier.isFound = false;
            soldier.AstarWithVectorsQueue(startP, goalP);

            yield return null;
        }
    }


    void UpdateSelectionBox()
    {
        if (!isSelecting)
            return;

        Vector2 boxStart = startPos;
        Vector2 boxEnd = endPos;
        boxCenter = (boxStart + boxEnd) / 2;

        rectTransform.localPosition = boxCenter;

        Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));
        rectTransform.sizeDelta = boxSize;

    }
    void OnDrawGizmos()
    {
        if (isSelecting && rectTransform != null)
        {
            // Get the screen corners of the RectTransform
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            // Convert the screen corners to world coordinates
            for (int i = 0; i < 4; i++)
            {
                Ray ray = Camera.main.ScreenPointToRay(corners[i]);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    corners[i] = hit.point;
                }
            }

            // Calculate the center and size of the box in world coordinates
            Vector3 boxCenter = (corners[0] + corners[2]) / 2;
            Vector3 boxSize = new Vector3(
                Mathf.Abs(corners[0].x - corners[2].x),
                Mathf.Abs(corners[0].y - corners[2].y),
                Mathf.Abs(corners[0].z - corners[2].z)
            );

            // Draw a wire cube at the position of the box with the size of the box
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boxCenter, boxSize);
        }
    }
}
