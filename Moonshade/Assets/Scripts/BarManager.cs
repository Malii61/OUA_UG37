using UnityEngine;

public class BarManager : MonoBehaviour
{
    public static BarManager Instance { get; private set; }
    // [SerializeField] private ProgressBarPro healthBar;
    // [SerializeField] private ProgressBarPro energyBar;
    public Gradient healthGradient;
    public Gradient energyGradient;
    public enum BarType
    {
        health,
        energy,
    }

    private void Awake()
    {
        Instance = this;
    }

    public void SetValue(float currentValue, float maxValue, BarType type, Color color = new Color())
    {
        // ProgressBarPro bar = SetBar(type);
        //
        // //value
        // bar.SetValue(currentValue, maxValue);
        //
        // //color
        // float percentage = currentValue / maxValue;
        // ImageSlicedMirror fill = bar.GetComponentInChildren<ImageSlicedMirror>();
        // Gradient gradient = type == BarType.health ? healthGradient : energyGradient;
        // if (fill != null)
        // {
        //     if (color.a == 0 && color.b == 0)
        //     {
        //         fill.color = gradient.Evaluate(percentage);
        //     }
        //     else
        //         fill.color = color;
        // }
    }

    // private ProgressBarPro SetBar(BarType type)
    // {
    //     return type switch
    //     {
    //         BarType.energy => energyBar,
    //         _ => healthBar,
    //     };
    // }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
