using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class PlayerUI : MonoBehaviourPunCallbacks
{
    public static PlayerUI Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private TextMeshProUGUI readyText;
    [SerializeField] private bool isGhost;
    private PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (isGhost)
        {
            if (PV.IsMine)
                nicknameText.enabled = false;

            nicknameText.color = Color.red;
            readyText.enabled = false;
        }
        if (PV.IsMine)
            Instance = this;
    }
    private void Start()
    {
        nicknameText.text = PV.Owner.NickName;
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        readyText.enabled = false;
        if (PV.IsMine)
            nicknameText.enabled = false;
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    public void SetReadyText()
    {
        PV.RPC(nameof(SetReadyTextPunRPC), RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void SetReadyTextPunRPC()
    {
        readyText.text = "Ready";
    }
}
