
using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.ObjectChangeEventStream;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;

public class SoldierMove : MonoBehaviour
{


    private float totalDist;
    public float speed;
    private float startTime;
    public bool isSprinting=false;
    //public float interpolationRatio;
    public Animator anim;
    private LineRenderer lineRenderer;


    [Header("PathFinding")]
    [SerializeField]
    private Vector2 startP;
    [SerializeField]
    private Vector2 goalP;
    [SerializeField]
    private int sizeX, sizeY;

    public bool isFound = false;

    public bool isPathGenerated = false;



    public List<Node> closedNodes = new List<Node>();
    public List<Node> pathNodes = new();
   

    public FastPriorityQueue<Node> priorityqueue;
    public Dictionary<int, Node> nodeIndToNode = new();
    private void Awake()
    {
       lineRenderer = GetComponentInChildren<LineRenderer>();
        priorityqueue = new(sizeX * sizeY);

    }
    void Start()
    {
        totalDist = 1;
        startTime = Time.time;

        startP = new Vector2(startP.x - 4.5f,  4.5f - startP.y);
        goalP = new Vector2(goalP.x - 4.5f, 4.5f - goalP.y );


    }



    public void AstarWithVectorsQueue(Vector2 startPos, Vector2 endPos)
    {
        pathNodes.Clear();
        closedNodes.Clear();
        priorityqueue.Clear();
        nodeIndToNode.Clear();

        foreach (Node node in Builder.Instance.Nodes)
        {
            node.isOnClosed = false;
            node.parentNode = null;
            node.hVal = 0;
            node.gVal = 0;
            node.fVal = 0;

        }

        AddInitialNodeToOpen(startPos);

        StartCoroutine(AStar6(endPos));
    }
    private void AddInitialNodeToOpen(Vector2 startPos)
    {

        Node node = Builder.Instance.Nodes.Find(x => x.nodePos.x == startPos.x && x.nodePos.y == startPos.y);
        node.parentNode = null;
        priorityqueue.Enqueue(node, node.fVal);
        nodeIndToNode.Add(node.nodeIndex, node);

    }


    
    public IEnumerator AStar6(Vector2 endPos)
    {
        isFound = false;
        while (!isFound)
        {


            Node currNode = priorityqueue.Dequeue();

            if (currNode.nodePos == endPos)
            {
                isFound = true;
                closedNodes.Add(currNode);
                continue;
            }

            FindNeighbours3(currNode.nodeIndex);
            closedNodes.Add(currNode);
            currNode.isOnClosed = true;

            yield return null;
        }
        if (isFound)
        {
            PrintPath3(pathNodes);
        }
    }
   

    

    private void FindNeighbours3(int currNodeIndex)
    {
        int[] neighbourIndexes = new int[4];
        List<Node> neighbour = new List<Node>();

        //Sag Kontrol satirin soluna gecmesin
        if ((currNodeIndex) % sizeX == 0)
            neighbourIndexes[0] = -1;
        else
            neighbourIndexes[0] = currNodeIndex - 1;

        //Sol kontrol satirin obur tarafina gecmesin.
        if ((currNodeIndex + 1) % sizeX == 0)
            neighbourIndexes[1] = -1;
        else
            neighbourIndexes[1] = currNodeIndex + 1;

        neighbourIndexes[2] = currNodeIndex + sizeX; // asagi
        neighbourIndexes[3] = currNodeIndex - sizeX; //yukari

        foreach (int i in neighbourIndexes)
        {
            if (i < Builder.Instance.Nodes.Count && i >= 0 && CheckCol(Builder.Instance.Nodes[i].nodePos) && !Builder.Instance.Nodes[i].isOnClosed)
            {
                int gValTmp = Builder.Instance.Nodes[currNodeIndex].gVal + 1;
                int hValTmp = ManhattanDistance(Builder.Instance.Nodes[i].nodePos);
                int fValTmp = gValTmp + hValTmp;


                if (nodeIndToNode.TryGetValue(i, out Node node))
                {
                    if (Builder.Instance.Nodes[i].gVal > gValTmp)
                    {
                        // If the new path to the node is shorter, update the node
                        Builder.Instance.Nodes[i].hVal = hValTmp;
                        Builder.Instance.Nodes[i].gVal = gValTmp;
                        Builder.Instance.Nodes[i].fVal = fValTmp;
                        Builder.Instance.Nodes[i].parentNode = Builder.Instance.Nodes[currNodeIndex];

                        priorityqueue.UpdatePriority(node, fValTmp);
                    }
                }
                else
                {
                    // If the node is not in the open list, add it
                    Builder.Instance.Nodes[i].hVal = hValTmp;
                    Builder.Instance.Nodes[i].gVal = gValTmp;
                    Builder.Instance.Nodes[i].fVal = fValTmp;
                    Builder.Instance.Nodes[i].parentNode = Builder.Instance.Nodes[currNodeIndex];

                    nodeIndToNode.Add(Builder.Instance.Nodes[i].nodeIndex, Builder.Instance.Nodes[i]);
                    priorityqueue.Enqueue(Builder.Instance.Nodes[i], Builder.Instance.Nodes[i].fVal);
                }



            }
        }
       
    }
    private bool CheckCol(Vector2 pos)
    {
        return Builder.Instance.collisionMat[(int)(4.5f - pos.y), (int)(pos.x + 4.5f)] == -1;
    }
    private int ManhattanDistance(Vector2 nodePos)
    {
        return (int)Mathf.Abs(nodePos.x - goalP.x) + (int)Mathf.Abs(nodePos.y - goalP.y);
    }
    private IEnumerator Move()
    {
        int i = 0;
        anim.Play("sprint");
        isSprinting = true;
        transform.rotation = Quaternion.identity;
        float startTime = Time.time;
        float interpolationRatio = 0;
        while (i < pathNodes.Count - 1)
        {
            Vector3 currNodePos = new Vector3(pathNodes[i].nodePos.x, -1, pathNodes[i].nodePos.y);
            Vector3 nextNodePos = new Vector3(pathNodes[i + 1].nodePos.x, -1, pathNodes[i + 1].nodePos.y);

            startTime = Time.time;
            while (interpolationRatio < 1)
            {
                Vector3 targetDirection = nextNodePos - transform.position;
                if (Vector3.Dot(transform.forward, targetDirection) != 1)
                {
                  
                    Quaternion rotation = Quaternion.LookRotation(targetDirection);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, interpolationRatio);

                }

                interpolationRatio = (Time.time - startTime) * speed / totalDist;
                transform.position = Vector3.Lerp(currNodePos, nextNodePos, interpolationRatio);

                yield return null;

            }
            interpolationRatio = 0;
            i++;
            
            yield return null;
        }
        anim.Play("idle");
        isSprinting = false;
    }

    
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
        pathNodes.AddRange(pathStack);

      
        closedNodes.Clear();
        priorityqueue.Clear();
        nodeIndToNode.Clear();

        foreach (Node node in Builder.Instance.Nodes)
        {
            node.isOnClosed = false;
            node.parentNode = null;
            node.hVal = 0;
            node.gVal = 0;
            node.fVal = 0;

        }

        isPathGenerated = true;


        RenderPath();
        StartCoroutine(Move());

    }

    private void RenderPath()
    {
        lineRenderer.positionCount = pathNodes.Count;
       for(int i =0;i < pathNodes.Count;i++)
        {
            lineRenderer.SetPosition(i, new Vector3(pathNodes[i].nodePos.x,-1, pathNodes[i].nodePos.y));
        }
    }
}
