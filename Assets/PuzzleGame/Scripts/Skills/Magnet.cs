using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Assets.PuzzleGame.Scripts.General_Classes;
using Assets.PuzzleGame.Support;

public class Magnet : MonoBehaviour, IPunObservable
{
    public float maxActivationDistance = 10f;   // The distance where the magnet can affect an object
    public float maxPushDistance = 10f;         // Max push distance with mousewheel of the active item
    public float minPullDistance = 2f;          // Min pull distance with mousewheel of the active item
    public float pushAndPullForce = 1000f;      // The force of push and pull
    public float scrollSpeed = 1f;              // Scroll speed of the mousewheel

    public GameObject objectPosition;           // Destination of the active item
    public GameObject blueTrail;
    public GameObject redTrail;

    private Camera cam;
    private Animator animator;
    private GameObject activeItem;
    private bool hasItem = false;               // If the magnet has an active item
    private float currentDistance;              // Current distance between the magnet and objectPosition
    private float mouseScrolling;               // The mousewheel value
    private int activeItemViewID;               // The Photon ID of the active item

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hasItem);
            if(activeItem)
            {
                stream.SendNext(activeItem.GetPhotonView().ViewID);
            } else
            {
                stream.SendNext(-1);
            }
        }
        else
        {
            hasItem = (bool)stream.ReceiveNext();
            activeItemViewID = (int)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Updating the other players
        if (GetComponent<PhotonView>().IsMine == false)
        {

            // Set Magnet Trails and disable animator
            blueTrail.SetActive(hasItem);
            redTrail.SetActive(hasItem);
            animator.enabled = !hasItem;

            // If magnet has item and item is set
            if (hasItem && activeItem)
            {
                // Rotate Magnet model in direction of the active item
                transform.LookAt(activeItem.transform);
                transform.Rotate(new Vector3(90, 0, 0), Space.Self);

                // Scale Magnet Trails depending on the distance of the active item
                blueTrail.transform.localScale = new Vector3(1, 1, Vector3.Distance(blueTrail.transform.position, activeItem.transform.position));
                redTrail.transform.localScale = new Vector3(1, 1, Vector3.Distance(redTrail.transform.position, activeItem.transform.position));
            }
            // If magnet has item but item is not yet set
            else if (hasItem && !activeItem)
            {
                activeItem = PhotonNetwork.GetPhotonView(activeItemViewID).gameObject;
            }
            // If magnet has no item
            else
            {
                activeItem = null;
            }

            return;
        }

        // If the magnet has an active item
        if (activeItem && activeItem.GetComponent<IMagnetic>().GetMagneticState())
        {
            hasItem = true;

            // Rotate Magnet model in direction of the active item
            transform.LookAt(activeItem.transform);
            transform.Rotate(new Vector3(90, 0, 0), Space.Self);

            // Activate Magnet Trails and disable animator
            blueTrail.SetActive(true);
            redTrail.SetActive(true);
            animator.enabled = false;

            // Scale Magnet Trails depending on the distance of the active item
            blueTrail.transform.localScale = new Vector3(1, 1, Vector3.Distance(blueTrail.transform.position, activeItem.transform.position));
            redTrail.transform.localScale = new Vector3(1, 1, Vector3.Distance(redTrail.transform.position, activeItem.transform.position));
        }
        else if (hasItem)
        {
            hasItem = false;
            activeItem = null;

            // Disable Magnet Trails and disable animator
            blueTrail.SetActive(false);
            redTrail.SetActive(false);
            animator.enabled = true;
        }

        // If Magnet has currently no active item and left mouse key is pressed
        if (!hasItem && Input.GetKey(KeyCode.Mouse0))
        {

            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            Debug.DrawRay(ray.origin, cam.transform.forward * maxActivationDistance, Color.red);

            if (Physics.Raycast(ray, out RaycastHit hit, maxActivationDistance))
            {
                if (hit.rigidbody)
                {
                    var magnetic = hit.rigidbody.GetComponent<IMagnetic>();
                    if (magnetic != null && !magnetic.GetMagneticState() && !magnetic.IsOnCooldown() && magnetic.GetEnabledState())
                    {
                        // Set the active item reference
                        activeItem = hit.rigidbody.gameObject;

                        // If the active item is not mine, transfer the ownership
                        if (!activeItem.GetPhotonView().IsMine)
                        {
                            activeItem.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer);
                        }

                        // Call the Magnetic Method on the active item and initialize the current distance to the object
                        activeItem.GetComponent<IMagnetic>().SetMagneticState(gameObject);
                        currentDistance = hit.distance;
                    }
                }
            }
        }

        // If Magnet currently has an active item
        if (hasItem)
        {
            // Set new object-position, affected by the mouse wheel (push and pull) and camera position
            mouseScrolling = Input.mouseScrollDelta.y;
            currentDistance = Mathf.Clamp((currentDistance + mouseScrolling * scrollSpeed), minPullDistance, maxPushDistance);
            objectPosition.transform.position = cam.transform.position + Vector3.Normalize(cam.transform.forward) * currentDistance;

            // Calculate the velocity and direction, which the acitve item should move to
            Vector3 dir = Vector3.Normalize(objectPosition.transform.position - activeItem.transform.position);
            Vector3 velocity = dir * Vector3.Distance(objectPosition.transform.position, activeItem.transform.position) * pushAndPullForce;
            velocity.ApplyMoveConstraints(activeItem.GetComponent<IMagnetic>().MoveConstraints);
            activeItem.transform.GetComponent<Rigidbody>().velocity = velocity;

            // Remove the active item, if left mouse key is released or if the magnetic item is on cooldown.
            var removeActiveItem = Input.GetKeyUp(KeyCode.Mouse0)
                || activeItem.transform.GetComponent<IMagnetic>().IsOnCooldown();
            if (removeActiveItem)
            {
                activeItem.GetComponent<IMagnetic>().RemoveMagneticState();
                activeItem = null;
            }
        }
    }

    private void OnDisable()
    {
        if(hasItem)
        {
            activeItem.GetComponent<IMagnetic>().RemoveMagneticState();
            activeItem = null;
        }
    }
}
