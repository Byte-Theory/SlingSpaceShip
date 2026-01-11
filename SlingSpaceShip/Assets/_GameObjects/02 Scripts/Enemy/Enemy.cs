using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStateManager enemyStateManager { get; private set; }
    public EnemyCollisionDetection enemyCollisionDetection { get; private set; }
    public EnemyMovement enemyMovement { get; private set; }

    #region SetUp

    public void SetUp(Transform playerT)
    {
        enemyStateManager = GetComponent<EnemyStateManager>();
        enemyCollisionDetection = GetComponent<EnemyCollisionDetection>();
        enemyMovement = GetComponent<EnemyMovement>();
        
        enemyStateManager.SetUp(this);
        enemyCollisionDetection.SetUp(this, playerT);
        enemyMovement.SetUp(this, playerT);
    }

    #endregion
}
