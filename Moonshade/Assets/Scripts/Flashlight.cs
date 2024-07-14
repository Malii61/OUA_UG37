using Photon.Pun;
using UnityEngine;

public class Flashlight : Item, I_Interactable
{
    private bool isPicked;
    [SerializeField] private Transform spotLight;
    private PhotonView PV;

    private void Awake()
    {
        spotLight.gameObject.SetActive(false);
        PV = GetComponent<PhotonView>();
    }

    public void Interact(bool isPlayer = true)
    {
        if (isPicked || ItemManager.LocalInstance.DoesThePlayerHaveThisItem(this))
            return;
        ItemManager.LocalInstance.AddItem(this);
        InteractionText.Instance.SetText("You got a " + itemName);
        PV.RPC(nameof(OnPickedPunRpc), RpcTarget.All);
    }

    [PunRPC]
    private void OnPickedPunRpc()
    {
        isPicked = true;
        spotLight.gameObject.SetActive(true);
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

    public override void Use()
    {
    }
}