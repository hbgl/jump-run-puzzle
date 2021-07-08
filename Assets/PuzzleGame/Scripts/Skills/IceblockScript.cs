using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Assets.PuzzleGame.Support;

public class IceblockScript : MonoBehaviourPun, IMagnetic, IPunObservable
{
    public Animator animator;                   // The animator of the iceblock
    public GameObject ice;                      // The iceblock model
    public float lifetime = 8f;                 // Lifetime of the iceblock

    public GameObject frozenObject;              // The frozen object inside the iceblock

    private Rigidbody rb;                       // Rigidbody of the iceblock
    private GameObject magnet;                  // The active magnet
    private bool isActiveMagnetItem;            // If the object is currently affected by the magnet
    private bool isMagnetic;
    private bool isMelting = false;             // Checks if Melt-Method was already triggered
    private double CooldownTimestamp;           // The timestamp at which the magnetic cooldown ends
    private Vector3 windForce = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (frozenObject != null)
        {
            SetFrozenItem(frozenObject.GetPhotonView().ViewID);
        }
        else
        {
            // Find all colliding objects during creation of the iceblock
            Collider[] hitColliders = Physics.OverlapBox(transform.position, new Vector3(0.5f, 0.5f, 0.5f));

            // Search for freezable colliders in the collider array
            foreach (Collider c in hitColliders)
            {
                if (c.gameObject.GetComponent<PhotonView>() && SetFrozenItem(c.gameObject.GetPhotonView().ViewID))
                {
                    photonView.RPC("SetFrozenItem", RpcTarget.Others, c.gameObject.GetPhotonView().ViewID);
                    break;
                }
            }
        }

        if (lifetime > 0)
        {
            Invoke(nameof(Melt), lifetime);
        }
    }

    [PunRPC]
    private bool SetFrozenItem(int objID)
    {
        GameObject obj = PhotonNetwork.GetPhotonView(objID).gameObject;
        if (obj.GetComponent<IFreezable>() != null && !obj.GetComponent<IFreezable>().GetFrozenState())
        {
            // Set the frozen object reference
            frozenObject = obj;

            // If the frozen object is not mine, transfer the ownership
            if (photonView.IsMine)
            {
                if (!frozenObject.GetPhotonView().IsMine)
                {
                    frozenObject.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer);
                }
            }

            // Call the Freeze Method on the frozen object
            frozenObject.GetComponent<IFreezable>().SetFrozenState(gameObject);

            // Set the iceblock magnetic, if the frozen object is magnetic
            if (frozenObject.GetComponent<IMagnetic>() != null)
            {
                isMagnetic = true;
            }

            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        SyncMultiplayerState();

        if (photonView.IsMine == false)
        {
            return;
        }

        // Do several checks if the icblock contains a frozen object
        if (frozenObject)
        {
            // If ownership of Iceblock changes, also change owner of frozen object
            if (!frozenObject.GetPhotonView().IsMine)
            {
                frozenObject.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer);
            }

            // If iceblock is the current active item of magnet, but the player with magnet disconnected, remove iceblock as active item
            if (isActiveMagnetItem && !magnet)
            {
                RemoveMagneticState();
            }

            // Update position of frozen object to the center if the iceblock
            frozenObject.transform.position = ice.transform.position;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Fire"))
        {
            Melt();
        }
    }

    private void Melt()
    {
        if (!isMelting)
        {
            isMelting = true;

            animator.Play("Melt");

            if (!photonView.IsMine)
            {
                return;
            }

            // Destroy the iceblock gameobject after animation has finished
            Invoke(nameof(Kill), animator.GetCurrentAnimatorStateInfo(0).length);
        }
    }

    private void Kill()
    {
        // Remove freeze-status of frozen object, after iceblock melts
        if (frozenObject)
        {
            frozenObject.GetPhotonView().RPC("RemoveFrozenState", RpcTarget.All);
        }
        PhotonNetwork.Destroy(gameObject);
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
        rb.useGravity = true;
    }

    public bool GetMagneticState()
    {
        return isActiveMagnetItem;
    }

    public bool GetEnabledState()
    {
        return isMagnetic;
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

    public void SyncMultiplayerState()
    {
        if (isActiveMagnetItem && !photonView.IsMine)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = false;
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
