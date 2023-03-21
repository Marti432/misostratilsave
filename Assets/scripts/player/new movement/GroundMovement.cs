using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    [Header("Ground vars")]
    public float groundSpeed;
    public float sprintSpeed;
    public float GroundDrag = 5;
    [Header("Air and jump")]
    public float airDrag = 0;
    public float airSpeed = 2;
    [SerializeField] float JumpForce;
    [SerializeField] float doubleJumpUpForce;
    [SerializeField] float doubleJumpForce;
    [SerializeField] float VelForNoDoubleJumpBoost;
    [SerializeField] float MaxAirSpeed;
    [SerializeField] float AirStrafeForce;
    [SerializeField] MovementController mc;
    Rigidbody rb;
    Transform Orientation;
    Vector3 moveDir;
    bool useAirMovement;
    void Awake()
    {
        rb = mc.rb;
        Orientation = mc.Orientation;
    }
    public void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, JumpForce, rb.velocity.z);
    }
    public void DoubleJump()
    {
        mc._gravity = mc.gravity;
        mc.canDoubleJump = false;
        if (new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude > VelForNoDoubleJumpBoost)
            rb.velocity = new Vector3(rb.velocity.x, doubleJumpForce, rb.velocity.z);
        else
            rb.velocity = doubleJumpForce * mc.moveInput.x * Orientation.right + Vector3.up * doubleJumpUpForce + doubleJumpForce * mc.moveInput.y * Orientation.forward;
    }


    public  Vector3 AirMovement(Vector3 vector3)
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

    public Vector3 GroundedMovement()
    {
        Vector3 moveDir = (Orientation.right * mc.moveInput.x + Orientation.forward * mc.moveInput.y).normalized;
        RaycastHit hit;
        Physics.Raycast(transform.position, -transform.up, out hit, 0.2f + transform.lossyScale.y + 1, mc.groundMask);

        return Vector3.ProjectOnPlane(moveDir, hit.normal);
    }
}
