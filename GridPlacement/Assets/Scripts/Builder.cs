
using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Builder : MonoBehaviour
{
    public GetMousePos GetMousePos;
    public TileSO tiles;
    public int selectedIndex;

    [SerializeField]
    public OpStateBase currOpState, previewOpState,deleteOpState;

    public bool canPlace;
    [SerializeField]
    public GameObject gridPlane;

    [SerializeField]
    public Preview preview;

    public int[,] collisionMat = new int[10, 10];

    public List<Node> Nodes = new List<Node>();

    public static Builder Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        int indexCounter = 0;
        int yCounter = collisionMat.GetLength(1)-1;
        for (int y = 0; y < collisionMat.GetLength(0); y++)
        {
            for (int x = 0; x < collisionMat.GetLength(1); x++)
            {
                collisionMat[y, x] = -1;

                Node node = new Node();
                node.nodePos = new Vector2(x-4.5f, yCounter-4.5f);
                node.nodeIndex = indexCounter++;
                Nodes.Add(node);

            }
            yCounter--;
        }


    }

    

    public void GetDecimalCoordinateIndex(out Vector3 placePos, out int i, out int j)
    {
        placePos = GetMousePos.cursor.transform.position;
        i = (int)(4.5f - placePos.z);
        j = (int)(placePos.x + 4.5f);
    }
    public void GetObjDecimalCoordinateIndex(Vector3 objPos, out int i, out int j)
    { 
        i = (int)( 4.5f - objPos.z);
        j = (int)(objPos.x + 4.5f);
    }
    public void MakeCellDirty(int indexI, int indexJ, Vector2 buildingSize, int buildingId)
    {
        for (int i = indexI; i < indexI+buildingSize.y; i++)
        {
            for (int j = indexJ; j < indexJ+buildingSize.x; j++)
            {
                collisionMat[i, j] = buildingId;
            }
        }
    }
    public void MakeCellClear(int indexI, int indexJ, Vector2 buildingSize)
    {
        for (int i = indexI; i < indexI + buildingSize.y; i++)
        {
            for (int j = indexJ; j < indexJ + buildingSize.x; j++)
            {
                collisionMat[i, j] = -1;
            }
        }
    }

    public  bool CheckCollision(int indexI,int indexJ, Vector2 buildingSize)
    {
        for(int i= indexI; i< indexI + buildingSize.y; i++)
        {
            for(int j= indexJ; j< indexJ + buildingSize.x; j++)
            {
                if (i >= collisionMat.Length || j >= collisionMat.GetLength(0))
                    return false;
                if (collisionMat[i, j] != -1 )
                    return false;
            }
        }
        return true;
    }

    private void Update()
    {

        if(currOpState != null) 
            currOpState.UpdateInState();

    }

    public void ShiftToState(OpStateBase opState)
    {
        if (currOpState != null)
            currOpState.Exit();

        currOpState = opState;
        currOpState.Enter();

    }

    public void StartPreview(int id)
    {
        selectedIndex = id;
        ShiftToState(previewOpState);
    }
    public void StartDeleting()
    {
        ShiftToState(deleteOpState);
    }
}

[System.Serializable]
public class Node : FastPriorityQueueNode
{
    public Vector2 nodePos;
    public Node parentNode;
    public int nodeIndex;
    public bool isOnClosed = false;
    public int fVal;
    public int gVal;
    public int hVal;


}
