using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
	[SerializeField] TMP_Text roomName;
	public TMP_Text playerCount;
	public RoomInfo info;
	public void SetUp(RoomInfo _info)
	{
		info = _info;
		roomName.text = _info.Name;
	}
	public void OnClick()
	{
		PhotonLauncher.Instance.JoinRoom(info);
	}
}