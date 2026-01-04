using UnityEngine;

[System.Serializable]
public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public Vector2Int girdCoord;
    public int movementPenalty;

    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;

    public int heapIdx { get; set; }
    
    public Node parentNode;

    public Node(bool _walkable, Vector3 _worldPosition,  Vector2Int _girdCoord, int _movementPenalty)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        girdCoord = _girdCoord;
        movementPenalty = _movementPenalty;
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        
        return -compare;
    }
}
