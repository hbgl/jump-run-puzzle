using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using JumpRunPuzzle.Seesaw;
using JumpRunPuzzle.Support;
using Assets.PuzzleGame.Scripts.Interfaces;

public class CharacterMovement : MonoBehaviourPunCallbacks, ISeesawAgent, IPunObservable
{
    public GameObject addons;
    public Transform[] eyes;
    public GameObject skills;

    public GameObject magnet;
    public GameObject icewand;
    public GameObject gliderCheck;

    private bool gliderIsEnabled;
    private bool handIsEnabled;
    private bool OnMagnetizedObject;

    public float MouseSensitivity = 10f;
    public float collectDistance = 3f;

    public float Speed = 6.0f;
    public float GlideSpeed = 10f;
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
    public float SeesawBoostEnergy = 3.0f;
    public double MagneticCooldown = 2.0;
    public Animator gliderAnimator;

    private CharacterController Controller;
    private Camera Camera;
    private float RotationX = 0f;
    private float VelocityY;
    private float LastFallVelocityY;
    private bool IsGrounded;
    private bool isGliding = false;
    private float SeesawKnockUpVelocity = 0f;

    private Vector3 windVelocity = Vector3.zero;
    private float eyeRotation = 0f;

    private JumpPad JumpPad;
    private SeesawPad SeesawPad;
    private List<ControllerColliderHit> Collisions = new List<ControllerColliderHit>(12);

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine == false)
        {
            // Set Item-Position for 3rd Person View of Remote Player
            skills.transform.parent = addons.transform;
            magnet.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
            icewand.transform.localPosition = new Vector3(-0.36f, -0.12f, 0.31f);

            return;
        }

        addons.SetActive(false);

        // Camera Setup for the Player
        Camera = Camera.main;
        Camera.transform.parent = transform;
        Camera.transform.rotation = Quaternion.Euler(Vector3.zero);
        Camera.transform.localPosition = new Vector3(0, 0.6f, 0);

        skills.transform.parent = Camera.transform;
        skills.transform.localPosition = Vector3.zero;

        Controller = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false)
        {
            foreach(Transform eye in eyes)
            {
                float finalEyeRotation = Mathf.Clamp(Mathf.Repeat(eyeRotation + 180, 360) - 180, -55f, 55f);
                eye.localRotation = Quaternion.Euler(finalEyeRotation, 0f, 0f);
            }
            return;
        }

        // Camera Movement and Rotation
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;
        RotationX -= mouseY;
        RotationX = Mathf.Clamp(RotationX, -90f, 90f);
        Camera.transform.localRotation = Quaternion.Euler(RotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Player Velocity
        Vector3 velocity = new Vector3(0f, VelocityY, 0f);

        // Handle Horizontal Movement
        isGliding = gliderCheck.activeInHierarchy;
        float x = (isGliding ? 0 : Input.GetAxisRaw("Horizontal"));
        float z = (isGliding ? 1 : Input.GetAxisRaw("Vertical"));

        Vector3 moveVelocity = (transform.right * x + transform.forward * z).normalized * (isGliding ? GlideSpeed : Speed);
        velocity += moveVelocity;

        // Handle Vertical Movement
        var wasGrounded = IsGrounded;
        IsGrounded = Controller.isGrounded;

        if (IsGrounded)
        {
            if (!wasGrounded)
            {
                gliderCheck.SetActive(false);
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

        // DONE: Check if variable >> seesawEnergy << needs to be synced
        var seesawEnergy = 0f;
        if (SeesawPad != null)
        {
            // Check if hitting the seesaw from above.
            if (!wasGrounded)
            {
                seesawEnergy += 0.5f * Mass * LastFallVelocityY * LastFallVelocityY;
                if (Input.GetButton("Jump"))
                {
                    // Boost the seesaw energy by fixed amount, simulating pushing
                    // the seesaw with the legs.
                    seesawEnergy += SeesawBoostEnergy;
                }
            }
        }

        // Push onto seesaw.
        if (seesawEnergy > 0f)
        {
            // DONE: Check if method >> AddEnergy << needs to be synced in class SeesawPad
            SeesawPad.photonView.RPC("AddEnergy", RpcTarget.All, seesawEnergy);
            //SeesawPad.AddEnergy(seesawEnergy);
        }
        // Get pushed up by seesaw.
        else if (SeesawKnockUpVelocity >= 0.5f)
        {
            velocity.y = SeesawKnockUpVelocity;
            SeesawKnockUpVelocity = 0;
        }
        // Get boost on jump pad.
        else if (Input.GetButton("Jump") && JumpPad != null)
        {
            velocity.y = Mathf.Sqrt(JumpPadJumpHeight * -GravityJumpFactor * Gravity);
        }
        // Normal jump.
        else if (Input.GetButtonDown("Jump") && IsGrounded && !OnMagnetizedObject)
        {
            velocity.y = Mathf.Sqrt(JumpHeight * -GravityJumpFactor * Gravity);
        }

        // Set glider state
        gliderAnimator.SetBool("IsGliding", isGliding);
        if (gliderIsEnabled && !IsGrounded && Input.GetButtonDown("Jump"))
        {
            gliderCheck.SetActive(!isGliding);
        }
        else if (isGliding && !gliderIsEnabled)
        {
            gliderCheck.SetActive(false);
        }

        // Calculate velocity and vertical velocity depending on if glider is active and affected by wind
        velocity.y = (isGliding ? -1 : velocity.y);
        velocity.y += Gravity * GravityFallFactor * Time.deltaTime;
        velocity = (isGliding ? velocity + windVelocity : velocity);

        // Handle all collisions from the last move.
        Collisions.Clear();
        Controller.Move(velocity * Time.deltaTime);
        HandleCollisions();
        Collisions.Clear();

        // Save current vertical velocity for gravity calculations.
        VelocityY = velocity.y;

        // Collecting Powerups
        if (Input.GetKeyDown(KeyCode.C))
        {
            Ray ray = new Ray(Camera.transform.position, Camera.transform.forward);
            Debug.DrawRay(ray.origin, Camera.transform.forward * collectDistance, Color.magenta);

            if (Physics.Raycast(ray, out RaycastHit hit, collectDistance))
            {
                var powerup = hit.transform.gameObject.GetComponent<Powerup>();
                var button = hit.transform.gameObject.GetComponent<IButton>();
                if (powerup != null)
                {
                    powerup.Collect();
                }
                else if(button != null)
                {
                    hit.transform.gameObject.GetPhotonView().RPC("ToggleState", RpcTarget.MasterClient);
                }
            }
        }
    }

    private void HandleCollisions()
    {
        JumpPad jumpPad = null;         // Single Jump Pad
        SeesawPad seesawPad = null;     // Jump Pad on a Seesaw
        bool onMagnetizedObject = false;

        var groundY = Controller.bounds.min.y + GetComponent<CapsuleCollider>().radius;

        var hitCount = Collisions.Count;
        for (var i = 0; i < hitCount; i++)
        {
            var hit = Collisions[i];

            // Searching for collisions with a pad
            if (jumpPad == null && hit.gameObject.CompareTag("JumpPad"))
            {
                var pad = hit.gameObject.GetComponent<JumpPad>();
                if (jumpPad.IsInBounds(Controller.bounds))
                {
                    jumpPad = pad;
                }
            }
            if (seesawPad == null && hit.gameObject.CompareTag("SeesawPad"))
            {
                var pad = hit.gameObject.GetComponent<SeesawPad>();
                if (pad.IsInBounds(Controller.bounds))
                {
                    seesawPad = pad;
                }
            }

            if (hit.rigidbody && hit.point.y < groundY)
            {
                var magnetic = hit.rigidbody.GetComponent<IMagnetic>();
                if (magnetic != null && !magnetic.CanStandOn && magnetic.GetMagneticState())
                {
                    Debug.Log("Cooldown");
                    hit.rigidbody.gameObject.GetPhotonView().RPC("Cooldown", RpcTarget.All, MagneticCooldown);
                    onMagnetizedObject = true;
                }
            }

            // Searching for collisions with pushable objects and push them
            // TODO: Transfer ownership when pushing to be able to sync position
            // TODO: isKinematic-Check might cause issues, Objects can be kinematic on one client but not on the other
            //if (this.handIsEnabled && Input.GetKey(KeyCode.Mouse0))
            //{
            //    var rigidbody = hit.collider.attachedRigidbody;
            //    var isPushable = hit.gameObject.GetComponent<Pushable>();
            //    if (isPushable && rigidbody != null && !rigidbody.isKinematic && hit.moveDirection.y >= -0.3f)
            //    {
            //        var pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
            //        var pushForce = pushDir * PushPower;
            //        rigidbody.AddForceAtPosition(pushForce, rigidbody.worldCenterOfMass, ForceMode.Impulse);
            //    }
            //}
        }

        OnMagnetizedObject = onMagnetizedObject;

        // Handle jump pad
        JumpPad = jumpPad;

        // Check seesaw pad changed.
        // DONE: Check if need to sync method calls >> RemoveAgent / AddAgent << in class SeesawPad
        if (SeesawPad != seesawPad)
        {
            if (SeesawPad != null)
            {
                // Remove agent from old pad.
                SeesawPad.photonView.RPC("RemoveAgent", RpcTarget.All, photonView.ViewID);
                //SeesawPad.RemoveAgent(this);
            }
            if (seesawPad != null)
            {
                seesawPad.photonView.RPC("AddAgent", RpcTarget.All, photonView.ViewID);
                //seesawPad.AddAgent(this);
            }
            SeesawPad = seesawPad;
        }
    }

    void FixedUpdate()
    {
        // Push pull objects.
        // The implementation is pretty shitty. The object and the player are not "stuck"
        // together. So when pulling, the player will outrun the pulled object.
        var mouse0 = Input.GetKey(KeyCode.Mouse0);
        var mouse1 = Input.GetKey(KeyCode.Mouse1);
        if (this.handIsEnabled && (mouse0 || mouse1))
        {
            Ray ray = Camera.main.ViewportPointToRay(Vector3.one * 0.5f);
            Debug.DrawRay(ray.origin, ray.direction * 1f, Color.green);
            if (Physics.Raycast(ray, out var hit, 1f))
            {
                var rigidbody = hit.collider.attachedRigidbody;
                var isPushable = hit.transform.gameObject.GetComponent<Pushable>();
                if (isPushable && rigidbody != null && !rigidbody.isKinematic)
                {
                    // If the active item is not mine, transfer the ownership
                    if (!rigidbody.gameObject.GetPhotonView().IsMine)
                    {
                        rigidbody.gameObject.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer);
                    }
                    var pushDir = new Vector3(ray.direction.x, 0f, ray.direction.z);
                    if (mouse1)
                    {
                        pushDir = -pushDir;
                    }
                    var pushForce = pushDir * PushPower;
                    rigidbody.AddForceAtPosition(pushForce, rigidbody.worldCenterOfMass, ForceMode.Impulse);
                }
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Collisions.Add(hit);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wind"))
        {
            windVelocity += other.transform.forward * other.GetComponent<Wind>().strength;
        }

        if (other.CompareTag("LevelClear"))
        {
            GameManager.Instance.TriggerLevelEnd();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wind"))
        {
            windVelocity -= other.transform.forward * other.GetComponent<Wind>().strength;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Fire"))
        {
            transform.position = other.gameObject.GetComponentInParent<Fire>().respawnPoint;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Camera.transform.rotation.eulerAngles.x);
        }
        else
        {
            eyeRotation = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void SetVelocity(float velocity)
    {
        SeesawKnockUpVelocity = velocity;
    }

    float ISeesawAgent.GetMass()
    {
        return Mass;
    }

    public void SetActiveSkill(PowerupType skill)
    {
        PhotonNetwork.OpCleanRpcBuffer(photonView);
        photonView.RPC(nameof(RPCSetActiveSkill), RpcTarget.AllBuffered, skill);
    }

    [PunRPC]
    public void RPCSetActiveSkill(PowerupType skill)
    {
        magnet.SetActive(false);
        icewand.SetActive(false);
        gliderIsEnabled = false;
        handIsEnabled = false;

        switch (skill)
        {
            case PowerupType.Magnet:
                magnet.SetActive(true);
                break;
            case PowerupType.Icewand:
                icewand.SetActive(true);
                break;
            case PowerupType.Glider:
                gliderIsEnabled = true;
                break;
            case PowerupType.Hand:
                handIsEnabled = true;
                break;
        }

    }

}
