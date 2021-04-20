using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using JumpRunPuzzle.Seesaw;

public class CharacterMovement : MonoBehaviourPun, ISeesawAgent
{
    public GameObject addons;
    public GameObject skills;
    
    public float MouseSensitivity = 1000f;
    public GameObject PlayerUI;

    public float Speed = 6.0f;
    public float JumpHeight = 1.0f;
    public float JumpPadJumpHeight = 5.0f;
    public float Gravity = -12f;
    [Range(0f, 10f)]
    public float GravityJumpFactor = 2.0f;
    [Range(0f, 10f)]
    public float GravityFallFactor = 2.0f;
    public float TurnSmoothTime = 0.1f;
    public float PushPower = 2.0f;
    public float Mass = 50f;
    public float SeesawLegJumpHeight = 3.0f;

    private CharacterController Controller;
    private Camera Camera;
    private float RotationX = 0f;
    private float VelocityY;
    private float LastFallVelocityY;
    private bool IsGrounded;
    private bool IsOnJumpPad;
    private bool IsOnSeesaw;
    private float SeesawJumpHeight;
    private Seesaw Seesaw;
    private List<ControllerColliderHit> Collisions = new List<ControllerColliderHit>(12);

    // Start is called before the first frame update
    void Start()
    {
        // GameObject nameUI = Instantiate(playerUI);
        // nameUI.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

        if (!photonView.IsMine)
        {
            return;
        }
        addons.SetActive(false);

        GetComponent<FreezeGun>().enabled = true;
        GetComponent<Magnet>().enabled = true;
        // Camera Setup for the Player
        Camera = Camera.main;
        Camera.transform.parent = transform;
        Camera.transform.rotation = Quaternion.Euler(Vector3.zero);
        Camera.transform.localPosition = new Vector3(0, 0.6f, 0);

        skills.transform.parent = cam.transform;
        skills.transform.localPosition = Vector3.zero;

        Controller = gameObject.GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        // Camera Movement

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            } else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        // Handle rotation
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;
        RotationX -= mouseY;
        RotationX = Mathf.Clamp(RotationX, -90f, 90f);
        Camera.transform.localRotation = Quaternion.Euler(RotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Player velocity
        var velocity = new Vector3(0f, VelocityY, 0f);

        // Handle horizontal movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        var moveVelocity = (transform.right * x + transform.forward * z) * Speed;
        velocity += moveVelocity;

        // Handle vertical movement
        var wasGrounded = IsGrounded;
        var isGrounded = Controller.isGrounded;
        IsGrounded = isGrounded;
        if (isGrounded)
        {
            if (!wasGrounded)
            {
                LastFallVelocityY = VelocityY;
                Debug.Log($"Player last fall velocity {LastFallVelocityY}");
            }
            if (velocity.y < 0)
            {
                // When the player is walking on the ground set a negative vertical velocity
                // to push the model onto the ground. This prevents model from hopping down
                // slopes.
                velocity.y = -4f;
            }
        }

        var pushSeesawJumpHeight = 0f;
        if (Seesaw != null)
        {
            // Check if hitting the seesaw from above with another agent on
            // the other side.
            if (!wasGrounded && Seesaw.HasOtherAgent(this))
            {
                pushSeesawJumpHeight = Mathf.Abs(LastFallVelocityY) / 2.0f;

                if (Input.GetButton("Jump"))
                {
                    // Boost the jump height by fixed amount, simulating pushing
                    // the seesaw with the legs.
                    pushSeesawJumpHeight += SeesawLegJumpHeight;
                }
                else
                {
                    // Reduce the jump height by fixed amount, simulating mass of
                    // the seesaw and friction.
                    pushSeesawJumpHeight -= Seesaw.JumpHeightReduction;
                }

                pushSeesawJumpHeight = Mathf.Max(pushSeesawJumpHeight, 0f);
            }
        }

        if (pushSeesawJumpHeight >= 0.5f)
        {
            // Push the other agent up.
            Seesaw.GetOtherAgent(this).SetSeesawJumpHeight(pushSeesawJumpHeight);
        }
        else if (SeesawJumpHeight >= 0.5f)
        {
            // Get pushed up by the other agent.
            velocity.y += Mathf.Sqrt(SeesawJumpHeight * -GravityJumpFactor * Gravity);
            SeesawJumpHeight = 0f;
        }
        else if (Input.GetButton("Jump") && IsOnJumpPad)
        {
            // Get boost on jump pad.
            velocity.y = Mathf.Sqrt(JumpPadJumpHeight * -GravityJumpFactor * Gravity);
        }
        else if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Normal jump.
            velocity.y = Mathf.Sqrt(JumpHeight * -GravityJumpFactor * Gravity);
        }
        velocity.y += Gravity * GravityFallFactor * Time.deltaTime;

        Collisions.Clear();

        Controller.Move(velocity * Time.deltaTime);

        // Handle all collisions from the last move.
        HandleCollisions();
        Collisions.Clear();

        // Save current vertical velocity for gravity calculations.
        VelocityY = velocity.y;
    }

    private void HandleCollisions()
    {
        ControllerColliderHit jumpPadHit = null;
        ControllerColliderHit seesawHit = null;

        var hitCount = Collisions.Count;
        for (var i = 0; i < hitCount; i++)
        {
            var hit = Collisions[i];
            if (jumpPadHit == null && hit.gameObject.CompareTag("JumpPad") && hit.transform.position.y <= Controller.bounds.min.y)
            {
                jumpPadHit = hit;
            }
            if (seesawHit == null && hit.gameObject.CompareTag("SeesawPad") && hit.transform.position.y <= Controller.bounds.min.y)
            {
                seesawHit = hit;
            }
        }

        // Handle jump pad
        IsOnJumpPad = jumpPadHit != null;

        // Handle seesaw
        if (seesawHit != null)
        {
            IsOnSeesaw = true;
            Seesaw = seesawHit.gameObject.GetComponentInParent<Seesaw>();
            Seesaw.SetAgent(this, seesawHit.gameObject.transform.GetSiblingIndex());
        }
        else if (Seesaw != null)
        {
            IsOnSeesaw = false;
            Seesaw.RemoveAgent(this);
            Seesaw = null;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Collisions.Add(hit);
    }

    public void SetSeesawJumpHeight(float value)
    {
        SeesawJumpHeight = value;
    }
}
