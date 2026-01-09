using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PathFinding : MonoBehaviour
{
    [SerializeField] private Grid grid;
 
    public void FindPath(PathRequest request, Action<PathResult> callback)
    {
        Vector3[] wayPoints = new Vector3[0];
        bool isSuccess = false;
        
        Node startNode = grid.NodeFromWorldPoint(request.pathStart);
        Node targetNode = grid.NodeFromWorldPoint(request.pathEnd);

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    isSuccess = true;
                    break;
                }

                List<Node> neighbours = grid.GetNeighbours(currentNode);

                foreach (Node neighbour in neighbours)
                {
                    if (closedSet.Contains(neighbour) || !neighbour.walkable)
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parentNode = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        
        if (isSuccess)
        {
            wayPoints = RetracePath(startNode, targetNode);
            
            isSuccess = wayPoints.Length > 0;
        }
        
        PathResult pathResult = new PathResult(wayPoints, isSuccess, request.callback);
        callback?.Invoke(pathResult);
    }

    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        
        Vector3[] wayPoints = SimplifyPath(path);
        Array.Reverse(wayPoints);
        
        return wayPoints;
    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> wayPoints = new List<Vector3>();
        Vector2 lastDir = Vector2.zero;

        for (int idx = 1; idx < path.Count; idx++)
        {
            Node prevNode = path[idx - 1];
            Node node = path[idx];

            Vector2Int prevNodeCoord = prevNode.girdCoord;
            Vector2Int nodeCoord = node.girdCoord;
            
            Vector2 dir = nodeCoord -  prevNodeCoord;

            if (dir != lastDir)
            {
                wayPoints.Add(node.worldPosition);
            }
            
            lastDir = dir;
        }
        
        return wayPoints.ToArray();
    }
    
    private int GetDistance(Node a, Node b)
    {
        Vector2Int aCoord = a.girdCoord;
        Vector2Int bCoord = b.girdCoord;
        Vector2Int delta = bCoord - aCoord;
        delta.x = Mathf.Abs(delta.x);
        delta.y = Mathf.Abs(delta.y);

        int dist = 0;
        
        if (delta.x > delta.y)
        {
            dist = 14 * (delta.y) + 10 * (delta.x - delta.y);
        }
        else
        {
            dist = 10 * (delta.x) + 10 * (delta.y - delta.x);
        }

        return dist;
    }
}
