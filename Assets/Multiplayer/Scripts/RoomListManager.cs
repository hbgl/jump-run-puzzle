using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

/**
 * Class that manages the content of the Room List
 */
public class RoomListManager : MonoBehaviourPunCallbacks
{
    public GameObject roomListContent; // Attach Content Object
    public GameObject roomEntryPrefab; // Attach Entry Prefab

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();      // Contains Info about each Room
    private Dictionary<string, GameObject> roomListEntries = new Dictionary<string, GameObject>(); // Contains all RoomEntry GameObjects

    #region Methods

    // Remove all Entry-Objects and clear list
    private void ClearRoomListView()
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        roomListEntries.Clear();
    }

    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(roomEntryPrefab);
            entry.transform.SetParent(roomListContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<RoomEntry>().Initialize(info.Name, (byte) info.PlayerCount, info.MaxPlayers);
            roomListEntries.Add(info.Name, entry);
        }
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }
    #endregion

    #region PUN Callbacks

    public override void OnJoinedLobby()
    {
        // Clear any previous room lists
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    #endregion

}
