using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum EnemyType
{
    Ghost,
    HorrorMan,
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
}
