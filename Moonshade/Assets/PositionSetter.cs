using StarterAssets;
using UnityEngine;

public class PositionSetter : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private Camera cam;
    void Start()
    {
        spawnPos = new Vector3(spawnPos.x + Random.Range(-0.5f, 0.5f), spawnPos.y,
            spawnPos.z + Random.Range(-0.5f, 0.5f));
        PlayerController.LocalInstance.SetPosition(spawnPos);
        cam.gameObject.SetActive(false);
    }
}
