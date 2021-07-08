using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Fire : MonoBehaviourPun, IInteractable
{
    public bool isActivated;
    public Vector3 respawnPoint;
    private List<GameObject> fireSources = new List<GameObject>();

    private void Awake()
    {
        foreach(Transform child in transform)
        {
            if(child.childCount > 0)
            {
                fireSources.Add(child.GetChild(0).gameObject);
            }
        }
        SetActive(isActivated);
    }

    public void Interact()
    {
        photonView.RPC(nameof(SetActive), RpcTarget.All, !isActivated);
    }

    [PunRPC]
    public void SetActive(bool state)
    {
        isActivated = state;

        if (isActivated)
        {
            foreach (GameObject fire in fireSources)
            {
                fire.GetComponent<ParticleSystem>().Play();
            }
        }
        else
        {
            foreach (GameObject fire in fireSources)
            {
                fire.GetComponent<ParticleSystem>().Stop();
            }
        }
    }
}
