using UnityEngine;
using Photon.Pun;
public class PlayerVisualUI : MonoBehaviourPunCallbacks
{
    public static PlayerVisualUI Instance { get; private set; }
    private PhotonView PV;
    [SerializeField] private MeshRenderer faceRenderer;
    private Texture2D currentTexture;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        faceRenderer.enabled = false;
        if (PV.IsMine)
            Instance = this;
    }
    public void SetFaceTexture(byte[] textureData)
    {
        PV.RPC(nameof(SetFaceTexturePunRpc), RpcTarget.AllBuffered, textureData);
    }
    [PunRPC]
    private void SetFaceTexturePunRpc(byte[] textureData)
    {
        faceRenderer.enabled = true;
        // Texture2D oluþturma ve verileri yükleme
        currentTexture = new Texture2D(2, 2);
        currentTexture.LoadImage(textureData);

        // Karakterin yüzüne texture'ý uygulama
        Material characterMaterial = faceRenderer.material;
        characterMaterial.mainTexture = currentTexture;
    }
}
