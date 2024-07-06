using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightSpawner : MonoBehaviour
{
    [SerializeField] List<Transform> flashlightSpawnPoints = new List<Transform>();
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach(Transform flashlightSpawnPoint in flashlightSpawnPoints)
            {
                PhotonNetwork.Instantiate("PhotonPrefabs/Flashlight", flashlightSpawnPoint.position, flashlightSpawnPoint.rotation);
            }
        }
    }
}
