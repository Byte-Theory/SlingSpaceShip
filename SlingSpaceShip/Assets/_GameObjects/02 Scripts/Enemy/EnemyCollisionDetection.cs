using System;
using UnityEngine;

public class EnemyCollisionDetection : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    
    // Enemy
    private Enemy enemy;
    
    // Player
    private Transform playerT;

    #region Unity Functions
    
    private void Update()
    {
        DetectPlayer();
    }

    #endregion
    
    #region SetUp

    public void SetUp(Enemy enemy, Transform playerT)
    {
        this.enemy = enemy;
        this.playerT = playerT;
    }

    #endregion

    #region Detection Player

    private void DetectPlayer()
    {
        EnemyStates enemyState = enemy.enemyStateManager.GetCurrentState();
        float dist = Vector3.Distance(transform.position, playerT.position);
        
        if (dist < Constants.EnemyData.PlayerDetectionRange)
        {
            Vector3 dir = playerT.position - transform.position;
            dir.Normalize();
            
            Physics.Raycast(transform.position, dir, out RaycastHit hit, 1000, playerLayer);

            if (hit.collider != null)
            {
                PlayerMovement playerMovement = hit.collider.GetComponent<PlayerMovement>();

                if (playerMovement != null)
                {
                    enemy.enemyStateManager.UpdateState(EnemyStates.ChasingPlayer);
                }
            }
        }
        else if (enemyState >= EnemyStates.ChasingPlayer)
        {
            if (dist > Constants.EnemyData.PlayerFollowRange)
            {
                enemy.enemyStateManager.UpdateState(EnemyStates.GoingBackToSpawn);
            }
        }
    }

    #endregion
    
    #region Ray Cast

    public bool DetectGroundBelow(out float hitDist)
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1000, groundLayer);

        if (hit.collider == null)
        {
            hitDist = -1;
            return false;
        }
        else
        {
            hitDist = hit.distance;
            return true;
        }
    }

    public bool DetectWallOnRight(out float hitDist)
    {
        Physics.Raycast(transform.position, Vector3.right, out RaycastHit hit, 1000, groundLayer);

        if (hit.collider == null)
        {
            hitDist = -1;
            return false;
        }
        else
        {
            hitDist = hit.distance;
            return true;
        }
    }

    public bool DetectWallOnLeft(out float hitDist)
    {
        Physics.Raycast(transform.position, Vector3.left, out RaycastHit hit, 1000, groundLayer);

        if (hit.collider == null)
        {
            hitDist = -1;
            return false;
        }
        else
        {
            hitDist = hit.distance;
            return true;
        }
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Constants.EnemyData.PlayerShootingRange);
        
        Gizmos.color = Color.brown;
        Gizmos.DrawWireSphere(transform.position, Constants.EnemyData.PlayerDetectionRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Constants.EnemyData.PlayerFollowRange);
    }

    #endregion
}
