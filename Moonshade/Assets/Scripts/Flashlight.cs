using System;
using Photon.Pun;
using UnityEngine;

public class Flashlight : Item, I_Interactable
{
    private bool isPicked;
    [SerializeField] private Transform spotLight;
     [SerializeField] private FlashlightTrigger flashlightTrigger;
    private PhotonView PV;
    private bool isMine;
    private bool checkBool;

    private void Awake()
    {
        spotLight.gameObject.SetActive(false);
        flashlightTrigger.SetColliderState(false);
        PV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!isMine) return;
        
        if (checkBool && ItemManager.LocalInstance.GetCurrentItem() == this)
        {
            flashlightTrigger.SetColliderState(true);
            checkBool = false;
        }
        else if(!checkBool && ItemManager.LocalInstance.GetCurrentItem() != this)
        {
            flashlightTrigger.SetColliderState(false);
            checkBool = true;
        }
    }

    public void Interact(bool isPlayer = true)
    {
        if (isPicked || ItemManager.LocalInstance.DoesThePlayerHaveThisItem(this))
            return;
        ItemManager.LocalInstance.AddItem(this);
        InteractionText.Instance.SetText("You got a " + itemName);
        flashlightTrigger.enabled = true;
        isMine = true;
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