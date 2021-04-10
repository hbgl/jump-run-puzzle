using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxUp : MonoBehaviour
{
    public float force = 10f;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            var point = GetComponent<Renderer>().bounds.center;
            GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(0, force, 0), point, ForceMode.Impulse);
        }
    }
}
