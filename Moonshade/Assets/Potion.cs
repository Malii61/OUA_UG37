using Photon.Pun;
using UnityEngine;

public class Potion : Item, I_Interactable
{
    private bool isPicked;
    private PhotonView PV;
    private bool checkBool;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void Use()
    {
        Debug.Log("used");
    }

    public void Interact(bool isPlayer = true)
    {
        if (isPicked || ItemManager.LocalInstance.DoesThePlayerHaveThisItem(this))
            return;
        ItemManager.LocalInstance.AddItem(this);
        InteractionText.Instance.SetText("You got the " + itemName);
        PV.RPC(nameof(OnPickedPunRpc), RpcTarget.All);
    }

    [PunRPC]
    private void OnPickedPunRpc()
    {
        isPicked = true;
        Debug.Log("dontdestroy this");
        DontDestroyOnLoad(gameObject);
    }

    public void OnFaced()
    {
        if (isPicked || ItemManager.LocalInstance.DoesThePlayerHaveThisItem(this))
            return;
        InteractionText.Instance.SetText(itemName + "\n[E]");
    }

    public void OnInteractEnded()
    {
        InteractionText.Instance.DisableText();
    }
}