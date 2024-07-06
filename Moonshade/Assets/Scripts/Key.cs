using Photon.Pun;
using UnityEngine;
public enum KeyType
{
    None,
    HouseKey,
    BasemantKey,
    MainGateKey,
}
public class Key : Item, I_Interactable
{
    [SerializeField] private KeyType keyType;
    private bool isPicked;
    private PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    public void Interact(bool isPlayer = true)
    {
        if (isPicked || !isPlayer)
            return;
        ItemManager.LocalInstance.AddItem(this);
        GameFlowManager.Instance.AddInfo(PhotonNetwork.LocalPlayer.NickName + " got the " + itemName);
        InteractionText.Instance.SetText("You got the " + itemName);
        PV.RPC(nameof(SetPickedPunRpc), RpcTarget.All, true);
    }

    [PunRPC]
    private void SetPickedPunRpc(bool _isPicked)
    {
        isPicked = _isPicked;
    }

    public void OnFaced()
    {
        if (isPicked)
            return;
        InteractionText.Instance.SetText(itemName + "\n[E]");
    }

    public void OnInteractEnded()
    {
        InteractionText.Instance.DisableText();
    }
    public KeyType GetKeyType()
    {
        return keyType;
    }
    public override void Use()
    {
    }
}
