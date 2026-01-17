using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyStateManager : MonoBehaviour
{
    [Header("Aniamtion Curves")]
    [SerializeField] private AnimationCurve movingToIdleCurve;
    
    // States
    private EnemyStates enemyState = EnemyStates.Unknown;
    private EnemyStates prevState = EnemyStates.Unknown;
    
    // Duration
    private float stateTimeElapsed = 0.0f;
    private float stateDuration = 0.0f;

    // Enemy
    private Enemy enemy;
    
    #region Unity Functions

    private void Update()
    {
        UpdateStateTimer();
    }

    #endregion
    
    #region SetUp

    public void SetUp(Enemy enemy)
    {
        this.enemy = enemy;
        
        SetEnemyState(EnemyStates.MovingToIdle);
    }

    #endregion

    #region States

    private void SetEnemyState(EnemyStates newState, bool forceSet = false)
    {
        if (newState == enemyState && !forceSet)
        {
            return;
        }

        SetStateData(newState);
        
        prevState = enemyState;
        enemyState = newState;
        
        stateTimeElapsed = 0.0f;
    }
    
    private void SetStateData(EnemyStates newState)
    {
        switch (newState)
        {
            case EnemyStates.GoingBackToSpawn:
            {
                enemy.enemyMovement.StopPathMovement();
                enemy.enemyMovement.TriggerOscillateContainerUpAndDown(false, 0.0f, 0.0f);
                
                enemy.enemyMovement.GoBackToSpawn();
            }
                break;
            
            case EnemyStates.MovingToIdle:
            {
                enemy.enemyMovement.StopPathMovement();
                enemy.enemyMovement.TriggerOscillateContainerUpAndDown(false, 0.0f, 0.0f);
                
                bool isGroundBelow = enemy.enemyCollisionDetection.DetectGroundBelow(out float distToGround);

                if (isGroundBelow)
                {
                    Vector3 curPos = transform.position;
                    Vector3 targetPos = curPos;
                    float distToMove = 0.0f;

                    if (distToGround < Constants.EnemyData.IdleMinDistFromGround)
                    {
                        float delta = Constants.EnemyData.IdleMinDistFromGround - distToGround;
                        targetPos.y += delta;
                        
                        distToMove = delta;
                    }
                    else if (distToGround >= Constants.EnemyData.IdleMinDistFromGround &&
                             distToGround < Constants.EnemyData.IdleMaxDistFromGround)
                    {
                        float delta = distToGround - Constants.EnemyData.IdleMinDistFromGround;
                        targetPos.y -= delta;
                        
                        distToMove = delta;
                    }

                    if (distToMove != 0.0f)
                    {
                        stateDuration = distToMove / Constants.EnemyData.EnemyIdleMoveSpeed;
                    }
                    else
                    {
                        stateDuration = 0.1f;
                    }
                    
                    enemy.enemyMovement.SetMovingToIdleData(curPos, targetPos);
                }
                else
                {
                    stateDuration = 0.1f;
                }
            }
                break;
            
            case EnemyStates.Idle:
            {
                enemy.enemyMovement.StopPathMovement();
                enemy.enemyMovement.TriggerOscillateContainerUpAndDown(true, 0.65f, 0.25f);
                stateDuration = Random.Range(Constants.EnemyData.IdleDuration.x, Constants.EnemyData.IdleDuration.y);
            }
                break;
            
            case EnemyStates.Patrolling:
            {
                bool isWallOnRight = enemy.enemyCollisionDetection.DetectWallOnRight(out float distToRightWall);
                bool isWallOnLeft = enemy.enemyCollisionDetection.DetectWallOnLeft(out float distToLeftWall);

                float enemyLength = enemy.enemyMovement.EnemyLength;
                
                Vector3 curPos = transform.position;
                Vector3 rightPos = curPos;
                Vector3 leftPos = curPos;

                if (isWallOnRight)
                {
                    distToRightWall = Mathf.Min(distToRightWall - enemyLength * 0.5f, Constants.EnemyData.MaxPetrolHorizontalDist);
                    rightPos.x += distToRightWall;
                }
                else
                {
                    rightPos.x += Constants.EnemyData.MaxPetrolHorizontalDist;
                }
                
                if (isWallOnLeft)
                {
                    distToLeftWall = Mathf.Min(distToLeftWall - enemyLength * 0.5f, Constants.EnemyData.MaxPetrolHorizontalDist);
                    leftPos.x -= distToLeftWall;
                }
                else
                {
                    leftPos.x -= Constants.EnemyData.MaxPetrolHorizontalDist;
                }
                
                enemy.enemyMovement.StopPathMovement();
                enemy.enemyMovement.TriggerOscillateContainerUpAndDown(true, 1.0f, 0.35f);
                enemy.enemyMovement.SetPatrollingData(curPos, leftPos, rightPos);
            }
                break;
            
            case EnemyStates.ChasingPlayer:
            {
                enemy.enemyMovement.TriggerOscillateContainerUpAndDown(false, 0.0f, 0.0f);
                enemy.enemyMovement.StartChasingPlayer();
            }
                break;
            
            case EnemyStates.ChaseComplete:
            {
                enemy.enemyMovement.TriggerOscillateContainerUpAndDown(true, 0.65f, 0.25f);
            }
                break;
        }
    }

    private void UpdateStateTimer()
    {
        if (enemyState == EnemyStates.MovingToIdle)
        {
            stateTimeElapsed += Time.deltaTime;

            if (stateTimeElapsed < stateDuration)
            {
                float fac = stateTimeElapsed / stateDuration;
                float delta = movingToIdleCurve.Evaluate(fac);
                enemy.enemyMovement.UpdateMovingToIdleMovement(delta);
            }
            else
            {
                enemy.enemyMovement.UpdateMovingToIdleMovement(1.0f);
                SetEnemyState(EnemyStates.Idle);
            }
        }
        else if (enemyState == EnemyStates.Idle)
        {
            stateTimeElapsed += Time.deltaTime;

            if (stateTimeElapsed >= stateDuration)
            {
                SetEnemyState(EnemyStates.Patrolling);
            }
        }
        else if(enemyState == EnemyStates.Patrolling)
        {
            enemy.enemyMovement.UpdatePatrollingMovement();
        }
        else if(enemyState == EnemyStates.ChaseComplete)
        {
            enemy.enemyMovement.RotateTowardsTarget();
        }
    }

    #endregion

    #region Path Movement Callbacks

    public void PathMovementCompleted()
    {
        if (enemyState == EnemyStates.GoingBackToSpawn)
        {
            UpdateState(EnemyStates.MovingToIdle);
        }
        else if (enemyState == EnemyStates.ChasingPlayer)
        {
            UpdateState(EnemyStates.ChaseComplete);
        }
    }

    #endregion
    
    #region Getter / Setter

    public EnemyStates GetCurrentState()
    {
        return enemyState;
    }
    
    public void UpdateState(EnemyStates newState)
    {
        SetEnemyState(newState);
    }

    #endregion
}
