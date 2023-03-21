using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playermovement : MonoBehaviour
{
    [Header("references")]
    [SerializeField] playercam camController;
    [SerializeField] bobbing _bobbing;
    [SerializeField] Transform groundCheckpos; 
    [SerializeField] Transform Orientation;
    wallrunScript wallrun;
    Rigidbody rb;
    [Header("KeyBinds")]
    public KeyCode crouchKey = KeyCode.LeftControl;
    [Header("GroundCheck")]
    public LayerMask groundMask;
    [SerializeField] float groundDistance = 0.2f;
    public float gravity = 12;
    [Header("AirStrafing stuff")]
    [SerializeField] float MaxAirSpeed;
    [SerializeField] float AirStrafeForce;
    [Header("movement variables")]
    [SerializeField] float groundSpeed = 6.5f;
    [SerializeField] float airSpeed = 1;
    [SerializeField] float crouchSpeed = 4.5f;
    [SerializeField] float slideSpeed = 1;
    [SerializeField] float sprintSpeed = 10;
    [SerializeField] public float wallrunSpeed = 3;
    [SerializeField] float sprintFOV = 10;
    [SerializeField] float wallrunFOV = 10;
    [SerializeField] float jumpForce = 5.5f;
    [SerializeField] float doubleJumpForce = 5.5f;
    [SerializeField] float doubleJumpHForce = 5;
    [HideInInspector] public float maxWallrunSpeed;
    [HideInInspector] public Vector2 moveInput;
    [Header("air & ground variables")]
    [Tooltip("please leave at zero unless sure"), SerializeField] float airDrag = 0;
    // [SerializeField] float grappleDrag = 1;
    [SerializeField] float groundDrag = 5;
    [SerializeField] float slideDrag = 0.5f;
    [SerializeField] float wallrunDrag = 1;
    [SerializeField] float airStrafeCooldown = 0.25f;
    Vector3 moveDir;
    float moveMultiplier = 5;
    [Header("Bools and Debug")]
    [SerializeField] TMP_Text spedText;
    [HideInInspector] public RaycastHit wallhit;
    float AirStrafeCooldownTime;
    public bool isGrappling;
    public bool canAirStrafe;
    public bool canDoubleJump;
    public bool isGrounded;
    public bool resetCrouch;
    public bool isSprinting;
    public bool isSliding;
    public bool isCrouching;
    public bool isWallrunning;
    public bool isDashing;
    // public bool wallrunStartBonusHeigth;
    public float _gravity;
    [HideInInspector] public bool wallLeft, wallBackward, wallForward;
    [HideInInspector] public float _speed;
    void Start()
    {
        wallrun = GetComponent<wallrunScript>();
        rb = GetComponent<Rigidbody>();
    }
    
    bool onGround()
    {
        bool ground = Physics.Raycast(transform.position, -transform.up, groundDistance + transform.lossyScale.y, groundMask);
        // if (ground) wallrunStartBonusHeigth = false;
        return ground;
    }
    public Vector3 slopedMoveDir()
    {
        if (!isGrounded)
        {
            moveDir = (Orientation.right * moveInput.x + Orientation.forward * moveInput.y).normalized;
            if (isWallrunning)
            {
                Vector3 wallForward = Vector3.Cross(wallhit.normal, transform.up);
                if (Vector3.Dot(wallForward, Orientation.forward) > 0)
                {
                    moveDir = wallForward * moveInput.y;
                }
                else
                {
                    moveDir = -wallForward * moveInput.y;
                }
            }
        }
        else
        {
            moveDir = (Orientation.right * moveInput.x + Orientation.forward * moveInput.y).normalized;
            RaycastHit hit;
            Physics.Raycast(transform.position, -transform.up, out hit, groundDistance + transform.lossyScale.y + 1, groundMask);
            return Vector3.ProjectOnPlane(moveDir, hit.normal);
        }
        return moveDir;
    }

    void Update()
    {
        spedText.text = Mathf.Round(rb.velocity.magnitude).ToString();
        Inputs();
        States();

        isGrounded = onGround();
    }
    void FixedUpdate()
    {
        if (isGrounded)
        {
            rb.AddForce(slopedMoveDir() * _speed * moveMultiplier);
        }
        else
        {
            rb.AddForce(-transform.up * _gravity);
            if (!isWallrunning && canAirStrafe) rb.AddForce(AirMovement(Orientation.right * moveInput.x) * moveMultiplier * _speed, ForceMode.VelocityChange);
            else if (!isWallrunning && !canAirStrafe) rb.AddForce(slopedMoveDir() * _speed);
            else rb.AddForce(slopedMoveDir() * _speed * moveMultiplier);
        }
    }
    void Jump()
    {
        if (isGrounded)
        {
            // rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
            _gravity = gravity;
            Debug.Log("Jump");
            canDoubleJump = true;
        }
        else if (isWallrunning)
        {
            Debug.Log("WallJump");
            wallrun.StopWallRun(true, 0);
        }
        else if (canDoubleJump && !isWallrunning && !isGrounded)
        {
            if (rb.velocity.y < 0) rb.velocity = new Vector3(rb.velocity.x, doubleJumpForce, rb.velocity.z);
            else rb.velocity += Vector3.up * doubleJumpForce;
            rb.AddForce(slopedMoveDir() * doubleJumpHForce, ForceMode.Impulse);
            Debug.Log("DoubleJump");
            canDoubleJump = false;
            _gravity = gravity;
        }
    }
    public void Sprint(bool state)
    {
        _speed = state ? sprintSpeed : _speed;
        isSprinting = state;
        if (state) 
        {
            isCrouching = false;
        }
    }
    void States()
    {
        if (isWallrunning && !(wallForward || wallBackward)) 
            camController.Zangle = wallLeft ? Mathf.Lerp(camController.Zangle, -15, Time.deltaTime * 8) : Mathf.Lerp(camController.Zangle, 15, Time.deltaTime * 8);
        else camController.Zangle = Mathf.Lerp(camController.Zangle, 0, Time.deltaTime * 8);
        if (isGrounded)
        {
            canDoubleJump = true;
            _gravity = gravity;
            canAirStrafe = false;
            if (!isCrouching && !isSliding) 
            {
                // if (moveInput.x != 0 || moveInput.y != 0) _bobbing.enableBobbing = true;
                // else _bobbing.enableBobbing = false;
                rb.drag = groundDrag;
                if (isSprinting) camController.plusFov = Mathf.Lerp(camController.plusFov, sprintFOV, Time.deltaTime * 10);
                else {
                    _speed = groundSpeed;
                    camController.plusFov = Mathf.Lerp(camController.plusFov, 0, Time.deltaTime * 10);
                }
                resetCrouch = true;
            }
            else if (isCrouching && !isSliding)
            {
                // if (moveInput.x != 0 || moveInput.y != 0) _bobbing.enableBobbing = true;
                // else _bobbing.enableBobbing = false;
                _speed = crouchSpeed;
                rb.drag = groundDrag;
                camController.plusFov = Mathf.Lerp(camController.plusFov, 0, Time.deltaTime * 10);
            }
            else if (isSliding)
            {
                // _bobbing.enableBobbing = false;
                rb.drag = slideDrag;
                _speed = slideSpeed;
                camController.plusFov = Mathf.Lerp(camController.plusFov, 0, Time.deltaTime * 10);
            }
            else camController.plusFov = Mathf.Lerp(camController.plusFov, 0, Time.deltaTime * 10);
        }
        else
        {
            if (isWallrunning)
            {
                canAirStrafe = false;
                // _bobbing.enableBobbing = true;
                canDoubleJump = true;
                rb.drag = wallrunDrag;
                _speed = wallrunSpeed;
                camController.plusFov = Mathf.Lerp(camController.plusFov, wallrunFOV, Time.deltaTime * 10);

                if (rb.velocity.x > maxWallrunSpeed)
                    rb.velocity = new Vector3(Mathf.Lerp(rb.velocity.x, maxWallrunSpeed, Time.deltaTime), rb.velocity.y, rb.velocity.z);
                if (rb.velocity.z > maxWallrunSpeed)
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Mathf.Lerp(rb.velocity.z, maxWallrunSpeed, Time.deltaTime));
            }
            else
            {
                if (isGrappling && !isDashing) {
                    _gravity = 0;
                    // rb.drag = grappleDrag;
                }
                else _gravity += gravity * Time.deltaTime;
                AirStrafeCooldownTime += Time.deltaTime;
                if (AirStrafeCooldownTime >= airStrafeCooldown)
                {
                    canAirStrafe = true;
                }
                // _bobbing.enableBobbing = false;
                camController.plusFov = Mathf.Lerp(camController.plusFov, 0, Time.deltaTime * 10);
                _speed = airSpeed;
                rb.drag = airDrag;
            }
            resetCrouch = true;

            isSprinting = false;
        }
    }

    void Inputs()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        if (!Input.GetKey(KeyCode.LeftShift) && moveInput.y != 1) Sprint(false);
        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift) && !isSprinting && moveInput.y > 0 && !isSliding && !isCrouching) Sprint(true);
        }
    }

    // Strafing (pls keep)
    Vector3 AirMovement(Vector3 vector3)
    {
        // project the velocity onto the movevector
        Vector3 projVel = Vector3.Project(rb.velocity, vector3);

        // check if the movevector is moving towards or away from the projected velocity
        bool isAway = Vector3.Dot(vector3, projVel) <= 0f;

        // only apply force if moving away from velocity or velocity is below MaxAirSpeed
        if (projVel.magnitude < MaxAirSpeed || isAway)
        {
            // calculate the ideal movement force
            Vector3 vc = vector3.normalized * AirStrafeForce;

            //cap it if it would accelerate beyond MaxAirSpeed directly.
            if (!isAway)
            {
                vc = Vector3.ClampMagnitude(vc, MaxAirSpeed + projVel.magnitude);
            }
            else
            {
                vc = Vector3.ClampMagnitude(vc, MaxAirSpeed - projVel.magnitude);
            }
            

            // Apply the force
            return vc;
        }
        return Vector3.zero;
    }
}
