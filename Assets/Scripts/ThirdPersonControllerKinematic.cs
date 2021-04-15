using JumpRunPuzzle.Seesaw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonControllerKinematic : MonoBehaviour, ISeesawAgent
{
    public float Speed = 6.0f;
    public float JumpHeight = 1.0f;
    public float JumpPadJumpHeight = 5.0f;
    public float Gravity = -12f;
    public float TurnSmoothTime = 0.1f;
    public float PushPower = 2.0f;
    public float Mass = 50f;
    public float SeesawLegJumpHeight = 3.0f;
    
    private CharacterController Controller;
    private Transform Camera;
    private float VelocityY;
    private float LastFallVelocityY;
    private float TurnSmoothVelocity;
    private bool IsGrounded;
    private bool IsOnJumpPad;
    private bool IsOnSeesaw;
    private float SeesawJumpHeight;
    private Seesaw Seesaw;

    private List<ControllerColliderHit> Collisions = new List<ControllerColliderHit>(12);

    private void Start()
    {
        Controller = gameObject.GetComponent<CharacterController>();
        Camera = GameObject.Find("Main Camera").transform;
    }

    void Update()
    {
        Debug.Log(IsGrounded);
        var wasGrounded = IsGrounded;
        var isGrounded = Controller.isGrounded;
        IsGrounded = isGrounded;
        var velocity = new Vector3(0f, VelocityY, 0f); ;
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

        // Handle horizontal movement.
        var move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        if (move.magnitude > 0.1f)
        {
            var targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + Camera.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVelocity, TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            var moveVelocity = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized * Speed;
            velocity.x = moveVelocity.x;
            velocity.z = moveVelocity.z;
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
            velocity.y += Mathf.Sqrt(SeesawJumpHeight * -3.0f * Gravity);
            SeesawJumpHeight = 0f;
        }
        else if (Input.GetButton("Jump") && IsOnJumpPad)
        {
            // Get boost on jump pad.
            velocity.y = Mathf.Sqrt(JumpPadJumpHeight * -3.0f * Gravity);
        }
        else if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Normal jump.
            velocity.y = Mathf.Sqrt(JumpHeight * -3.0f * Gravity);
        }
        velocity.y += Gravity * Time.deltaTime;

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
        return;

        if (hit.gameObject.name == "Board")
        {
            //var force = new Vector3(0, -10f, 0);
            //hit.collider.attachedRigidbody.AddForceAtPosition(force, hit.point, ForceMode.Force);
            //return;
        }
        
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
            return;

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f)
        {
            if (VelocityY <= -5.25f)
                Debug.Log($"Hit down {VelocityY} {LastFallVelocityY}");
            return;
        }
        Debug.Log("Hit");
        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * PushPower;
    }

    public void SetSeesawJumpHeight(float value)
    {
        SeesawJumpHeight = value;
    }
}
