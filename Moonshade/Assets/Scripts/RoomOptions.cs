using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RoomOptions : MonoBehaviour
{
    public const int MAX_PLAYER_COUNT = 10;
    public static bool isRoomPublic = true;
    [SerializeField] private TMP_InputField roomNameIF;
    [SerializeField] Button publicButton;
    [SerializeField] Button privateButton;
    // Start is called before the first frame update
    private void Awake()
    {
        publicButton.onClick.AddListener(() => OnClick_RoomTypeButton(privateButton));
        privateButton.onClick.AddListener(() => OnClick_RoomTypeButton(publicButton));
    }

    private void OnEnable()
    {
        roomNameIF.text = PhotonNetwork.LocalPlayer.NickName + "'s room";
    }

    void Start()
    {
        publicButton.image.color = ColorBlock.defaultColorBlock.selectedColor;
        privateButton.image.color = ColorBlock.defaultColorBlock.disabledColor;
    }
    public void OnClick_RoomTypeButton(Button otherButton)
    {
        otherButton.image.color = ColorBlock.defaultColorBlock.disabledColor;
        if (otherButton.name == "PrivateBtn")
        {
            publicButton.image.color = ColorBlock.defaultColorBlock.selectedColor;
            isRoomPublic = true;
        }
        else
        {
            privateButton.image.color = ColorBlock.defaultColorBlock.selectedColor;
            isRoomPublic = false;
        }

    }
}
