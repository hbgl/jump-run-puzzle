using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
///     The local GameManager class of every player.
///     Contains the logic to setup the local player instance and controls some local and network functions.
/// </summary>
/// <remarks>
///     <para>This class has a static <c>Instance</c> Property to access it from every script.</para>
///     <para>It also keeps the state of already collected powerups in the public HashSet <c>collectedItems</c>.</para>
/// </remarks>

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [HideInInspector]
    public HashSet<PowerupType> collectedItems = new HashSet<PowerupType>();
    public GUISystem gui;                   // [Assign] Reference to the local Skill GUI of the current scene
    public GameObject playerPrefab;         //          The player prefab to instantiate
    public Vector3 spawnPoint;
    public GameObject sceneSwitch;
    private Animator sceneAnimator;

    private GameObject player;              //          The networked local player instance  
    public static GameManager Instance;     //          Instance of the GameManager accessible in any script

    private const byte ChangeLevel = 1;
    private const byte ExitLevel = 2;

    void Awake()
    {
        // Initialize static instance, that can be accessed from any script
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        sceneAnimator = sceneSwitch.GetComponent<Animator>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // Instantiate local player object
        if(PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.JoinRoom("Debug");
            Debug.LogWarningFormat("<size=25><color=#42aaf5>Photon is now in Offline Mode</color></size>");
        }
        player = PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPoint, Quaternion.identity, 0);
        // Initialize Skill GUI of the player
        gui.Init(player.GetComponent<CharacterMovement>());
        gui.AddItem(PowerupType.Hand);
    }

    private void Update()
    {
        // Toggle between local Mouse Lock State
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DebugAllSkills();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
            PhotonNetwork.RaiseEvent(ExitLevel, null, raiseEventOptions, SendOptions.SendReliable);
        }

    }

    public void TriggerLevelEnd()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(ChangeLevel, null, raiseEventOptions, SendOptions.SendReliable);
    }

    void LoadNextLevel(string message)
    {
        sceneSwitch.GetComponentInChildren<Text>().text = message;
        sceneAnimator.Play("SceneSwitch_End");
        Invoke(nameof(SwitchScene), sceneAnimator.GetCurrentAnimatorStateInfo(0).length + 5);
    }

    private void SwitchScene()
    {
        Cursor.lockState = CursorLockMode.None;
        PhotonNetwork.LeaveRoom();
    }

    private void DebugAllSkills()
    {
        gui.AddItem(PowerupType.Hand);
        gui.AddItem(PowerupType.Glider);
        gui.AddItem(PowerupType.Icewand);
        gui.AddItem(PowerupType.Magnet);
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

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == ChangeLevel)
        {
            LoadNextLevel("Level cleared");
        }
        else if (eventCode == ExitLevel)
        {
            LoadNextLevel("Level failed");
        }
    }

    private new void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private new void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
