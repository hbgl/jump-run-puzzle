using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyDoor : MonoBehaviour, IInteractable
{
    public float velocity = 5f;

    public bool isActive;

    private Rigidbody rb;

    public void Interact()
    {
        var a = gameObject.GetPhotonView();
        gameObject.GetPhotonView().RPC(nameof(SetActive), RpcTarget.All, !isActive);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isActive)
        {
            rb.velocity = new Vector3(0f, velocity, 0f);
            
        }
        else
        {
            rb.velocity = new Vector3(0f, -velocity, 0f);
        }
    }

    [PunRPC]
    public void SetActive(bool state)
    {
        isActive = state;
    }
}
