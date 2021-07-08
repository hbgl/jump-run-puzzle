using JumpRunPuzzle.Support;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private Collider Collider;

    private void Start()
    {
        Collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //collision.gameObject.GetComponent<ThirdPersonControllerKinematic>().IsOnJumpPad = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //collision.gameObject.GetComponent<ThirdPersonControllerKinematic>().IsOnJumpPad = false;
        }
    }

    public bool IsInBounds(Bounds bounds)
    {
        return BoundsUtils.IsInPadBounds(Collider.bounds, bounds);
    }
}
