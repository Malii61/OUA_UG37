using Photon.Pun;
using UnityEngine;

public class AimPointManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] Camera cam;
    PhotonView PV;
    private void Awake()
    {
        PV = transform.root.GetComponent<PhotonView>();
        GetComponent<MeshRenderer>().enabled = false;
    }
    private void Update()
    {
        if (!PV.IsMine)
            return;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = cam.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999, aimColliderLayerMask))
        {
            transform.position = raycastHit.point;
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
        }
    }
}
