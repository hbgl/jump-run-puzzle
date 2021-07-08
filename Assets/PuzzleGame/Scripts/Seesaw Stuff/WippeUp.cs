using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WippeUp : MonoBehaviour
{
    public float force = 10f;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(0, force, 0), new Vector3(9f, 0.9f, -1.5f), ForceMode.Impulse);
        }   
    }
}
