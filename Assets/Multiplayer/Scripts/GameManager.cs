using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject playerPrefab;

    private void Start()
    {
        if (playerPrefab != null)
        {
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
        }
    }

    #region Photon Callbacks

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Launcher");
    }

    //public override void OnPlayerEnteredRoom(Player other)
    //{
    //    Debug.LogFormat("Player entered: {0}", other.NickName); // not seen if you're the player connecting

    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        LoadArena();
    //    }
    //}

    //public override void OnPlayerLeftRoom(Player other)
    //{
    //    Debug.LogFormat("Player left: {0}", other.NickName); // seen when other disconnects


    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        LoadArena();
    //    }
    //}

    #endregion

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    //private void LoadArena()
    //{
    //    Debug.Log("PhotonNetwork : Loading Level");
    //    PhotonNetwork.LoadLevel("MultiplayerLevel");
    //}
}
