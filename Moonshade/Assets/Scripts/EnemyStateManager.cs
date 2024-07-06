using Photon.Pun;
using UnityEngine;

// this class is for master client only
public class EnemyStateManager : MonoBehaviour
{
    public static EnemyStateManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    // public void OnDoorOpened(KeyType key)
    // {
    //     switch (key)
    //     {
    //         case KeyType.HouseKey:
    //             EnemySpawner.Instance.SpawnEnemy(EnemyType.HorrorMan);
    //             break;
    //         case KeyType.MainGateKey:
    //             break;
    //     }
    // }
}
