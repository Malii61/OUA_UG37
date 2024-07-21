using StarterAssets;
using UnityEngine;

public class JumpableSurface : MonoBehaviour
{
    [SerializeField] private float force = 10f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("other geldi");
        if (other.TryGetComponent(out PlayerController playerController))
        {
            Debug.Log("player force uygulanıyor");
            playerController.JumpManually(force);
        }
    }
}