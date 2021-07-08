using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Wind : MonoBehaviour, IInteractable
{
    public float strength;
    public bool isActivated;
    public GameObject particles;
    
    private CapsuleCollider triggerCollider;
    private Vector3 triggerCenter;
    private float triggerRadius;
    private float triggerHeight;

    private void Awake()
    {
        triggerCollider = GetComponent<CapsuleCollider>();
        triggerCenter = triggerCollider.center;
        triggerRadius = triggerCollider.radius;
        triggerHeight = triggerCollider.height;

        SetActive(isActivated);
    }


    public void Interact()
    {
        gameObject.GetPhotonView().RPC(nameof(SetActive), RpcTarget.All, !isActivated);
    }

    [PunRPC]
    public void SetActive(bool state)
    {
        isActivated = state;

        if (isActivated)
        {
            particles.GetComponent<ParticleSystem>().Play();
            triggerCollider.center = triggerCenter;
            triggerCollider.radius = triggerRadius;
            triggerCollider.height = triggerHeight;
        }
        else
        {
            particles.GetComponent<ParticleSystem>().Stop();
            triggerCollider.center = Vector3.zero;
            triggerCollider.radius = 0;
            triggerCollider.height = 0;
        }
    }
}
