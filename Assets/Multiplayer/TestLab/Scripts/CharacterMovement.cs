using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterMovement : MonoBehaviourPun
{
    public float mouseSensitivity = 1000f;
    public CharacterController controller;
    public float speed = 8f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;


    public GameObject playerUI;

    private Camera cam;
    private float xRotation = 0f;
    private Vector3 velocity;


    // Start is called before the first frame update
    void Start()
    {
        GameObject nameUI = Instantiate(playerUI);
        nameUI.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

        //if (!photonView.IsMine)
        //{
        //    return;
        //}
        transform.Find("Addons").gameObject.SetActive(false);

        // Camera Setup for the Player
        cam = Camera.main;
        cam.transform.parent = transform;
        cam.transform.rotation = Quaternion.Euler(Vector3.zero);
        cam.transform.localPosition = new Vector3(0, 0.6f, 0);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!photonView.IsMine)
        //{
        //    return;
        //}

        // Camera Movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Player Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Gravity and Jumping

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime + move * speed * Time.deltaTime);
    }
}
