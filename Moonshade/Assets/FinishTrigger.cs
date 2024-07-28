using System;
using System.Collections.Generic;
using System.Linq;
using StarterAssets;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class FinishTrigger : MonoBehaviourPunCallbacks
{
    private List<PlayerController> readyPlayers = new List<PlayerController>();
    [SerializeField] private Loader.Scene sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("other geldi");
        if (other.TryGetComponent(out PlayerController playerController))
        {
            if (playerController.GetComponent<PhotonView>().IsMine)
                InteractionText.Instance.SetText("Wait for other players..");

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (!readyPlayers.Contains(playerController))
                    readyPlayers.Add(playerController);

                if (readyPlayers.Count >= PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    GoOtherScene();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("other gitti");
        if (other.TryGetComponent(out PlayerController playerController))
        {
            if (playerController.GetComponent<PhotonView>().IsMine)
                InteractionText.Instance.DisableText();

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
                readyPlayers.Remove(playerController);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            var pl = readyPlayers.FirstOrDefault(pl => Equals(pl.GetComponent<PhotonView>().Owner, otherPlayer));
            if (pl != null)
            {
                readyPlayers.Remove(pl);
            }

            if (readyPlayers.Count >= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                GoOtherScene();
            }
        }
    }

    private void GoOtherScene()
    {
        if (sceneToLoad != Loader.Scene.GameScene)
        {
            Loader.LoadNetwork(sceneToLoad);
        }
        else
        {
            Debug.Log("Sahne atanmamış!!");
        }
    }
}