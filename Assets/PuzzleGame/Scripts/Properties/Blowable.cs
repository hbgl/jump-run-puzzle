using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blowable : MonoBehaviour
{
    private Rigidbody Rigidbody;
    private Vector3 WindForce = Vector3.zero;

    private void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (WindForce != Vector3.zero)
        {
            Rigidbody.AddForce(WindForce, ForceMode.Force);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wind"))
        {
            WindForce += other.transform.forward * other.GetComponent<Wind>().strength;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wind"))
        {
            WindForce -= other.transform.forward * other.GetComponent<Wind>().strength;
        }
    }
}
