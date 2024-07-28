using UnityEngine;
using Photon.Pun;

public class PotionSpawner : MonoBehaviour
{
    [SerializeField] private Transform redPotionSpawnPoint;
    [SerializeField] private Transform bluePotionSpawnPoint;

    private void Start()
    {
        Invoke(nameof(Spawn), 1f);
    }

    private void Spawn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("PhotonPrefabs/BluePotion", bluePotionSpawnPoint.position,
                bluePotionSpawnPoint.rotation);

            PhotonNetwork.Instantiate("PhotonPrefabs/RedPotion", redPotionSpawnPoint.position,
                redPotionSpawnPoint.rotation);
        }
    }
}