using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Powerup : MonoBehaviourPun
{
    public PowerupType type;

    public void Collect()
    {
        if(GameManager.Instance.gui.AddItem(type))
        {
            photonView.RPC(nameof(RemovePowerup), RpcTarget.All);
        }
    }

    [PunRPC]
    public void RemovePowerup()
    {
        if(photonView.IsMine)
        {
            PhotonNetwork.Destroy(photonView);
        }
    }
}

public enum PowerupType
{
    Magnet,
    Icewand,
    Glider,
    Hand
}