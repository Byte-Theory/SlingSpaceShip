using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Player")] 
    [SerializeField] private Transform playerT;

    [Header("Spawned Enemies")] 
    [SerializeField] private List<Enemy> allEnemies;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUp();
    }

    #region SetUp

    private void SetUp()
    {
        for (int idx = 0; idx < allEnemies.Count; idx++)
        {
            Enemy enemy = allEnemies[idx];
            enemy.SetUp(playerT);
        }
    }

    #endregion
}
