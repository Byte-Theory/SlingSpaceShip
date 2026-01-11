using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    [Header("Container")]
    [SerializeField] private GameObject container;

    [Header("Enemy Length")]
    [SerializeField] private float enemyLength = 5.0f;
    
    [Header("Chase Data")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float turnDist;
    [SerializeField] private float stoppingDist;

    // State Data
    private Vector3 stateStartPos;
    private Vector3 stateEndPos;
    private Vector3 patrollingLeftPos;
    private Vector3 patrollingRightPos;
    private bool isPatrollingToRight;
    
    // Container
    private float containerOscillationTimer = 0.0f;
    private bool containerOscillate = false;
    private float containerSpeed = 0.0f;
    private float containerAmplitude = 0.0f;
    
    // Target
    private Transform target;
    
    // Enemy
    private Enemy enemy;
    
    // Travel path
    private TravelPath travelPath;
    private Coroutine updatePathCoroutine;
    private Coroutine moveCoroutine;
    
    // Orientation Data
    private float containerXRotation;
    
    private const float pathUpdateMoveThreshold = 0.5f;
    private const float minPathUpdateDelay = 0.5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stoppingDist = Constants.EnemyData.PlayerShootingRange;
    }

    private void Update()
    {
        OscillateContainerUpAndDown();
        UpdateContainerRotation();
    }
    
    #region SetUp

    public void SetUp(Enemy enemy, Transform playerT)
    {
        this.enemy = enemy;
        this.target = playerT;
    }

    #endregion

    #region Idle

    public void SetMovingToIdleData(Vector3 stateStartPos, Vector3 stateEndPos)
    {
        this.stateStartPos = stateStartPos;
        this.stateEndPos = stateEndPos;
    }

    public void UpdateMovingToIdleMovement(float delta)
    {
        transform.position = Vector3.Lerp(stateStartPos, stateEndPos, delta);
    }
    
    #endregion

    #region Patrolling

    public void SetPatrollingData(Vector3 curPos, Vector3 leftPos, Vector3 rightPos)
    {
        stateStartPos = curPos;
        patrollingLeftPos = leftPos;
        patrollingRightPos = rightPos;

        isPatrollingToRight = Random.value > 0.5f;
        
        UpdateLookDirectionWhilePatrolling();
    }

    public void UpdatePatrollingMovement()
    {
        if (isPatrollingToRight)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                patrollingRightPos,
                Time.deltaTime * Constants.EnemyData.EnemyPatrollingMoveSpeed);
            
            float distLeft = Vector3.Distance(transform.position, patrollingRightPos);
            if (distLeft < 0.1f)
            {
                isPatrollingToRight = !isPatrollingToRight;
                UpdateLookDirectionWhilePatrolling();
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position,
                patrollingLeftPos,
                Time.deltaTime * Constants.EnemyData.EnemyPatrollingMoveSpeed);
            
            float distLeft = Vector3.Distance(transform.position, patrollingLeftPos);
            if (distLeft < 0.1f)
            {
                isPatrollingToRight = !isPatrollingToRight;
                UpdateLookDirectionWhilePatrolling();
            }
        }
    }

    private void UpdateLookDirectionWhilePatrolling()
    {
        if (isPatrollingToRight)
        {
            Vector3 dir = patrollingRightPos - transform.position;
            dir.Normalize();
            float finalRotationAngleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion finalRotation = Quaternion.Euler(0.0f, 0.0f, finalRotationAngleZ);
            transform.rotation = finalRotation;
        }
        else
        {
            Vector3 dir = patrollingLeftPos - transform.position;
            dir.Normalize();
            float finalRotationAngleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion finalRotation = Quaternion.Euler(0.0f, 0.0f, finalRotationAngleZ);
            transform.rotation = finalRotation;
        }
    }

    #endregion
    
    #region Chasing Player Logic

    public void StartChasingPlayer()
    {
        if (updatePathCoroutine != null)
        {
            StopCoroutine(updatePathCoroutine);
        }
        
        updatePathCoroutine = StartCoroutine(UpdatePath());
    }

    public void StopChasingPlayer()
    {
        if (updatePathCoroutine != null)
        {
            StopCoroutine(updatePathCoroutine);
        }
        
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
    }
    
    private void OnPathFound(Vector3[] path, bool success)
    {
        Debug.Log(success);
        
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

    #endregion

    #region Container Oscillation

    public void TriggerOscillateContainerUpAndDown(bool oscillate, float speed, float amplitude)
    {
        containerOscillate = oscillate;
        containerSpeed = speed;
        containerAmplitude = amplitude;

        if (!containerOscillate)
        {
            containerOscillationTimer = 0.0f;
        }
    }

    private void OscillateContainerUpAndDown()
    {
        if (containerOscillate)
        {
            containerOscillationTimer += Time.deltaTime;
            Vector3 containerPos = Vector3.zero;
            containerPos.y = Mathf.PingPong(containerOscillationTimer * containerSpeed, containerAmplitude);

            container.transform.localPosition = containerPos;
        }
        else
        {
            container.transform.localPosition = Vector3.MoveTowards(container.transform.localPosition, 
                Vector3.zero,
                Time.deltaTime * 2.0f);
        }
    }

    #endregion
    
    #region Rotation

    private void UpdateContainerRotation()
    {
        Vector3 lookDir = transform.right;

        if (lookDir.x >= 0)
        {
            containerXRotation = 0.0f;
        }
        else if (lookDir.x < 0)
        {
            containerXRotation = 180.0f;
        }

        container.transform.localRotation = Quaternion.Euler(containerXRotation, 0.0f, 0.0f);
    }

    #endregion

    #region Getters

    public float EnemyLength => enemyLength;

    #endregion
    
    private void OnDrawGizmos()
    {
        if (travelPath != null)
        {
            travelPath.DrawWithGizmos();
        }
    }
}
