using System;
using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float moveSpeed;
    private Vector3[] path;
    private int targetIndex;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.transform.position, OnPathFound);
    }

    private void OnPathFound(Vector3[] path, bool success)
    {
        this.path = path;

        StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;

                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                
                currentWaypoint = path[targetIndex];
            }
            
            transform.position  = Vector3.MoveTowards(transform.position, currentWaypoint, moveSpeed * Time.deltaTime);
            
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null && path.Length > 0)
        {
            Gizmos.color = Color.green;

            if (targetIndex < path.Length)
            {
                Gizmos.DrawLine(transform.position, path[targetIndex]);
            }

            for (int i = targetIndex; i < path.Length - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]); 
            }
        }
    }
}
