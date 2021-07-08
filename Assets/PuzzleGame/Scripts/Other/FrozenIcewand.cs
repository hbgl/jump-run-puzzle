using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Assets.PuzzleGame.Support;

public class FrozenIcewand : MonoBehaviourPun, IMagnetic, IFreezable
{

    private Rigidbody rb;
    private bool isActiveMagnetItem;
    private bool isFrozen;
    private GameObject magnet;
    public GameObject iceblock;
    private double CooldownTimestamp;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().detectCollisions = false;
    }

    private void Update()
    {
        SyncMultiplayerState();

        if (photonView.IsMine == false)
        {
            return;
        }

        if (isActiveMagnetItem && !magnet)
        {
            RemoveMagneticState();
        }
    }

    // --- Magnet Functionality

    public void SetMagneticState(GameObject magnet)
    {
        isActiveMagnetItem = true;
        rb.useGravity = false;
        this.magnet = magnet;
    }

    public void RemoveMagneticState()
    {
        isActiveMagnetItem = false;
        magnet = null;
        rb.useGravity = true;
    }

    public bool GetMagneticState()
    {
        return isActiveMagnetItem;
    }

    public bool GetEnabledState()
    {
        return true;
    }

    public bool CanStandOn => false;

    public MoveConstraints MoveConstraints => MoveConstraints.None;

    [PunRPC]
    public void Cooldown(double secs)
    {
        CooldownTimestamp = Time.timeAsDouble + secs;
    }

    public double GetCooldown()
    {
        return Math.Max(0.0, CooldownTimestamp - Time.timeAsDouble);
    }

    // --- Freeze Functionality

    public void SetFrozenState(GameObject iceblock)
    {
        if (isActiveMagnetItem)
        {
            RemoveMagneticState();
        }

        isFrozen = true;
        this.iceblock = iceblock;
        GetComponent<Rigidbody>().detectCollisions = false;
        transform.localPosition = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    [PunRPC]
    public void RemoveFrozenState()
    {
        isFrozen = false;
        iceblock = null;
        transform.parent = null;
        GetComponent<Rigidbody>().detectCollisions = true;
        if (GetComponent<PhotonView>().IsMine)
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public bool GetFrozenState()
    {
        return isFrozen;
    }

    // --- Multiplayer

    public void SyncMultiplayerState()
    {
        if (isFrozen || (isActiveMagnetItem && !photonView.IsMine))
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isActiveMagnetItem);
            // stream.SendNext(CooldownTimestamp);
        }
        else
        {
            isActiveMagnetItem = (bool)stream.ReceiveNext();
            // CooldownTimestamp = (double)stream.ReceiveNext();
        }
    }
}
