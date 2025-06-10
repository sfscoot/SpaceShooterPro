using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyClass
{
    public string name;
    public int waveAvailable;
    public GameObject enemyPrefab;
    public EnemyType _enemyType;
    public EnemyMovementType _enemyMovementType;
}
