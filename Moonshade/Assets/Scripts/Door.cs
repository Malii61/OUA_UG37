using Photon.Pun;
using UnityEngine;

public class Door : MonoBehaviour, I_Interactable
{
    [SerializeField] private KeyType requiredKey;
    [SerializeField] private string doorName;
    [SerializeField] private bool openOnlyOnce;
    [SerializeField] private bool unlocked = false;
    [SerializeField] Animator leftDoorAnim;
    [SerializeField] Animator rightDoorAnim;
    private bool isOpened = false;
    private PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    public void Interact(bool isPlayer = true)
    {
        if (HasKey(isPlayer) || unlocked)
        {
            isOpened = !isOpened;
            if (!unlocked)
                GameFlowManager.Instance.AddInfo(PhotonNetwork.LocalPlayer.NickName + " has unlocked the " + doorName);
            PV.RPC(nameof(OpenDoorPunRpc), RpcTarget.All, isOpened);
        }
        else if (isPlayer)
            InteractionText.Instance.SetText("Locked!");
    }
    [PunRPC]
    private void OpenDoorPunRpc(bool _isOpened)
    {
        unlocked = true;
        isOpened = _isOpened;
        if (openOnlyOnce) Destroy(GetComponent<BoxCollider>());

        if (leftDoorAnim) leftDoorAnim.SetBool("isLeftDoorOpen", _isOpened);

        if (rightDoorAnim) rightDoorAnim.SetBool("isRightDoorOpen", _isOpened);
    }

    private bool HasKey(bool isPlayer)
    {
        if (!isPlayer)
            return false;
        if (ItemManager.LocalInstance.GetCurrentItem().TryGetComponent(out Key key))
        {
            if (key.GetKeyType() == requiredKey)
            {
                ItemManager.LocalInstance.RemoveItem(key);
                PhotonNetwork.Destroy(key.gameObject);
                return true;
            }
        }
        return false;
    }

    public void OnFaced()
    {
        InteractionText.Instance.SetText(doorName + " \n [E]");
    }

    public void OnInteractEnded()
    {
        InteractionText.Instance.DisableText();
    }
    public bool IsOpen()
    {
        return isOpened;
    }
    public bool isOpenable()
    {
        return !isOpened && unlocked;
    }
}
