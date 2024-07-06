using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private const string READY_PLAYER_COUNT = "readyPlayerCount";
    [SerializeField] private Button readyButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button leaveRoomButton;
    private int readyPlayerCount;
    private bool isReady;
    private void Awake()
    {
        readyButton.onClick.AddListener(() => SetPlayerReady());
        startGameButton.onClick.AddListener(() => StartGame());
        leaveRoomButton.onClick.AddListener(() => LeaveRoom());
        startGameButton.gameObject.SetActive(false);
    }
    private void SetPlayerReady()
    {
        if (!isReady)
        {
            int readyPlayerCount;
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(READY_PLAYER_COUNT))
            {
                readyPlayerCount = (int)PhotonNetwork.CurrentRoom.CustomProperties[READY_PLAYER_COUNT];
                Debug.Log(readyPlayerCount);
            }
            else
                readyPlayerCount = 0;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { READY_PLAYER_COUNT, readyPlayerCount + 1 } });
            isReady = true;
            PlayerUI.Instance.SetReadyText();
        }
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(READY_PLAYER_COUNT))
        {
            readyPlayerCount = (int)PhotonNetwork.CurrentRoom.CustomProperties[READY_PLAYER_COUNT];
            if (PhotonNetwork.LocalPlayer.IsMasterClient && readyPlayerCount == PhotonNetwork.CurrentRoom.PlayerCount)
                startGameButton.gameObject.SetActive(true);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameButton.gameObject.SetActive(false);
        }
    }
    private void LeaveRoom()
    {
        if (isReady)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { READY_PLAYER_COUNT, readyPlayerCount - 1 } });
        }
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    public void StartGame()
    {
        Loader.LoadNetwork(Loader.Scene.GameScene);
    }

}
