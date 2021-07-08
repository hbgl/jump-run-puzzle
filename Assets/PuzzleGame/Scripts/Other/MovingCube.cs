using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCube : MonoBehaviour, IInteractable
{
    public Vector3 delta;

    public float speed;

    private Vector3 originalPosition;

    private Vector3 target;

    public void Interact()
    {
        gameObject.GetPhotonView().RPC(nameof(Move), RpcTarget.All);
    }

    void Start()
    {
        originalPosition = transform.position;
        target = originalPosition;
    }

    void Update()
    {
        if (transform.position != target)
        {
            var step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }
    }

    [PunRPC]
    public void Move()
    {
        if (target == originalPosition)
        {
            target = originalPosition + delta;
        }
        else
        {
            target = originalPosition;
        }
    }
}
