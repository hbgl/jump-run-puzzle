using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Door : MonoBehaviour, IInteractable
{
    public bool isOpen;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        SetActive(isOpen);
    }

    public void Interact()
    {
        gameObject.GetPhotonView().RPC(nameof(SetActive), RpcTarget.All, !isOpen);
    }

    [PunRPC]
    public void SetActive(bool state)
    {
        isOpen = state;

        if (isOpen)
        {
            animator.SetBool("Open", true);
        }
        else
        {
            animator.SetBool("Open", false);
        }
    }
}
