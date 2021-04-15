using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public float maxDist;
    public float attractionForce;
    public LayerMask interactionLayer;

    private Camera cam;
    private bool hasItem = false;
    private bool wasKinematic;
    private GameObject activeItem;

    private float maxDistance = 15f;
    private float minDistance = 2f;
    private float distance;

    private float scrollSpeed = 1f;
    private bool hasRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (activeItem)
        {
            hasItem = true;
        }
        else
        {
            hasItem = false;
            activeItem = null;
        }

        if (!hasItem && Input.GetKey(KeyCode.Mouse0))
        {

            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            Debug.DrawLine(ray.origin, ray.GetPoint(maxDist));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDist, interactionLayer))
            {
                activeItem = hit.collider.gameObject;
                if(activeItem.GetComponent<ParentReference>())
                {
                    activeItem = activeItem.GetComponent<ParentReference>().parent;
                }
                distance = hit.distance;
                hasRigidbody = false;
                if (activeItem.GetComponent<Rigidbody>())
                {
                    wasKinematic = hit.rigidbody.isKinematic;
                    hit.rigidbody.isKinematic = true;
                    hasRigidbody = true;
                }

            }
        }

        if (hasItem)
        {
            distance = distance + Input.mouseScrollDelta.y * scrollSpeed;
            if (distance > maxDistance)
            {
                distance = maxDistance;
            }
            else if (distance < minDistance)
            {
                distance = minDistance;
            }
            activeItem.transform.position = cam.transform.position + Vector3.Normalize(cam.transform.forward) * distance;
        }

        if (hasItem && Input.GetKeyUp(KeyCode.Mouse0))
        {
            if(hasRigidbody)
            {
                activeItem.GetComponent<Rigidbody>().isKinematic = wasKinematic;
            }
            activeItem = null;

        }

        //Vector3 dir = Vector3.Normalize(hand.transform.position - hit.transform.position);
        //hit.transform.GetComponent<Rigidbody>().AddForce(dir * attractionForce * Time.deltaTime, ForceMode.Impulse);
        //hit.transform.GetComponent<Rigidbody>().useGravity = false;

    }
}
