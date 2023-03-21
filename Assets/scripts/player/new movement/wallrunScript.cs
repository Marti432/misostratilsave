using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallrunScript : MonoBehaviour
{
    [SerializeField] MovementController mc;
    Transform Orientation;
    Rigidbody rb;
    [SerializeField] float minWallHeight = 1;
    public float gravityAcceleration = 4;
    [SerializeField] float baseGravity;
    public float speed;
    public float drag;
    [SerializeField, Tooltip("applies a boost to your speed at wallrun start")] float wallrunBoost = 5;
    [SerializeField] Vector2 JumpOffForce = new Vector2(12, 5);
    [HideInInspector] public Vector3 moveDir;
    float maxWallrunSpeed;
    bool canWallRun = true;
    int wallhitId;
    RaycastHit hitWallLeft, hitWallRight, hitWallBackward;
    RaycastHit hitWall;
    [HideInInspector] public float LerpCamtoLeft;
    bool wallClimbCheck;
    void Start()
    {
        rb = mc.rb;
        Orientation = mc.Orientation;
    }
    bool NearGround()
    {
        return Physics.Raycast(transform.position, -Orientation.up, minWallHeight + transform.lossyScale.y, mc.groundMask);
    }

    void Update()
    {
        CheckforWall();
    }

    void FixedUpdate()
    {
        if (mc.state == States.Wallrunning)
        {
            rb.AddForce(-hitWall.normal * 65);
        }
    }


    void CheckforWall()
    {
        if (!NearGround() && canWallRun && WallCheck(Orientation, out wallhitId) && mc.state != States.Wallrunning)
        {
            switch (wallhitId)
            {
                case 0:
                    hitWall = hitWallBackward;
                    break;
                case 1:
                    hitWall = hitWallRight;
                    break;
                case 2:
                    hitWall = hitWallLeft;
                    break;
            }
            StartWallRun();
        }
        else if (mc.state == States.Wallrunning && (!WallCheck(Orientation, out wallhitId) || NearGround()))
        {
            StopWallRun(false, 0);
        }
    }

    public void ExecuteWallRun()
    {
        // Get the move dir
        switch (wallhitId)
        {
            case 0:
                hitWall = hitWallBackward;
                break;
            case 1:
                hitWall = hitWallRight;
                break;
            case 2:
                hitWall = hitWallLeft;
                break;
        }

        Vector3 moveInput = (Orientation.forward * mc.moveInput.y + Orientation.right * mc.moveInput.x).normalized;
        Vector3 Forward = Vector3.Cross(hitWall.normal, Vector3.up);
        LerpCamtoLeft = Vector3.Dot(Forward, Orientation.forward);
        if (Vector3.Dot(Forward, Orientation.forward) > 0)
        {
            float dot = Vector3.Dot(moveInput, Forward);
            // float RoundedDot = float.Parse(dot.ToString("F2"));
            float RoundedDot = Mathf.Round(dot);
            if (RoundedDot != 0)
                moveDir = Forward * RoundedDot;
            else
                moveDir = Forward;
        }
        else
        {
            float dot = Vector3.Dot(moveInput, Forward);
            // float RoundedDot = float.Parse(dot.ToString("F2"));
            float RoundedDot = Mathf.Round(dot);
            if (RoundedDot != 0)
                moveDir = Forward * RoundedDot;
            else
                moveDir = -Forward;
        }

        Debug.DrawLine(transform.position, -hitWall.normal * 5 + transform.position, Color.red);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxWallrunSpeed);
        mc._gravity += gravityAcceleration * Time.deltaTime;
    }
    public void StartWallRun()
    {
        Vector3 Forward = Vector3.Cross(hitWall.normal, Vector3.up);
        maxWallrunSpeed = rb.velocity.magnitude * 2;
        if (Vector3.Dot(Forward, Orientation.forward) > 0)
        {
            rb.velocity = Forward * new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        }
        else
        {
            rb.velocity = -Forward * new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        }
        rb.AddForce(Vector3.Cross(hitWall.normal, transform.up) * wallrunBoost, ForceMode.Impulse);

        mc._gravity = -(Mathf.Clamp(rb.velocity.magnitude, 10, 20)/10) * baseGravity;

        mc.state = States.Wallrunning;
    }

    public void StopWallRun(bool JumpOff, int id)
    {
        if (id == 0) StartCoroutine(WallRunCooldown());
        else StartCoroutine(WallRunCooldown_BIG());
        if (JumpOff)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z) + hitWall.normal * JumpOffForce.x + transform.up * JumpOffForce.y;
        }
        mc._gravity = mc.gravity;
        mc.state = States.AirMovement;
    }

    public IEnumerator WallRunCooldown()
    {
        canWallRun = false;
        yield return new WaitForSeconds(0.45f);
        canWallRun = true;
        // movement.canDoubleJump = true;
    }
    public IEnumerator WallRunCooldown_BIG()
    {
        canWallRun = false;
        yield return new WaitForSeconds(1.5f);
        canWallRun = true;
        // movement.canDoubleJump = true;
    }
    bool WallCheck(Transform WallCheckPos, out int wallId)
    {
        wallId = 0;
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 2:
                    if (Physics.Raycast(WallCheckPos.position, -WallCheckPos.right, out hitWallLeft, 0.8f, mc.groundMask)) {
                        wallId = 2;
                        return true;
                    }
                break;
                case 1:
                    if (Physics.Raycast(WallCheckPos.position, WallCheckPos.right, out hitWallRight, 0.8f, mc.groundMask)) {
                        wallId = 1;
                        return true;
                    }
                break;
                default:
                    if (Physics.Raycast(WallCheckPos.position, -WallCheckPos.forward, out hitWallBackward, 0.8f, mc.groundMask)) {
                        wallId = 0;
                        return true;
                    }
                break;
            }
        }
        return false;
    }
}



//sex
