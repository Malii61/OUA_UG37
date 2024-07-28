using UnityEngine;

public enum ItemId
{
    Idle,
    Flashlight,
    RedPotion,
    BluePotion,
}
public abstract class Item : MonoBehaviour
{
    public ItemId itemId;
    public Sprite itemSprite;
    public string itemName;
    public GameObject itemGameObject;
    public Vector3 localPos = Vector3.zero;
    public Vector3 rotOffset = Vector3.zero;
    public abstract void Use();
}
