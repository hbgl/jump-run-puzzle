using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public float maxDist;
    public float attractionForce;
    public LayerMask interactionLayer;
    public Animator animator;
    public GameObject objectPosition;

    private Camera cam;
    private bool hasItem = false;
    // private bool wasKinematic;
    private GameObject activeItem;

    private float maxDistance = 10f;
    private float minDistance = 2f;
    private float distance;
    private int direction = -1;

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
            if(animator.GetBool("magnetIsPushing") == true)
            {
                animator.SetBool("magnetIsPushing", false);
            }
        }

        animator.SetBool("magnetIsActive", hasItem);

        if (!hasItem && Input.GetKey(KeyCode.Mouse0))
        {

            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            Debug.DrawLine(ray.origin, ray.GetPoint(maxDist));
            if (Physics.Raycast(ray, out RaycastHit hit, maxDist, interactionLayer))
            {
                activeItem = hit.collider.gameObject;
                if (activeItem.GetComponent<ParentReference>())
                {
                    activeItem = activeItem.GetComponent<ParentReference>().parent;
                }
                distance = hit.distance;
                hasRigidbody = false;
                if (activeItem.GetComponent<Rigidbody>())
                {
                    //wasKinematic = hit.rigidbody.isKinematic;
                    //hit.rigidbody.isKinematic = true;
                    hit.rigidbody.useGravity = false;
                    hasRigidbody = true;
                }

            }
        }

        if (hasItem)
        {
            float mouseScrolling = Input.mouseScrollDelta.y;

            if (mouseScrolling < 0)
            {
                animator.SetBool("magnetIsPushing", false);
                direction = -1;
            }
            else if (mouseScrolling > 0)
            {
                animator.SetBool("magnetIsPushing", true);
                direction = 1;
            }

            distance += mouseScrolling * scrollSpeed;
            if (distance > maxDistance)
            {
                distance = maxDistance;
            }
            else if (distance < minDistance)
            {
                distance = minDistance;
            }

            //activeItem.transform.position = cam.transform.position + Vector3.Normalize(cam.transform.forward) * distance;
            objectPosition.transform.position = cam.transform.position + Vector3.Normalize(cam.transform.forward) * distance;

            Vector3 dir = Vector3.Normalize(objectPosition.transform.position - activeItem.transform.position);
            activeItem.transform.GetComponent<Rigidbody>().velocity = dir * Vector3.Distance(objectPosition.transform.position, activeItem.transform.position) * attractionForce * Time.deltaTime;
            //activeItem.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //activeItem.transform.GetComponent<Rigidbody>().AddForce(dir * Vector3.Distance(objectPosition.transform.position, activeItem.transform.position) * attractionForce * Time.deltaTime, ForceMode.Impulse);
        }

        if (hasItem && Input.GetKeyUp(KeyCode.Mouse0))
        {
            if(hasRigidbody)
            {
                //activeItem.GetComponent<Rigidbody>().isKinematic = wasKinematic;
                activeItem.GetComponent<Rigidbody>().useGravity = true;
            }
            activeItem = null;

        }

    }
}
