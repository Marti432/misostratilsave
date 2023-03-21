using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [SerializeField] Transform cam;
    [SerializeField] Transform grappleHit;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform GrapplePointStart;
    Rigidbody rb;
    playermovement movement;
    [SerializeField] float grappleDistance;
    [SerializeField] float rayAssist;
    // [SerializeField] float grappleForce;
    // [Range(0.05f, 1), SerializeField] float PullForce = 0.4f;
    [SerializeField] float spring = 5;
    [SerializeField] float dampness = 2.5f;
    [SerializeField] float waitBeforeAddForce;
    [SerializeField] LayerMask grappleMask;
    SpringJoint joint;
    Camera camController;
    float startDistance;
    float startVel;
    RaycastHit hit;
    void Start()
    {
        camController = cam.GetComponent<Camera>();
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<playermovement>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2) && !movement.isGrappling)
        {
            if (Physics.SphereCast(cam.position, rayAssist, cam.forward, out hit, grappleDistance, grappleMask))
            {
                StartGrapple();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse2) && movement.isGrappling)
        {
            StopGrapple();
        }
        if (movement.isGrappling)
        {
            float dis = Vector3.Distance(transform.position, hit.point);
            RaycastHit hit2;
            if (Physics.Raycast(transform.position, hit.point - transform.position, out hit2, dis - 0.2f, grappleMask))
                hit.point = hit2.point;
            lineRenderer.SetPosition(1, hit.point);
        }
    }
    void FixedUpdate()
    {
        if (movement.isGrappling)
        {
            ExecuteGrapple();
        }
    }
    void LateUpdate()
    {
        if (hit.collider != null)
            lineRenderer.SetPosition(0, GrapplePointStart.position);
    }

    void StartGrapple()
    {
        grappleHit.gameObject.SetActive(true);
        movement.isGrappling = true;
        lineRenderer.enabled = true;
        startVel = rb.velocity.magnitude;

        StartCoroutine(GrapplingRoutine());

        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = hit.point;
        float dis = Vector3.Distance(transform.position, hit.point);
        startDistance = dis;
    }
    void ExecuteGrapple()
    {
        float dis = Vector3.Distance(transform.position, hit.point) * 0.8f;
        joint.damper = dampness + (rb.velocity.magnitude/dis);
        joint.spring = spring/(dis*0.85f);
        joint.maxDistance = Mathf.Clamp(dis * 0.4f, 1, 10);

        grappleHit.position = hit.point;

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, Mathf.Clamp(startVel, 25, startVel + 10));

        Vector3 directionToObject = hit.point - cam.position;
        if (Vector3.Angle(cam.forward, directionToObject) > camController.fieldOfView) StopGrapple();

    }
    void StopGrapple()
    {
        Destroy(joint);
        StopAllCoroutines();
        grappleHit.gameObject.SetActive(false);
        movement._gravity = movement.gravity;
        movement.isGrappling = false;
        lineRenderer.enabled = false;
    }

    IEnumerator GrapplingRoutine()
    {
        yield return new WaitForSeconds(2);
        StopGrapple();
    }
}
