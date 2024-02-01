using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{

    [SerializeField]
    private Builder builder;

    [SerializeField]
    private Vector2 startP, goalP;

    [SerializeField]
    private int sizeX, sizeY;

    public bool isFound = false;
    public bool hasActiveNode = false;
    public bool isGeneratingPath = false;
    public List<Node> nodes = new List<Node>();
    public List<Node> openNodes = new List<Node>();
    public List<Node> closedNodes = new List<Node>();
    public List<Node> pathNodes = new();
    private void Start()
    {
        int indexCounter = 0;
        for(int i =0; i< sizeX; i++)
        {
            for(int j =0; j<sizeY; j++)
            {
                Node node = new Node();
                node.nodePos = new Vector2(i,j);
                node.nodeIndex = indexCounter++;
                nodes.Add(node);
            }
        }
        AddInitialNodeToOpen();

    }

    private void Update()
    {
        if (!isFound && !hasActiveNode)
            AStar();
    }



    private void AddInitialNodeToOpen()
    {
        Node node = nodes.Find(x => x.nodePos.x == startP.x && x.nodePos.y == startP.y);
        node.parentNode = null;
        openNodes.Add(node);
        
    }

    /// <summary>
    /// Ilk once baslangic elemanini al
    /// Cocuklarini open liste at
    /// bunlardan en kucuk f e sahip olani al 
    /// Closed list e at 
    /// open list'te en kucuk f e sahip olandan devam et 
    /// </summary>
    private void AStar()
    {
        Node currNode = openNodes.OrderBy(x => x.fVal).FirstOrDefault();
        
        //Debug.LogError("Current Node: " + currNode.nodePos);
        hasActiveNode = true;
        if (currNode.nodePos == goalP)
        {
            isFound = true;
            closedNodes.Add(currNode);
            
            
            StartCoroutine(PrintPath(pathNodes));
            
            //StartCoroutine(GeneratePath(pathNodes));
          //  PrintPath(closedNodes);
        }

        List<Node> neighbours =FindNeighbour(currNode.nodeIndex);

        foreach(Node node in neighbours)
        {
            int foundIndex = openNodes.FindIndex(x => x.nodeIndex == node.nodeIndex);
            int foundClosed = closedNodes.FindIndex(x=> x.nodeIndex == node.nodeIndex);

            //If already in closed list skip this neighbour
            if (foundClosed >= 0)
                continue;
            //If not check if its in open list if found compare f value
            if (foundIndex >= 0)
            {
                if (openNodes[foundIndex].fVal > node.fVal)
                    openNodes[foundIndex] = node;
            }
            else
            {
                openNodes.Add(node);
            }
        }
        openNodes.Remove(currNode);
        closedNodes.Add(currNode);
        currNode.isOnClosed = true;
        hasActiveNode = false;
    }

    private List<Node> FindNeighbour(int currNodeIndex)
    {
        int rightIndex = currNodeIndex + 1;
        int leftIndex = currNodeIndex - 1;
        int downIndex = currNodeIndex + sizeX;
        int upIndex = currNodeIndex - sizeX;
        List<Node> neighbourList = new();

        if(rightIndex < nodes.Count)
        {
            int gValTmp = nodes[currNodeIndex].gVal + 1;
            int hValTmp = ManhattanDistance(nodes[rightIndex].nodePos);
            int fValTmp = gValTmp + hValTmp;

            if(fValTmp < nodes[rightIndex].fVal || nodes[rightIndex].fVal == 0)
                nodes[rightIndex].fVal = fValTmp;

            neighbourList.Add(nodes[rightIndex]);
        }
        if (leftIndex >= 0)
        {
            int gValTmp = nodes[currNodeIndex].gVal + 1;
            int hValTmp = ManhattanDistance(nodes[leftIndex].nodePos);
            int fValTmp = gValTmp + hValTmp;

            if (fValTmp < nodes[leftIndex].fVal || nodes[leftIndex].fVal == 0)
                nodes[leftIndex].fVal = fValTmp;
            neighbourList.Add(nodes[leftIndex]);
        }
        if(downIndex < nodes.Count)
        {
            int gValTmp = nodes[currNodeIndex].gVal + 1;
            int hValTmp = ManhattanDistance(nodes[downIndex].nodePos);
            int fValTmp = gValTmp + hValTmp;

            if (fValTmp < nodes[downIndex].fVal || nodes[downIndex].fVal == 0)
                nodes[downIndex].fVal = fValTmp;
            neighbourList.Add(nodes[downIndex]);
        }
        if (upIndex >= 0 )
        {
            int gValTmp = nodes[currNodeIndex].gVal + 1;
            int hValTmp = ManhattanDistance(nodes[upIndex].nodePos);
            int fValTmp = gValTmp + hValTmp;

            if (fValTmp < nodes[upIndex].fVal || nodes[upIndex].fVal == 0)
                nodes[upIndex].fVal = fValTmp;
            neighbourList.Add(nodes[upIndex]);
        }
        foreach(Node node in neighbourList)
        {
            if(!node.isOnClosed)
                node.parentNode = nodes[currNodeIndex];
        }
        return neighbourList;

    }

    private int ManhattanDistance(Vector2 nodePos)
    {
        return (int)Mathf.Abs(nodePos.x - goalP.x) + (int)Mathf.Abs(nodePos.y- goalP.y);
    }

 
    private void PrintPath2(List<Node> pathNodes)
    {
        Node currentNode = closedNodes.LastOrDefault();
        for (int i = 0; i<closedNodes.Count; i++)
        {
            if (currentNode == null)
                break;
            pathNodes.Add(currentNode);
            //Debug.LogError("Current: " + currentNode.nodePos + ", Parent: " + currentNode.parentNode.nodePos);
            currentNode = currentNode.parentNode;
        }
        pathNodes.Reverse();
        foreach (Node node in pathNodes)
        {
            Debug.LogError(node.nodePos);
        }

    }


    private IEnumerator PrintPath(List<Node> pathNodes)
    {
        Node currentNode = closedNodes.LastOrDefault();
        while (currentNode != null)
        {
            pathNodes.Add(currentNode);
            currentNode = currentNode.parentNode;
            yield return null; // This will make Unity wait for the next frame before continuing the loop
        }
        pathNodes.Reverse();
        foreach (Node node in pathNodes)
        {
            Debug.LogError(node.nodePos);
            yield return null; // This will make Unity wait for the next frame before continuing the loop
        }
    }
}

[System.Serializable]
public class Node
{
    public Vector2 nodePos;
    public Node parentNode;
    public int nodeIndex;
    public bool isOnClosed = false;
    public int fVal;
    public int gVal;
    public int hVal;


}
