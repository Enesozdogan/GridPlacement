
using Priority_Queue;
using System;
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
    public FastPriorityQueue<Node> priorityqueue = new(100);
    
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
        priorityqueue.Clear();
        foreach (Node node in Builder.Instance.Nodes)
        {
            node.isOnClosed = false;
        
        }

        AddInitialNodeToOpen();

        StartCoroutine(AStar3());
    }
    public void AstarWithVectorsQueue()
    {
        pathNodes.Clear();
        openNodes.Clear();
        closedNodes.Clear();
        priorityqueue.Clear();
        foreach (Node node in Builder.Instance.Nodes)
        {
            node.isOnClosed = false;

        }

        AddInitialNodeToOpen();

        StartCoroutine(AStar4());
    }
    private void AddInitialNodeToOpen()
    {

        Node node = Builder.Instance.Nodes.Find(x => x.nodePos.x == startP.x && x.nodePos.y == startP.y);
        node.parentNode = null;
        openNodes.Add(node);

        //Priority Queue denemesi
        priorityqueue.Enqueue(node, node.fVal);
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

            List<Node> neighbours = FindNeighbours2(currNode.nodeIndex);

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
    public IEnumerator AStar4()
    {
        isFound = false;
        while (!isFound)
        {
            Debug.LogWarning(priorityqueue.Count);
            Node currNode = priorityqueue.Dequeue();
            if (currNode.nodePos == goalP)
            {
                isFound = true;
                closedNodes.Add(currNode);
                continue;
            }

            List<Node> neighbours = FindNeighbours2(currNode.nodeIndex);

            foreach (Node node in neighbours)
            {
                
                Node foundNode = priorityqueue.Where(x => x.nodeIndex == node.nodeIndex).FirstOrDefault();

                //int foundClosed = closedNodes.FindIndex(x => x.nodeIndex == node.nodeIndex);

                //If already in closed list skip this neighbour
                if (node.isOnClosed)
                    continue;
                //If not check if its in open list if found compare f value
                if (Builder.Instance.collisionMat[(int)(4.5f - currNode.nodePos.y), (int)(currNode.nodePos.x + 4.5f)] == -1)
                {
                    if (foundNode != null)
                    {
                        if (foundNode.fVal > node.fVal)
                            priorityqueue.UpdatePriority(foundNode, node.fVal);
                    }
                    else
                    {
                        priorityqueue.Enqueue(node, node.fVal);
                    }
                }
                else
                {
                    Debug.Log(node);
                }

            }
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

    private List<Node> FindNeighbours2(int currNodeIndex)
    {
        int[] neighbourIndexes = new int[4];
        List<Node> neighbour = new List<Node>();

        //Sag Kontrol satirin soluna gecmesin
        if((currNodeIndex)%10 == 0)
            neighbourIndexes[0] = -1;
        else
            neighbourIndexes[0] = currNodeIndex -1;

        //Sol kontrol satirin obur tarafina gecmesin.
        if((currNodeIndex + 1) % 10 == 0)
            neighbourIndexes[1] = -1;
        else
            neighbourIndexes[1] = currNodeIndex + 1;

        neighbourIndexes[2] = currNodeIndex +10; // asagi
        neighbourIndexes[3] = currNodeIndex -10; //yukari

        foreach (int i in neighbourIndexes)
        {
            if(i < Builder.Instance.Nodes.Count && i>= 0 && CheckCol(Builder.Instance.Nodes[i].nodePos))
            {
                int gValTmp = Builder.Instance.Nodes[currNodeIndex].gVal + 1;
                int hValTmp = ManhattanDistance(Builder.Instance.Nodes[i].nodePos);
                int fValTmp = gValTmp + hValTmp;

                if (fValTmp < Builder.Instance.Nodes[i].fVal || Builder.Instance.Nodes[i].fVal == 0)
                    Builder.Instance.Nodes[i].fVal = fValTmp;

                if (!Builder.Instance.Nodes[i].isOnClosed)
                    Builder.Instance.Nodes[i].parentNode = Builder.Instance.Nodes[currNodeIndex];

                neighbour.Add(Builder.Instance.Nodes[i]);

            }
        }
        return neighbour;
    }
    private int ManhattanDistance(Vector2 nodePos)
    {
        return (int)Mathf.Abs(nodePos.x - goalP.x) + (int)Mathf.Abs(nodePos.y - goalP.y);
    }
    private IEnumerator Move()
    {
        int i = 0;
        anim.Play("sprint");
        transform.rotation = Quaternion.identity;
        float startTime = Time.time;
        float interpolationRatio = 0;
        while (i < pathNodes.Count - 1)
        {
            Vector3 currNodePos = new Vector3(pathNodes[i].nodePos.x, 0, pathNodes[i].nodePos.y);
            Vector3 nextNodePos = new Vector3(pathNodes[i + 1].nodePos.x, 0, pathNodes[i + 1].nodePos.y);

            startTime = Time.time;
            while (interpolationRatio < 1)
            {
                Vector3 targetDirection = nextNodePos - transform.position;
                if (Vector3.Dot(transform.forward, targetDirection) != 1)
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
