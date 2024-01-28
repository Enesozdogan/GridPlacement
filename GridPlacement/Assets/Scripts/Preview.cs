using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preview : MonoBehaviour
{
    [SerializeField]
    private Material previewMat;

    [SerializeField]
    private float offsetY;

 
    private Material previewMatInstance;

    private GameObject previewObj;

    [SerializeField]
    private GameObject cursor;
    public void StartPreview(GameObject previewPrefab)
    {
        previewMatInstance= new Material(previewMat);
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
        previewObj.transform.position = cursor.transform.position + new Vector3(0, offsetY, 0);
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
}
