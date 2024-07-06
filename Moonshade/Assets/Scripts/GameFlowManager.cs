using UnityEngine;
using TMPro;
using Photon.Pun;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI gameFlowInfoTxt;
    private PhotonView PV;
    private void Awake()
    {
        Instance = this;
        PV = GetComponent<PhotonView>();
    }
    public void AddInfo(string info)
    {
        PV.RPC(nameof(AddInfoPunRpc), RpcTarget.All, info);
    }
    [PunRPC]
    private void AddInfoPunRpc(string _info)
    {
        gameFlowInfoTxt.text += "\n" + _info;
    }
}
