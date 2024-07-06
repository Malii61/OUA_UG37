using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public string itemName;
    public GameObject itemGameObject;
    public Vector3 localPos = Vector3.zero;
    public Vector3 rotOffset = Vector3.zero;
    public abstract void Use();
}
