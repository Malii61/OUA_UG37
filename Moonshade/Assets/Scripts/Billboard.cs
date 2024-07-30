using Photon.Pun;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera cam;

    void LateUpdate()
    {
        if (cam == null)
        {
            foreach (var cam in FindObjectsByType<Camera>(FindObjectsSortMode.None))
            {
                if (cam.enabled && cam.transform.root.GetComponent<PhotonView>().IsMine)
                {
                    this.cam = cam;
                }
            }
        }

        if (cam == null)
            return;

        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up * 180);
    }
}