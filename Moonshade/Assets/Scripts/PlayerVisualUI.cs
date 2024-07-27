using UnityEngine;
using Photon.Pun;
public class PlayerVisualUI : MonoBehaviourPunCallbacks
{
    public static PlayerVisualUI Instance { get; private set; }
    private PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
            Instance = this;
    }
}
