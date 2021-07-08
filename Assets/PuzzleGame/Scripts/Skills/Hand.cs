using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{

    public float maxActivationDistance = 2f;

    private GameObject activeObject;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (activeObject && Input.GetKeyDown(KeyCode.Mouse0))
        {
            activeObject.GetComponent<IPortable>().RemovePortableState();
            activeObject = null;
        }

        if (!activeObject && Input.GetKey(KeyCode.Mouse0))
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            Debug.DrawRay(ray.origin, cam.transform.forward * maxActivationDistance, Color.magenta);

            if (Physics.Raycast(ray, out RaycastHit hit, maxActivationDistance))
            {
                if (hit.rigidbody && hit.rigidbody.GetComponent<IPortable>() != null && !hit.rigidbody.GetComponent<IPortable>().GetPortableState())
                {
                    activeObject = hit.transform.gameObject;
                    activeObject.transform.position = transform.position;
                    activeObject.GetComponent<IPortable>().SetPortableState(gameObject);
                }
            }
        }

        if(activeObject)
        {
            // Calculate the velocity and direction, which the acitve item should move to
            Vector3 dir = Vector3.Normalize(transform.position - activeObject.transform.position);
            activeObject.transform.GetComponent<Rigidbody>().velocity = dir * Vector3.Distance(transform.position, activeObject.transform.position) * 100;
        }


    }
}
