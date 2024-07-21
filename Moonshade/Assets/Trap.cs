using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EnemyController enemyController))
        {
            enemyController.SlowEnemy(50f, 3f);
        }
    }
}