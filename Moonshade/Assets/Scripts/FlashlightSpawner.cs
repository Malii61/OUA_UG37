using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightSpawner : MonoBehaviour
{
    [SerializeField] List<Transform> flashlightSpawnPoints = new List<Transform>();

    private void Start()
    {
        Invoke(nameof(Spawn), 1f);
    }

    private void Spawn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Transform flashlightSpawnPoint in flashlightSpawnPoints)
            {
                PhotonNetwork.Instantiate("PhotonPrefabs/Flashlight", flashlightSpawnPoint.position,
                    flashlightSpawnPoint.rotation);
            }
        }
    }
}