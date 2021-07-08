using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PressurePlate : MonoBehaviour
{
    public List<GameObject> interactableObjects;
    public List<PressurePlate> connectedPlates;
    public bool oneTimeActivation;
    public bool detectPlayers;
    public bool onlyPlayers;

    private Animator animator;
    private int collidingObjects = 0;
    private bool selfActivated;
    private bool otherActivated;
    private bool allActivated;
    private bool wasActivatedOnce;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        otherActivated = true;
        if (connectedPlates.Count > 0)
        {
            foreach (PressurePlate plate in connectedPlates)
            {
                otherActivated = plate.GetCurrentState() == false ? false : otherActivated;
            }
        }

        if (collidingObjects > 0)
        {
            selfActivated = true;
            animator.SetBool("Activated", true);

            if (!allActivated && PhotonNetwork.IsMasterClient)
            {
                if(otherActivated && (!oneTimeActivation || !wasActivatedOnce))
                {
                    allActivated = true;
                    wasActivatedOnce = true;
                    foreach (GameObject interactable in interactableObjects)
                    {
                        interactable.GetComponent<IInteractable>().Interact();
                    }
                }
            }
        }
        else
        {
            animator.SetBool("Activated", false);
            selfActivated = false;
        }
        
        if(allActivated && (!oneTimeActivation || !wasActivatedOnce))
        {
            if(!selfActivated || !otherActivated)
            {
                allActivated = false;
                if (PhotonNetwork.IsMasterClient)
                {
                    foreach (GameObject interactable in interactableObjects)
                    {
                        interactable.GetComponent<IInteractable>().Interact();
                    }
                }
            }
        }

        collidingObjects = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(!detectPlayers)
                return;
        }
        else if(onlyPlayers)
        {
            return;
        }

        collidingObjects++;
    }

    public bool GetCurrentState()
    {
        return selfActivated;
    }
}
