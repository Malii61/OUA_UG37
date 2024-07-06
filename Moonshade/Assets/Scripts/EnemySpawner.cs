using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum EnemyType
{
    SchoolGirl,
    BunnyGirl,
}
[Serializable]
public class EnemyInfo
{
    public EnemyType type;
    public Transform transform;
}
public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }
    [SerializeField] private List<EnemyInfo> enemies = new List<EnemyInfo>();
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnEnemy(EnemyType.SchoolGirl);
        }
    }
    public void SpawnEnemy(EnemyType enemyType)
    {
        EnemyInfo enemyInfo = FindEnemyByType(enemyType);
        PhotonNetwork.Instantiate("PhotonPrefabs/Enemies/" + enemyInfo.type.ToString(), enemyInfo.transform.position, enemyInfo.transform.rotation);
    }
    private EnemyInfo FindEnemyByType(EnemyType type)
    {
        foreach(EnemyInfo enemy in enemies)
        {
            if (enemy.type == type)
                return enemy;
        }
        return default;
    }
}
