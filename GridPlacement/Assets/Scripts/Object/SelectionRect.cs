using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionRect : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;

    [SerializeField]
    Builder builder;

    private Vector2 startPos;
    private Vector2 endPos;
    private bool isSelecting = false;
    RectTransform parent;
    private void Awake()
    {
         parent = rectTransform.parent as RectTransform;
        builder.GetMousePos.OnClick += StartSelectingUnit;
        builder.GetMousePos.OnLetGo += StopSelectingUnit;
    }
    void Update()
    {
        if (isSelecting)
        {

            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, Input.mousePosition, null, out endPos);

            UpdateSelectionBox();
        }
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

        isSelecting = true;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, Input.mousePosition, null, out startPos);
    }
    void UpdateSelectionBox()
    {
        if (!isSelecting)
            return;

        Vector2 boxStart = startPos;
        Vector2 boxEnd = endPos;
        Vector2 boxCenter = (boxStart + boxEnd) / 2;
        
        rectTransform.localPosition = boxCenter;

        Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));
        rectTransform.sizeDelta = boxSize;
    }
}
