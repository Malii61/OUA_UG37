using UnityEngine;
using TMPro;
public class InteractionText : MonoBehaviour
{
    public static InteractionText Instance { get; private set; }
    private TextMeshProUGUI interactionText;
    private void Awake()
    {
        Instance = this;
        interactionText = GetComponent<TextMeshProUGUI>();
    }
    public void SetText(string txt)
    {
        interactionText.enabled = true;
        interactionText.text = txt;
    }
    public void DisableText()
    {
        interactionText.enabled = false;
    }
}
