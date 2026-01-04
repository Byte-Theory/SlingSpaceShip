using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainType
{
    public LayerMask terrainMask;
    public int terrainPenalty;
}

public class Grid : MonoBehaviour
{
    public Transform playerT;
    public LayerMask unwalkableLayerMask;
    public Vector2 gridWorldSize;
    public float nodeSize;
    private Node[,] grid;
    
    public TerrainType[] allTerrainTypes;
    public LayerMask walkableMask;
    private Dictionary<int, int> walkableTerrains = new Dictionary<int, int>();
    
    private int gridSizeX, gridSizeY;

    public bool showGridGizmo;
    
    void Awake()
    {
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeSize);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeSize);

        for (int i = 0; i < allTerrainTypes.Length; i++)
        {
            walkableMask.value |= allTerrainTypes[i].terrainMask.value;
            
            walkableTerrains.Add((int)Mathf.Log(allTerrainTypes[i].terrainMask.value, 2), allTerrainTypes[i].terrainPenalty);
        }
        
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
        
        for (int x = 0; x < gridSizeX; x++) 
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 nodePos = worldBottomLeft + Vector3.right * (x * nodeSize + nodeSize / 2) + Vector3.up * (y * nodeSize + nodeSize / 2);
                bool walkable = !Physics.CheckSphere(nodePos, nodeSize * 0.5f, unwalkableLayerMask);
                
                int movementPenalty = 0;

                if (walkable)
                {
                    if (Physics.Raycast(nodePos - Vector3.forward, Vector3.forward, out RaycastHit hit, 100,
                            walkableMask))
                    {
                        walkableTerrains.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }
                }

                grid[x, y] = new Node(walkable, nodePos, new Vector2Int(x, y), movementPenalty);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                
                int checkX = node.girdCoord.x + x;
                int checkY = node.girdCoord.y + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        
        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = (worldPos.x + gridSizeX * 0.5f) / gridWorldSize.x;
        float percentY = (worldPos.y + gridSizeY * 0.5f) / gridWorldSize.y;
        
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        
        int x = Mathf.RoundToInt(percentX * (gridSizeX - 1));
        int y = Mathf.RoundToInt(percentY * (gridSizeY - 1));
        
        Node node = grid[x, y];
        return node;
    }

    public int MaxSize => gridSizeX * gridSizeY;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid == null || !showGridGizmo)
        {
            return;
        }
        
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node node = grid[x, y];
                Gizmos.color = node.walkable ? Color.white : Color.red;

                Gizmos.DrawWireCube(node.worldPosition, Vector3.one * (nodeSize - 0.05f));
            }
        }
        
        /* Node playerNode = NodeFromWorldPoint(playerT.position);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(playerNode.worldPosition, Vector3.one * (nodeSize - 0.05f)); */
    }
}
