using System;
using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float turnDist;
    [SerializeField] private float stoppingDist;

    private const float pathUpdateMoveThreshold = 0.5f;
    private const float minPathUpdateDelay = 0.5f;
    
    private TravelPath travelPath;
    
    private Coroutine moveCoroutine;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(UpdatePath());
    }

    private void OnPathFound(Vector3[] path, bool success)
    {
        if (success)
        {
            travelPath = new TravelPath(path, transform.position, turnDist, stoppingDist);

            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            moveCoroutine = StartCoroutine(FollowPath());
        }
    }

    private IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 0.3f)
        {    
            yield return new WaitForSeconds(0.3f);
        }
        
        PathRequest request = new PathRequest(transform.position, target.transform.position, OnPathFound);
        PathRequestManager.RequestPath(request);
        
        float sqMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;
        
        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateDelay);
            
            if ((target.position - targetPosOld).sqrMagnitude >= sqMoveThreshold)
            {
                PathRequest _request = new PathRequest(transform.position, target.transform.position, OnPathFound);
                PathRequestManager.RequestPath(_request);
                targetPosOld = target.position;
            }
        }
    }

    private IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        
        Vector3 dir = travelPath.lookPoints[0] - transform.position;
        dir.Normalize();
        float finalRotationAngleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion finalRotation = Quaternion.Euler(0.0f, 0.0f, finalRotationAngleZ);
        transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, Time.deltaTime * 50.0f);

        float speedPercent = 1.0f;
        
        while (followingPath)
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            while (travelPath.turnBoundaries[pathIndex].HasCrossedLine(pos))
            {
                if (pathIndex == travelPath.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                if (pathIndex >= travelPath.slowDownIndex && stoppingDist > 0)
                {
                    speedPercent = travelPath.turnBoundaries[travelPath.finishLineIndex].DistFromPoint(pos) /
                                   stoppingDist;
                    speedPercent = Mathf.Clamp01(speedPercent);

                    if (speedPercent <= 0.05f)
                    {
                        followingPath = false;
                    }
                }

                dir = travelPath.lookPoints[pathIndex] - transform.position;
                dir.Normalize();
                finalRotationAngleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                finalRotation = Quaternion.Euler(0.0f, 0.0f, finalRotationAngleZ);
                transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, Time.deltaTime * 50.0f);
                
                transform.Translate(Vector3.right * moveSpeed * speedPercent * Time.deltaTime, Space.Self);
            }
            
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (travelPath != null)
        {
            travelPath.DrawWithGizmos();
        }
    }
}
