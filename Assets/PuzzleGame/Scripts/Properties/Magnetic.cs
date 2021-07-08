using Assets.PuzzleGame.Support;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnetic : MonoBehaviour, IMagnetic, IPunObservable
{
    public bool canStandOn = false;
    public bool resetVelocity = false;
    public MoveConstraints moveConstraints = MoveConstraints.None;

    private Rigidbody rb;
    private bool isActiveMagnetItem = false;
    private GameObject magnet;
    private double cooldownTimestamp;
    private bool originalUseGravity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isActiveMagnetItem && !magnet)
        {
            RemoveMagneticState();
        }
    }

    // --- Magnet Functionality

    public void SetMagneticState(GameObject magnet)
    {
        isActiveMagnetItem = true;
        originalUseGravity = rb.useGravity;
        rb.useGravity = false;
        this.magnet = magnet;
    }

    public void RemoveMagneticState()
    {
        isActiveMagnetItem = false;
        magnet = null;
        rb.useGravity = originalUseGravity;
        if (resetVelocity)
        {
            rb.velocity = Vector3.zero;
        }
    }

    public bool GetMagneticState()
    {
        return isActiveMagnetItem;
    }

    public bool GetEnabledState()
    {
        return true;
    }

    public bool CanStandOn => canStandOn;

    public MoveConstraints MoveConstraints => moveConstraints;

    [PunRPC]
    public void Cooldown(double secs)
    {
        cooldownTimestamp = Time.timeAsDouble + secs;
    }

    public double GetCooldown()
    {
        return Math.Max(0.0, cooldownTimestamp - Time.timeAsDouble);
    }

    // --- Freeze Functionality

    // --- Multiplayer

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
