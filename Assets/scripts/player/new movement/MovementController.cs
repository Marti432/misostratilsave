using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public enum States
{
    Walking,
    Sprinting,
    AirMovement,
    Wallrunning,
    Sliding,
    Crouching,
    Grappling
}
[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    public States state;
    [Header("LayerMasks")]
    public LayerMask groundMask;
    [Header("Transforms && GameObject")]
    [SerializeField] TMP_Text SpeedText;
    public Rigidbody rb;
    public Transform Orientation;
    [Header("Scripts")]
    public GroundMovement groundMovement;
    public wallrunScript wallrunMovement;
    [Header("movement vars")]
    Vector3 moveDir;
    [HideInInspector] public Vector2 moveInput;
    float speed;
    public float gravity;
    public float _gravity;
    [HideInInspector] public bool isGrounded;
    bool useGravity;
    bool canAirStrafe;
    float airStrafeTimer;
    [HideInInspector] public bool canDoubleJump;
    [Header("vars used only by other scripts")]
    [HideInInspector] public bool useBobbing;

    void Update()
    {
        Inputs();
        StateHandler();
        SpeedText.text = rb.velocity.magnitude.ToString("F2");
        isGrounded = Physics.Raycast(transform.position, -transform.up, 0.2f + transform.lossyScale.y, groundMask);
        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift) && state != States.Sprinting) state = States.Sprinting;
            if (!(Input.GetKey(KeyCode.LeftShift) || moveInput.y < 0)) state = States.Walking;
        }
        else
        {
            if (state != States.Wallrunning) state = States.AirMovement;
        }

        if (airStrafeTimer >= 0.1f) canAirStrafe = true;
    }
    void StateHandler()
    {
        switch (state)
        {
            case States.Walking:
                canDoubleJump = true;
                canAirStrafe = false;
                airStrafeTimer = 0;
                // visuals
                useBobbing = true;
                // speed vars
                speed = groundMovement.groundSpeed;
                moveDir = groundMovement.GroundedMovement();
                rb.drag = groundMovement.GroundDrag;
                // gravity check
                useGravity = false;
                _gravity = 0;
                //Jump
                if (Input.GetKeyDown(KeyCode.Space)) groundMovement.Jump();
            break;

            case States.Sprinting:
                canDoubleJump = true;
                canAirStrafe = false;
                airStrafeTimer = 0;
                // visuals
                useBobbing = true;
                // speed vars
                speed = groundMovement.sprintSpeed;
                moveDir = groundMovement.GroundedMovement();
                rb.drag = groundMovement.GroundDrag;
                // gravity check
                useGravity = false;
                _gravity = 0;
                //Jump
                if (Input.GetKeyDown(KeyCode.Space)) groundMovement.Jump();
            break;

            case States.AirMovement:
                airStrafeTimer += Time.deltaTime;
                // visuals
                useBobbing = false;
                // speed vars
                speed = groundMovement.airSpeed;
                moveDir = groundMovement.AirMovement(Orientation.right * moveInput.x);
                rb.drag = groundMovement.airDrag;
                // gravity check
                useGravity = true;
                _gravity += gravity * Time.deltaTime;
                if (Input.GetKeyDown(KeyCode.Space) && canDoubleJump) groundMovement.DoubleJump();
            break;

            case States.Wallrunning:
                canDoubleJump = true;
                canAirStrafe = false;
                airStrafeTimer = 0;
                // visuals
                useBobbing = true;
                // speed vars
                speed = wallrunMovement.speed;
                moveDir = wallrunMovement.moveDir;
                rb.drag = wallrunMovement.drag;
                // gravityCheck
                useGravity = true;
                // execute wallrun movement
                wallrunMovement.ExecuteWallRun();
                if (Input.GetKeyDown(KeyCode.Space)) wallrunMovement.StopWallRun(true, 0);
                if (Input.GetKey(KeyCode.LeftControl)) wallrunMovement.StopWallRun(false, 1);
            break;
        }
    }

    void FixedUpdate()
    {
        if (state == States.AirMovement && canAirStrafe) rb.AddForce(moveDir * speed, ForceMode.VelocityChange);
        else rb.AddForce(moveDir * speed);

        if (useGravity)
        {
            rb.AddForce(Vector3.down * _gravity);
        }
    }

    void Inputs()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
    }
}
