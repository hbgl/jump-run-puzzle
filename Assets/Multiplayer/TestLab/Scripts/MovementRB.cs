using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MovementRB : MonoBehaviourPun
{
    public Rigidbody player;
    public float mouseSensitivity = 1000f;
    public GameObject cubePrefab;

    private Camera cam;
    private Vector3 inputVector;
    private float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {

        if (!photonView.IsMine)
        {
            return;
        }
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
        if (!photonView.IsMine)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            PhotonNetwork.Instantiate(this.cubePrefab.name, cubePrefab.transform.position, Quaternion.identity, 0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            transform.position = new Vector3(4.2f, 20, 3.25f);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * 10, transform.position.z);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            transform.position = new Vector3(4.2f, 20, -3.25f);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        //    HandleGravity()
        //{
        //        float currentVerticalSpeed = rigidbody.velocity.y;
        //        if (isGrounded)
        //        {
        //            if (currentVerticalSpeed < 0f)
        //                currentVerticalSpeed = 0f;
        //        }
        //        else if (!isGrounded)
        //        {
        //            currentVerticalSpeed -= gravity * Time.deltaTime;
        //        }

        //        rigidbody.velocity.y = currentVerticalSpeed;
        //    }


        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        inputVector = transform.right * Input.GetAxis("Horizontal") * 10 + transform.up * player.velocity.y + transform.forward * Input.GetAxis("Vertical") * 10;
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        player.velocity = inputVector;
    }
}
