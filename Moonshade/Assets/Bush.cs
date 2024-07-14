using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Bush : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PhotonView PV) && PV.IsMine)
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "isHided", true } });
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PhotonView PV) && PV.IsMine)
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "isHided", false } });
    }
}