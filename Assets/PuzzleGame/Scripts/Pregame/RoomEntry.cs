using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/**
 * Class that initializes the content of a Room Entry in the List
 * Attach Script to RoomEntry Prefab
 */
public class RoomEntry : MonoBehaviour
{
    public Text roomNameText;
    public Text playersCountText;
    public Button joinButton;

    private string roomName;

    public void Initialize(string name, byte currentPlayers, byte maxPlayers)
    {
        roomName = name;
        roomNameText.text = name;
        playersCountText.text = currentPlayers + " / " + maxPlayers;
        if (currentPlayers >= maxPlayers)
        {
            joinButton.interactable = false;
        }
    }

    public void ButtonJoin()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(roomName);
    }
}
