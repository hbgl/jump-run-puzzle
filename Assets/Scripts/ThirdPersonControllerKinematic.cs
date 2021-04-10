using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonControllerKinematic : MonoBehaviour
{
    public float Speed = 6.0f;
    public float JumpHeight = 1.0f;
    public float Gravity = -12f;
    public float TurnSmoothTime = 0.1f;
    public float PushPower = 2.0f;
    public float Mass = 50f;

    private CharacterController Controller;
    private Transform Camera;
    private float VelocityY;
    private float LastFallVelocityY;
    private float TurnSmoothVelocity;
    private bool IsGrounded;

    private void Start()
    {
        Controller = gameObject.GetComponent<CharacterController>();
        Camera = GameObject.Find("Main Camera").transform;
    }

    void Update()
    {
        var wasGrounded = IsGrounded;
        var isGrounded = Controller.isGrounded;
        IsGrounded = isGrounded;
        var velocity = new Vector3(0f, VelocityY, 0f); ;
        if (isGrounded)
        {
            if (!wasGrounded)
            {
                LastFallVelocityY = VelocityY;
                Debug.Log($"Last fall velocity {LastFallVelocityY}");
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

        // Handle jumping.
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(JumpHeight * -3.0f * Gravity);
        }
        velocity.y += Gravity * Time.deltaTime;

        Controller.Move(velocity * Time.deltaTime);
        
        // Save current vertical velocity for gravity calculations.
        VelocityY = velocity.y;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
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
}
