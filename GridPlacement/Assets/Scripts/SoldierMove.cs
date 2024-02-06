using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.ObjectChangeEventStream;
using static UnityEditor.PlayerSettings;

public class SoldierMove : MonoBehaviour
{
  

    private float totalDist;
    public float speed;
    private float startTime;
    public bool isSprinting=true;
    //public float interpolationRatio;
    public Animator anim;



    [Header("PathFinding")]
    [SerializeField]
    private Vector2 startP;
    [SerializeField]
    private Vector2 goalP;
    [SerializeField]
    private int sizeX, sizeY;

    public bool isFound = false;

    public bool isPathGenerated = false;


    public List<Node> openNodes = new List<Node>();
    public List<Node> closedNodes = new List<Node>();
    public List<Node> pathNodes = new();
    
    private void Awake()
    {
        
    }
    void Start()
    {
        totalDist = 1;
        startTime = Time.time;

        startP = new Vector2(startP.x - 4.5f,  4.5f - startP.y);
        goalP = new Vector2(goalP.x - 4.5f, 4.5f - goalP.y );


    }
    public void AstarWithVectors()
    {
        pathNodes.Clear();
        openNodes.Clear();
        closedNodes.Clear();

        foreach (Node node in Builder.Instance.Nodes)
        {
            node.isOnClosed = false;
        
        }

        AddInitialNodeToOpen();

        StartCoroutine(AStar3());
    }
    private void AddInitialNodeToOpen()
    {

        Node node = Builder.Instance.Nodes.Find(x => x.nodePos.x == startP.x && x.nodePos.y == startP.y);
        node.parentNode = null;
        openNodes.Add(node);

    }

    private IEnumerator Move()
    {
        int i = 0;
        anim.Play("sprint");
        transform.rotation = Quaternion.identity;
        float startTime = Time.time;
        float interpolationRatio=0;
        while (i< pathNodes.Count-1)
        {
            Vector3 currNodePos = new Vector3(pathNodes[i].nodePos.x, 0, pathNodes[i].nodePos.y);
            Vector3 nextNodePos = new Vector3(pathNodes[i + 1].nodePos.x, 0, pathNodes[i + 1].nodePos.y);

            startTime = Time.time; 
            while(interpolationRatio < 1)
            {
                Vector3 targetDirection = nextNodePos - transform.position;
                if (Vector3.Dot(transform.forward, targetDirection)!=1)
                {
                     // Get the direction to the target
                    // Keep only the horizontal direction
                    Quaternion rotation = Quaternion.LookRotation(targetDirection);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, interpolationRatio);

                }

                interpolationRatio = (Time.time - startTime) * speed / totalDist;
                transform.position = Vector3.Lerp(currNodePos, nextNodePos, interpolationRatio);

                yield return null;

            }
            interpolationRatio = 0; 
            i++;
            isSprinting = interpolationRatio >= 1 ? false : true;
            yield return null;
        }
        anim.Play("idle");
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
                //int foundClosed = closedNodes.FindIndex(x => x.nodeIndex == node.nodeIndex);

                //If already in closed list skip this neighbour
                if (node.isOnClosed)
                    continue;
                //If not check if its in open list if found compare f value
                if (Builder.Instance.collisionMat[(int)(4.5f - currNode.nodePos.y), (int)(currNode.nodePos.x + 4.5f)] == -1)
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
                else
                {
                    Debug.Log(node);
                }

            }
            openNodes.Remove(currNode);
            closedNodes.Add(currNode);
            currNode.isOnClosed = true;

            yield return null;
        }
        if (isFound)
        {
            PrintPath3(pathNodes);
        }
    }

    private bool CheckCol(Vector2 pos)
    {
        return Builder.Instance.collisionMat[(int)( 4.5f - pos.y), (int)(pos.x + 4.5f)] == -1;
    }
    private List<Node> FindNeighbour(int currNodeIndex)
    {
        int rightIndex = currNodeIndex + 1;
        int leftIndex = currNodeIndex - 1;
        int downIndex = currNodeIndex + sizeX;
        int upIndex = currNodeIndex - sizeX;
        List<Node> neighbourList = new();

        if (rightIndex < Builder.Instance.Nodes.Count && CheckCol(Builder.Instance.Nodes[rightIndex].nodePos))
        {
            int gValTmp = Builder.Instance.Nodes[currNodeIndex].gVal + 1;
            int hValTmp = ManhattanDistance(Builder.Instance.Nodes[rightIndex].nodePos);
            int fValTmp = gValTmp + hValTmp;

            if (fValTmp < Builder.Instance.Nodes[rightIndex].fVal || Builder.Instance.Nodes[rightIndex].fVal == 0)
                Builder.Instance.Nodes[rightIndex].fVal = fValTmp;

            if (!Builder.Instance.Nodes[rightIndex].isOnClosed)
                Builder.Instance.Nodes[rightIndex].parentNode = Builder.Instance.Nodes[currNodeIndex];

            neighbourList.Add(Builder.Instance.Nodes[rightIndex]);
        }
        if (leftIndex >= 0 && CheckCol(Builder.Instance.Nodes[leftIndex].nodePos))
        {
            int gValTmp = Builder.Instance.Nodes[currNodeIndex].gVal + 1;
            int hValTmp = ManhattanDistance(Builder.Instance.Nodes[leftIndex].nodePos);
            int fValTmp = gValTmp + hValTmp;

            if (fValTmp < Builder.Instance.Nodes[leftIndex].fVal || Builder.Instance.Nodes[leftIndex].fVal == 0)
                Builder.Instance.Nodes[leftIndex].fVal = fValTmp;

            if (!Builder.Instance.Nodes[leftIndex].isOnClosed)
                Builder.Instance.Nodes[leftIndex].parentNode = Builder.Instance.Nodes[currNodeIndex];

            neighbourList.Add(Builder.Instance.Nodes[leftIndex]);
        }
        if (downIndex < Builder.Instance.Nodes.Count && CheckCol(Builder.Instance.Nodes[downIndex].nodePos))
        {
            int gValTmp = Builder.Instance.Nodes[currNodeIndex].gVal + 1;
            int hValTmp = ManhattanDistance(Builder.Instance.Nodes[downIndex].nodePos);
            int fValTmp = gValTmp + hValTmp;

            if (fValTmp < Builder.Instance.Nodes[downIndex].fVal || Builder.Instance.Nodes[downIndex].fVal == 0)
                Builder.Instance.Nodes[downIndex].fVal = fValTmp;

            if (!Builder.Instance.Nodes[downIndex].isOnClosed)
                Builder.Instance.Nodes[downIndex].parentNode = Builder.Instance.Nodes[currNodeIndex];

            neighbourList.Add(Builder.Instance.Nodes[downIndex]);
        }
        if (upIndex >= 0 && CheckCol(Builder.Instance.Nodes[upIndex].nodePos))
        {
            int gValTmp = Builder.Instance.Nodes[currNodeIndex].gVal + 1;
            int hValTmp = ManhattanDistance(Builder.Instance.Nodes[upIndex].nodePos);
            int fValTmp = gValTmp + hValTmp;

            if (fValTmp < Builder.Instance.Nodes[upIndex].fVal || Builder.Instance.Nodes[upIndex].fVal == 0)
                Builder.Instance.Nodes[upIndex].fVal = fValTmp;

            if (!Builder.Instance.Nodes[upIndex].isOnClosed)
                Builder.Instance.Nodes[upIndex].parentNode = Builder.Instance.Nodes[currNodeIndex];

            neighbourList.Add(Builder.Instance.Nodes[upIndex]);
        }
     
        return neighbourList;

    }

    private int ManhattanDistance(Vector2 nodePos)
    {
        return (int)Mathf.Abs(nodePos.x - goalP.x) + (int)Mathf.Abs(nodePos.y - goalP.y);
    }


    //private IEnumerator PrintPath(List<Node> pathNodes)
    //{
    //    isPathGenerated = false;
    //    Node currentNode = closedNodes.LastOrDefault();
    //    while (currentNode != null)
    //    {
    //        pathNodes.Add(currentNode);
    //        currentNode = currentNode.parentNode;
    //        yield return null; // This will make Unity wait for the next frame before continuing the loop
    //    }
    //    pathNodes.Reverse();

    //    isPathGenerated = true;

    //    StartCoroutine(Move());
    //}
    //private IEnumerator PrintPath2(List<Node> pathNodes)
    //{
    //    isPathGenerated = false;
    //    Node currentNode = closedNodes.LastOrDefault();
    //    int nodesPerFrame = 1; // Adjust this value to add more or fewer nodes per frame

    //    Stack<Node> pathStack = new Stack<Node>();

    //    while (currentNode != null)
    //    {
    //        for (; currentNode != null;)
    //        {
    //            pathStack.Push(currentNode);
    //            currentNode = currentNode.parentNode;
    //        }

    //        yield return null; // This will make Unity wait for the next frame before continuing the loop
    //    }

    //    pathNodes.AddRange(pathStack); // Add the elements from the stack to the list
    //    isPathGenerated = true;
    //    StartCoroutine(Move());
    //}

    private void PrintPath3(List<Node> pathNodes)
    {
        isPathGenerated = false;
        Node currentNode = closedNodes[closedNodes.Count-1];
        Stack<Node> pathStack = new Stack<Node>();
        for (; currentNode != null;)
        {
            pathStack.Push(currentNode);
           
            
            currentNode = currentNode.parentNode;
        }
        pathNodes.AddRange(pathStack); // Add the elements from the stack to the list

        foreach (var node in pathNodes)
        {
            int i = (int)( 4.5f - node.nodePos.y);
            int j = (int)(node.nodePos.x + 4.5f);
            Debug.Log("i: " + i + " j: " + j);
        }
        isPathGenerated = true;
        StartCoroutine(Move());

    }



}
