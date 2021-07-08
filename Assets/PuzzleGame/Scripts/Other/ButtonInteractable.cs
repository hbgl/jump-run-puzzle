using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Assets.PuzzleGame.Scripts.Interfaces;

public class ButtonInteractable : MonoBehaviourPun, IButton
{
    public List<GameObject> interactableObjects;

    private Animator animator;
    private bool isActivated;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    [PunRPC]
    public void ToggleState()
    {
        isActivated = !isActivated;

        foreach (GameObject interactable in interactableObjects)
        {
            interactable.GetComponent<IInteractable>().Interact();
        }
        animator.SetBool("Activated", isActivated);
    }

}

