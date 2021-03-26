using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1";
    public GameObject connectPanel;
    public GameObject connectLabel;
    public GameObject statusLabel;
    public GameObject selectionPanel;
    public GameObject roomListPanel;
    public byte maxPlayersPerRoom;

    void Awake()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetActivePanel(connectPanel.name);
    }

    public void ButtonJoinRandomRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            SetActivePanel(connectLabel.name);

            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            SetActivePanel(connectLabel.name);
        }
    }

    public void ButtonCreateRoom()
    {
        SetActivePanel(connectLabel.name);
        string roomName = PhotonNetwork.NickName + "'s Room " + Random.Range(1000, 10000);
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public void ButtonRoomList()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        SetActivePanel(roomListPanel.name);
    }

    public void ButtonBack()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        SetActivePanel(selectionPanel.name);
    }

    public void Connect()
    {
        SetActivePanel(connectLabel.name);

        statusLabel.GetComponent<Text>().text = "Connecting...";
        statusLabel.GetComponent<Text>().color = new Color32(255, 230, 81, 255);
        // #Critical, we must first and foremost connect to Photon Online Server.
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
    }

    public void SetActivePanel(string panel)
    {
        connectPanel.SetActive(panel.Equals(connectPanel.name));
        selectionPanel.SetActive(panel.Equals(selectionPanel.name));
        connectLabel.SetActive(panel.Equals(connectLabel.name));
        roomListPanel.SetActive(panel.Equals(roomListPanel.name));
    }

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        SetActivePanel(selectionPanel.name);

        statusLabel.GetComponent<Text>().text = "Connected";
        statusLabel.GetComponent<Text>().color = new Color32(98, 255, 109, 255);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN: Now this client is in a room.");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel("MultiplayerLevel");
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN: No random room available, so we create one.");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        string roomName = PhotonNetwork.NickName + "'s Room " + Random.Range(1000, 10000);
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(selectionPanel.name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(selectionPanel.name);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetActivePanel(connectPanel.name);

        statusLabel.GetComponent<Text>().text = "Not Connected";
        statusLabel.GetComponent<Text>().color = new Color32(255, 136, 97, 255);
        Debug.LogWarningFormat("PUN: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    #endregion
}
