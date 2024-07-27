using System;
using UnityEngine;

public class FlashlightTrigger : MonoBehaviour
{
    private MoonVisualRevealer lastVisualFaced;

    public void SetColliderState(bool isActive)
    {
        GetComponent<Collider>().enabled = isActive;
        if (isActive is false && lastVisualFaced != null)
        {
            lastVisualFaced.OnInteractEnded();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out MoonVisualRevealer moonVisualRevealer))
        {
            lastVisualFaced = moonVisualRevealer;
            moonVisualRevealer.OnFaced();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out MoonVisualRevealer moonVisualRevealer))
        {
            moonVisualRevealer.OnInteractEnded();
        }
    }
}