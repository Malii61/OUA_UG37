using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MoonVisualRevealer : MonoBehaviour, I_Interactable
{
    [SerializeField] private Image img;
    private PhotonView PV;

    private void Awake()
    {
        img.enabled = false;
        PV = GetComponent<PhotonView>();
    }

    public void Interact(bool isPlayer = true)
    {
    }

    public void OnFaced()
    {
        if (ItemManager.LocalInstance.GetCurrentItem().itemId == ItemId.Flashlight)
            PV.RPC(nameof(SetTargetAlphaPunRpc), RpcTarget.All, 1);
    }

    public void OnInteractEnded()
    {
        PV.RPC(nameof(SetTargetAlphaPunRpc), RpcTarget.All, 0);
    }

    [PunRPC]
    private void SetTargetAlphaPunRpc(int _targetAlpha)
    {
        img.enabled = _targetAlpha == 1;
    }
}