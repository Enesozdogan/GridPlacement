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
  
    public bool isPathGenerated = false;
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
       

    }
    public void AstarUI()
    {
        int randStartx = UnityEngine.Random.Range(0, 5);
        int randStarty = UnityEngine.Random.Range(0, 5);
        int randGoalx = UnityEngine.Random.Range(0, 5);
        int randGoaly = UnityEngine.Random.Range(0, 5);
       
        Vector2 startTmp = new Vector2(randStartx, randStarty);
        Vector2 goalTmp = new Vector2(randGoalx, randGoaly);
        GeneratePath(startTmp ,goalTmp , pathNodes =>
        {
            if(pathNodes == null)
            {
                Debug.LogError("Path yok");
            }
            else
            {
                Debug.LogError("Path var");
            }
        });
    }
    public void AstarWithVectors()
    {
        pathNodes.Clear();
        openNodes.Clear();
        closedNodes.Clear();

        AddInitialNodeToOpen();

        StartCoroutine(AStar3());
    }
   public void GeneratePath(Vector2 startP,  Vector2 goalP, Action<List<Node>> pathCallback)
    {
        pathNodes.Clear();
        openNodes.Clear();
        closedNodes.Clear();

        this.startP = startP;
        this.goalP = goalP;


        AddInitialNodeToOpen();

        StartCoroutine(AStar2(pathCallback));

        //if (isPathGenerated)
        //    return pathNodes;
        //else
        //    return null;

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
    public IEnumerator AStar2(Action<List<Node>> pathCallback)
    {
        isFound = false;

        while (!isFound)
        {
            Node currNode = openNodes.OrderBy(x => x.fVal).FirstOrDefault();
            if (currNode.nodePos == goalP)
            {
                isFound = true;
                closedNodes.Add(currNode);
                continue;
            }

            List<Node> neighbours = FindNeighbour(currNode.nodeIndex);

            foreach (Node node in neighbours)
            {
                int foundIndex = openNodes.FindIndex(x => x.nodeIndex == node.nodeIndex);
                int foundClosed = closedNodes.FindIndex(x => x.nodeIndex == node.nodeIndex);

                //If already in closed list skip this neighbour
                if (foundClosed >= 0)
                    continue;
                //If not check if its in open list if found compare f value
                if (builder.collisionMat[(int)(node.nodePos.x + 4.5f), (int)(node.nodePos.y + 4.5f)] == -1)
                {
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

            }
            openNodes.Remove(currNode);
            closedNodes.Add(currNode);
            currNode.isOnClosed = true;

            yield return null;
        }
        if (isFound)
        {
            StartCoroutine(PrintPath(pathNodes));
            if(isPathGenerated)
                pathCallback(pathNodes);
        }
        
    }

    public IEnumerator AStar3()
    {
        isFound = false;
        while (!isFound)
        {
            Node currNode = openNodes.OrderBy(x => x.fVal).FirstOrDefault();
            if (currNode.nodePos == goalP)
            {
                isFound = true;
                closedNodes.Add(currNode);
                continue;
            }

            List<Node> neighbours = FindNeighbour(currNode.nodeIndex);

            foreach (Node node in neighbours)
            {
                int foundIndex = openNodes.FindIndex(x => x.nodeIndex == node.nodeIndex);
                int foundClosed = closedNodes.FindIndex(x => x.nodeIndex == node.nodeIndex);

                //If already in closed list skip this neighbour
                if (foundClosed >= 0)
                    continue;
                //If not check if its in open list if found compare f value
                if (builder.collisionMat[(int)(node.nodePos.x + 4.5f), (int)(node.nodePos.y + 4.5f)] == -1)
                {
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

            }
            openNodes.Remove(currNode);
            closedNodes.Add(currNode);
            currNode.isOnClosed = true;

            yield return null;
        }
        if (isFound)
            StartCoroutine(PrintPath(pathNodes));
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


    private IEnumerator PrintPath(List<Node> pathNodes)
    {
        isPathGenerated = false;
        Node currentNode = closedNodes.LastOrDefault();
        while (currentNode != null)
        {
            pathNodes.Add(currentNode);
            currentNode = currentNode.parentNode;
            yield return null; // This will make Unity wait for the next frame before continuing the loop
        }
        pathNodes.Reverse();

        isPathGenerated = true;

        foreach (Node node in pathNodes)
        {
            Debug.LogError(node.nodePos);
            yield return null; // This will make Unity wait for the next frame before continuing the loop
        }
    }
}


//public class Node
//{
//    public Vector2 nodePos;
//    public Node parentNode;
//    public int nodeIndex;
//    public bool isOnClosed = false;
//    public int fVal;
//    public int gVal;
//    public int hVal;


//}
