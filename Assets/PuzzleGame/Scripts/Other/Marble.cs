using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : MonoBehaviour, IInteractable
{
    public Vector3 resetPosition;

    public void Interact()
    {
        gameObject.GetPhotonView().RPC(nameof(Reset), RpcTarget.All);
    }

    [PunRPC]
    public void Reset()
    {
        transform.position = resetPosition;
        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
