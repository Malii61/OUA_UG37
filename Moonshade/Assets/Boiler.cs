using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Boiler : MonoBehaviour, I_Interactable
{
    public void OnFaced()
    {
        if (ItemManager.LocalInstance.GetCurrentItem().itemId == ItemId.Potion)
        {
            InteractionText.Instance.SetText("Throw potion");
        }
    }

    public void Interact(bool isPlayer = true)
    {
        if (ItemManager.LocalInstance.GetCurrentItem().itemId == ItemId.Potion)
        {
            Item item = ItemManager.LocalInstance.GetCurrentItem();
            item.Use();
            ItemManager.LocalInstance.RemoveItem(item);
            PhotonNetwork.Destroy(item.gameObject);
            InteractionText.Instance.SetText("You throwed the potion!");
            Invoke(nameof(DisableText), 1.5f);
        }
    }

    private void DisableText()
    {
        InteractionText.Instance.DisableText();
    }

    public void OnInteractEnded()
    {
        InteractionText.Instance.DisableText();
    }
}