using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Assets.PuzzleGame.Support;

public class Ball : MonoBehaviourPun, IMagnetic, IFreezable, IPunObservable
{
    private Rigidbody rb;
    private bool isActiveMagnetItem = false;
    private bool isFrozen = false;
    private GameObject magnet;
    private GameObject iceblock;
    private Vector3 windForce = Vector3.zero;
    private double CooldownTimestamp;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        SyncMultiplayerState();

        if (photonView.IsMine == false)
        {
            return;
        }

        if(isActiveMagnetItem && !magnet)
        {
            RemoveMagneticState();
        }
    }

    // --- Wind Functionality

    private void FixedUpdate()
    {
        if (windForce != Vector3.zero)
        {
            rb.AddForce(windForce, ForceMode.Force);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wind"))
        {
            windForce += other.transform.forward * other.GetComponent<Wind>().strength;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wind"))
        {
            windForce -= other.transform.forward * other.GetComponent<Wind>().strength;
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
        //photonView.TransferOwnership(PhotonNetwork.MasterClient);
    }

    public bool GetMagneticState()
    {
        return isActiveMagnetItem;
    }

    public bool GetEnabledState()
    {
        return true;
    }

    [PunRPC]
    public void Cooldown(double secs)
    {
        CooldownTimestamp = Time.timeAsDouble + secs;
    }

    public double GetCooldown()
    {
        return Math.Max(0.0, CooldownTimestamp - Time.timeAsDouble);
    }

    public bool CanStandOn => false;

    public MoveConstraints MoveConstraints => MoveConstraints.None;

    // --- Freeze Functionality

    public void SetFrozenState(GameObject iceblock)
    {
        if(isActiveMagnetItem)
        {
            RemoveMagneticState();
        }

        isFrozen = true;
        this.iceblock = iceblock;
        GetComponent<Rigidbody>().detectCollisions = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    [PunRPC]
    public void RemoveFrozenState()
    {
        isFrozen = false;
        iceblock = null;
        GetComponent<Rigidbody>().detectCollisions = true;
        if (GetComponent<PhotonView>().IsMine)
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
        //photonView.TransferOwnership(PhotonNetwork.MasterClient);
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
        }
        else
        {
            isActiveMagnetItem = (bool)stream.ReceiveNext();
        }
    }
}
