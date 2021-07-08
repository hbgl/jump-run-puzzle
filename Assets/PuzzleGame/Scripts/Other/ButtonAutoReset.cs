using Assets.PuzzleGame.Scripts.Interfaces;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAutoReset : MonoBehaviourPun, IButton
{
    public List<GameObject> interactableObjects;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    [PunRPC]
    public void ToggleState()
    {
        foreach (GameObject interactable in interactableObjects)
        {
            interactable.GetComponent<IInteractable>().Interact();
        }
        gameObject.GetPhotonView().RPC(nameof(TriggerAnimator), RpcTarget.All);
    }

    [PunRPC]
    public void TriggerAnimator()
    {
        animator.SetTrigger("Push");
    }
}
