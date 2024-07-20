using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemVisualUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private Image itemImage;
    public static ItemVisualUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateVisual(ItemManager.LocalInstance.GetCurrentItem());
    }

    public void UpdateVisual(Item item)
    {
        itemName.text = item.itemName;
        itemImage.sprite = item.itemSprite;
    }
}