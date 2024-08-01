using StarterAssets;
using UnityEngine;

public class PositionSetter : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform[] extraTransformsToDeactivate;
    void Start()
    {
        spawnPos = new Vector3(spawnPos.x + Random.Range(-0.5f, 0.5f), spawnPos.y,
            spawnPos.z + Random.Range(-0.5f, 0.5f));
        PlayerController.LocalInstance.SetPosition(spawnPos);
        cam.gameObject.SetActive(false);
        Cursor.visible = false;

        foreach (var t in extraTransformsToDeactivate)
        {
            t.gameObject.SetActive(false);
        }
    }
}