using Photon.Pun;
using UnityEngine;
using Cinemachine;
public class GhostController : MonoBehaviour
{
    private float ghostWalkSpeed = 7f;
    private float ghostRunSpeed = 10f;
    Vector3 moveAmount;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject cinemachine;
    [SerializeField] private Transform ghost;
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private Animator animator;
    private PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
            _renderer.enabled = false;
        else
        {
            Destroy(cinemachine);
            Destroy(cam.gameObject);
        }
    }
    void LateUpdate()
    {
        if (!PV.IsMine)
            return;
        Move();
    }

    private void Move()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 moveDir = new Vector3(movement.x, 0, movement.y);
        moveDir = cam.transform.forward * moveDir.z + cam.transform.right * moveDir.x;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveAmount = ghostRunSpeed * Time.deltaTime * moveDir;
        }
        else
        {
            moveAmount = ghostWalkSpeed * Time.deltaTime * moveDir;
        }
        transform.position += moveAmount;
        animator.SetFloat("Speed", moveAmount.magnitude, 0.05f, Time.deltaTime);
        Quaternion rotation = cam.transform.rotation;
        ghost.rotation = Quaternion.Lerp(ghost.rotation, rotation, Time.deltaTime * 4f);
        Vector3 pos = ghost.position;
        pos.y = Mathf.Clamp(ghost.position.y, -10, 30);
        ghost.position = pos;
    }
}
