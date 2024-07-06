using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ItemManager : MonoBehaviourPunCallbacks
{
    public static ItemManager LocalInstance { get; private set; }

    private PhotonView PV;
    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] Transform aimPoint;
    int itemIndex;
    int previousItemIndex = -1;

    private void Awake()
    {
        PV = transform.root.GetComponent<PhotonView>();
        if (PV.IsMine)
            LocalInstance = this;
    }
    void Start()
    {
        if (PV.IsMine)
            EquipItem(0);
    }
    private void Update()
    {
        SelectItem();
        RotateItem();
    }

    private void RotateItem()
    {
        // eðer objeye çok yakýnsak rotate ederken obje yamuluyor. Bu sorunun çözümü için eðer objeye çok yakýnsak return diyoruz.
        float distance = Vector3.Distance(aimPoint.position, items[itemIndex].itemGameObject.transform.position);
        if (distance < 0.6f)
            return;
        Vector3 targetDirection = aimPoint.position - items[itemIndex].itemGameObject.transform.position;
        // objenin targeta doðru dönmesi
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        items[itemIndex].itemGameObject.transform.rotation = Quaternion.Slerp(items[itemIndex].itemGameObject.transform.rotation, targetRotation * Quaternion.Euler(items[itemIndex].rotOffset), 10 * Time.deltaTime);
    }

    public void AddItem(Item newItem)
    {
        Transform itemTransform = newItem.itemGameObject.transform;
        itemTransform.parent = transform;
        itemTransform.localPosition = newItem.localPos;
        items.Add(newItem);
        EquipItem(items.Count - 1);
        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("newItem", newItem.name);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    public void RemoveItem(Item item)
    {
        if (items[itemIndex] == item)
            EquipItem(--itemIndex);
        if (PV.IsMine)
        {
            item.itemGameObject.transform.parent = null;
            Hashtable hash = new Hashtable();
            hash.Add("deleteItem", item.name);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        items.Remove(item);
    }
    void SelectItem()
    {
        if (EventSystem.current.currentSelectedGameObject)
        {
            return;
        }

        var scrollValue = Mouse.current.scroll.ReadValue().normalized.y;
        if (itemIndex >= items.Count)
        {
            EquipItem(items.Count - 1);
        }
        else if (scrollValue > 0f)
        {
            if (itemIndex >= items.Count - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if (scrollValue < 0f)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Count - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }

    }
    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;
        if (!items[itemIndex].itemGameObject)
            items.Remove(items[itemIndex]);
        else
            items[itemIndex].itemGameObject?.SetActive(true);

        if (previousItemIndex != -1 && previousItemIndex < items.Count)
        {
            items[previousItemIndex].itemGameObject?.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    public Item GetCurrentItem()
    {
        if (itemIndex >= items.Count)
        {
            itemIndex = items.Count - 1;
        }
        return items[itemIndex];
    }
    public bool DoesThePlayerHaveThisItem(Item _item)
    {
        foreach(Item item in items)
        {
            if (item.itemName == _item.itemName)
                return true;
        }
        return false;

    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("newItem") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            foreach (Item item in FindObjectsOfType<Item>())
            {
                if (item.gameObject.name == (string)changedProps["newItem"])
                {
                    AddItem(item);
                }
            }
        }
        if (changedProps.ContainsKey("deleteItem") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            foreach (Item item in FindObjectsOfType<Item>())
            {
                if (item.gameObject.name == (string)changedProps["newItem"])
                {
                    RemoveItem(item);
                }
            }
        }
        if (changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            int index = (int)changedProps["itemIndex"];
            if (index < items.Count == index >= 0)
                EquipItem(index);
        }
    }
}
