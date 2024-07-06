using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    public static PhotonLauncher Instance { get; private set; }
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    //[SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    //[SerializeField] GameObject startGameButton;

    private int maxPlayer;
    private PhotonView PV;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        Instance = this;
    }

    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        maxPlayer = RoomOptions.MAX_PLAYER_COUNT;
        Photon.Realtime.RoomOptions ropts = new Photon.Realtime.RoomOptions() { IsOpen = true, IsVisible = RoomOptions.isRoomPublic, MaxPlayers = (byte)maxPlayer };
        PhotonNetwork.CreateRoom(roomNameInputField.text, ropts);
        MenuManager.Instance.OpenMenu("loading");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("odadasýn " + PhotonNetwork.LocalPlayer.NickName);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //MenuManager.Instance.OpenMenu("room");
        //roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        //Player[] players = PhotonNetwork.PlayerList;

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //startGameButton.SetActive(PhotonNetwork.IsMasterClient);

    }
    public override void OnCreatedRoom()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("password", out object value))
            Debug.Log((string)value);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        Debug.LogError("Room Creation Failed: " + message);
        MenuManager.Instance.OpenMenu("error");
    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }
    public void JoinPrivateRoom(TMP_InputField roomName)
    {
        PhotonNetwork.JoinRoom(roomName.text);
        MenuManager.Instance.OpenMenu("loading");
    }
    public override void OnLeftRoom()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
            SceneManager.LoadScene("MenuScene");
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            RoomListItem roomItem = Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>();
            roomItem.SetUp(roomList[i]);
            roomItem.playerCount.text = roomList[i].PlayerCount + "/" + roomList[i].MaxPlayers;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorText.text = "Joined room Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }
}