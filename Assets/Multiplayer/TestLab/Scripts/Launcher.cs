using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Assets.Multiplayer.TestLab
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        private string roomName = "TestLab";
        private byte maxPlayers = 4;
        private string gameVersion = "1";

        void Awake()
        {
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                PhotonNetwork.LoadLevel("TestScene");
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayers });
        }
    }
}
