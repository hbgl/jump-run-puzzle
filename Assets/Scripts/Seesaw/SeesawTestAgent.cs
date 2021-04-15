using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpRunPuzzle.Seesaw
{
    public class SeesawTestAgent : MonoBehaviour, ISeesawAgent
    {
        public float Gravity = -12f;

        private CharacterController Controller;
        private Seesaw Seesaw;
        private List<ControllerColliderHit> Collisions = new List<ControllerColliderHit>(12);
        private float SeesawJumpHeight;
        private float VelocityY;
        private float LastFallVelocityY;
        private bool IsGrounded;
        private bool IsOnSeesaw;

        private void Start()
        {
            Controller = gameObject.GetComponent<CharacterController>();
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
                    Debug.Log($"Test last fall velocity {LastFallVelocityY}");
                }
                if (velocity.y < 0)
                {
                    // When the player is walking on the ground set a negative vertical velocity
                    // to push the model onto the ground. This prevents model from hopping down
                    // slopes.
                    velocity.y = -4f;
                }
            }
            if (Seesaw != null)
            {
                var pushSeesawJumpHeight = 0f;
                if (!wasGrounded && Seesaw.HasOtherAgent(this))
                {
                    // Hitting the seesaw from above.
                    pushSeesawJumpHeight = Mathf.Abs(LastFallVelocityY) / 2.0f;
                    pushSeesawJumpHeight -= Seesaw.JumpHeightReduction;
                    pushSeesawJumpHeight = Mathf.Max(pushSeesawJumpHeight, 0f);
                }
                if (pushSeesawJumpHeight > Seesaw.JumpHeightMin)
                {
                    Seesaw.GetOtherAgent(this).SetSeesawJumpHeight(pushSeesawJumpHeight);
                }
                else if(SeesawJumpHeight >= Seesaw.JumpHeightMin)
                {
                    velocity.y += Mathf.Sqrt(SeesawJumpHeight * -3.0f * Gravity);
                    SeesawJumpHeight = 0f;
                }
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
            ControllerColliderHit seesawHit = null;

            var hitCount = Collisions.Count;
            for (var i = 0; i < hitCount; i++)
            {
                var hit = Collisions[i];
                if (seesawHit == null && hit.gameObject.CompareTag("SeesawPad") && hit.transform.position.y <= Controller.bounds.min.y)
                {
                    seesawHit = hit;
                }
            }

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
}