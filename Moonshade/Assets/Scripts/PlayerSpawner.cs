using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance { get; private set; }
    private PhotonView PV;
    private Transform player;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    private void Awake()
    {
        Instance = this;

        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        Debug.Log("Start");
        // Yeni bir oyuncu sahneye katıldığında cagrilir


        if (PhotonNetwork.IsConnected)
        {
            // Oyuncunun spawn pozisyonunu belirleme
            int spawnIndex = GetNextSpawnIndex();

            // Oyuncuyu olu�turma
            StartCoroutine(CreatePlayer(spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation));
        }
    }

    private int GetNextSpawnIndex()
    {
        // Mevcut oyuncu listesini al
        Player[] players = PhotonNetwork.PlayerList;

        // Mevcut oyuncu say�s�na g�re spawn indeksini belirle
        int spawnIndex = players.Length % spawnPoints.Count;
        return spawnIndex - 1;
    }

    private IEnumerator CreatePlayer(Vector3 spawnpoint, Quaternion rotation)
    {
        yield return new WaitForSeconds(0.5f);
        player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnpoint, rotation, 0,
            new object[] { PV.ViewID }).transform;
    }

    public Transform GetPlayer()
    {
        return player;
    }
}