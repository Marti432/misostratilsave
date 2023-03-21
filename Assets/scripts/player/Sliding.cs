using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    Rigidbody rb;
    playermovement movement;
    public bool canReset;
    [SerializeField] Transform Orientation;
    [SerializeField] Transform RoomForStandCheck;
    [SerializeField] float speedForSlide;
    [SerializeField] float slideForce;
    [SerializeField] float velocityForBoost;
    [SerializeField] Vector3 crouchScale;
    Vector3 originalScale;
    public bool canSlide = true;
    [HideInInspector] public Vector3 slideDir;
    [Header("Debug")]
    RaycastHit hit;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<playermovement>();
        originalScale = transform.localScale;
    }
    void Update()
    {
        canReset = movement.resetCrouch;
        if (!movement.isCrouching)
        {
            Crouch(false);
            if (movement.isSliding) Slide(false);
        }
        //if (Input.GetKey(movement.crouchKey) && !isSliding && movement.isGrounded && !isCrouching)
        if (Input.GetKey(movement.crouchKey) && !movement.isSliding && movement.isGrounded && canReset && !movement.isCrouching)
        {
            if (rb.velocity.magnitude > speedForSlide && canSlide)
            {
                Slide(true);
            }
            movement.Sprint(false);
            Crouch(true);
        }
        if ((!Input.GetKey(movement.crouchKey) && movement.isCrouching) || (movement.isSprinting && movement.isSliding))
        {
            Crouch(false);
            if (movement.isSliding) Slide(false);
        }
        if (movement.isSliding && rb.velocity.magnitude < speedForSlide)
        {
            Slide(false);
        }
    }
    public void Slide(bool state)
    {
        movement.isSliding = state;
        if (state)
        {
            canReset = false;
            slideDir = movement.slopedMoveDir();
            if (rb.velocity.magnitude < velocityForBoost)
            {
                rb.AddForce(slideDir * slideForce, ForceMode.Impulse);
            }
            else
            {
                rb.AddForce(slideDir * slideForce * 0.2f, ForceMode.Impulse);
            }
        }
        else
        {
            StartCoroutine(SlideCooldownRoutine());
        }
    }

    public void Crouch(bool state)
    {
        //if (!state && Physics.CheckSphere(RoomForStandCheck.position, transform.lossyScale.z + 0.2f, movement.groundMask)) return;
        movement.isCrouching = state;
        if (state)
        {
            movement.isSprinting = false;
            transform.localScale = crouchScale;
            if (movement.isGrounded) rb.AddForce(-transform.up * 5, ForceMode.Impulse);
        }
        else transform.localScale = originalScale;
    }
    IEnumerator SlideCooldownRoutine()
    {
        canSlide = false;
        yield return new WaitForSeconds(3);
        canSlide = true;
    }
}
