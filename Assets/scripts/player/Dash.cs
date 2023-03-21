using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    Rigidbody rb;
    playermovement movement;
    playercam camController;
    [SerializeField] Transform _camera;
    [SerializeField] float dashSpeed = 20;
    [SerializeField] float dashDuration = 0.5f;
    // [SerializeField] float dashCooldown = 1.5f;
    bool canDash = true;
    void Start()
    {
        camController = _camera.GetComponent<playercam>();
        movement = GetComponent<playermovement>();
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        movement.isDashing = !canDash;
        if ((movement.isGrounded || movement.isWallrunning) && !canDash) canDash = true;
        if (Input.GetKeyDown(KeyCode.Mouse2) && canDash && !movement.isWallrunning)
        {
            StartCoroutine(dashRoutine());
        }
    }

    IEnumerator dashRoutine()
    {
        canDash = false;
        Vector3 saveVelocity = rb.velocity;
        float saveGravity = movement._gravity;

        movement._gravity = 0;
        if (saveVelocity.magnitude > dashSpeed && Vector3.Dot(rb.velocity, _camera.forward) > 0) rb.velocity = _camera.forward * rb.velocity.magnitude;
        else rb.velocity = _camera.forward * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        // if (!movement.isWallrunning && !movement.isGrounded) rb.velocity = saveVelocity;
        if (!movement.isGrounded && !movement.isWallrunning) movement._gravity = saveGravity;
        
    }
}
