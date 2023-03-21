using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class enemy_flying_script : MonoBehaviour
{
    [Header("references")]
    Rigidbody rb;
    [SerializeField] enemy_FOV fov;
    [SerializeField] bobbing bobbing;
    public Transform[] PatrolPositions = new Transform[1];
    [SerializeField] Transform EnemyCamera;
    [SerializeField] Transform Orientation;
    [Header("variables :)")]
    [SerializeField] float orientationLerpSpeed = 5;
    [SerializeField, Range(1, 360)] float angleToMove = 75;
    [SerializeField] float moveSpeed;
    [SerializeField] float moveSpeedSlowed;
    [SerializeField] float moveSpeedTriggered;
    [SerializeField] float distanceToSlow = 4;
    [SerializeField] float patrolPointRadius = 1;
    [SerializeField] float waitTilCamReset = 0.5f;
    [SerializeField] float camLerpSpeed = 2;
    [Header("Use")]
    [SerializeField] bool SlowWhenNearPatrolPoint;
    [Header("Debug")]
    Transform player;
    [SerializeField] int patroledCount;
    public bool resetCamRot;
    Vector3 direction;
    float targetDistance;
    public bool moveForward;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Update()
    {
        CheckIfPatrolled();
        RotateOrientation();
        if (fov.isTriggered)
        {
            SlowWhenNearPatrolPoint = false;
            resetCamRot = true;
            // bobbing.enableBobbing = false;
        }
        if (fov.canSeeTarget)
        {
            resetCamRot = true;
        }
    }
    void LateUpdate()
    {
        if (resetCamRot) RotateCamera();
    }
    void FixedUpdate()
    {
        if (SlowWhenNearPatrolPoint)
        {
            if (moveForward)
            {
                if (targetDistance > distanceToSlow)
                {
                    rb.AddForce(Orientation.forward * moveSpeed * 4);
                }
                else
                {
                    rb.AddForce(Orientation.forward * moveSpeedSlowed * 4);
                }
            }
        }
        else
        {
            if (moveForward && !fov.isTriggered && !fov.canSeeTarget)
            {
                rb.AddForce(Orientation.forward * moveSpeed * 4);
            }
            else if (moveForward && !fov.isTriggered && fov.canSeeTarget)
            {
                rb.AddForce(Orientation.forward * moveSpeedSlowed * 2);
            }
            else if (moveForward && fov.isTriggered)
            {
                rb.drag = 0;
                rb.AddForce(Orientation.forward * moveSpeedTriggered * 4);
            }

        }
    }
    void CheckIfPatrolled()
    {
        if (Vector3.Distance(PatrolPositions[patroledCount].position, Orientation.position) < patrolPointRadius)
        {
            if (patroledCount == PatrolPositions.Length - 1)
            {
                patroledCount = 0;
            }
            else if (patroledCount < PatrolPositions.Length - 1)
            {
                patroledCount++;
            }
            StartCoroutine(resetCamRoutine(waitTilCamReset));
        }
    }
    void RotateOrientation()
    {
        Vector3 targetpos;
        if (!fov.isTriggered)
        {
            targetpos = PatrolPositions[patroledCount].position;

            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            Orientation.rotation = Quaternion.Slerp(Orientation.rotation, targetRotation, orientationLerpSpeed * Time.deltaTime);
        }
        else if (fov.isTriggered || fov.canSeeTarget)
        {
            targetpos = player.position;

            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            Orientation.rotation = Quaternion.Slerp(Orientation.rotation, targetRotation, orientationLerpSpeed * Time.deltaTime * 2);
        }
        else
        {
            targetpos = PatrolPositions[patroledCount].position;
        }

        direction = targetpos - Orientation.position;

        targetDistance = Vector3.Distance(targetpos, transform.position);

        if (Vector3.Angle(direction, Orientation.forward) < angleToMove)
        {
            moveForward = true;
        }
        else
        {
            moveForward = false;
        }
    }
    void RotateCamera()
    {
        if (fov.canSeeTarget || fov.isTriggered)
        {
            Quaternion targetRotation = Quaternion.LookRotation(player.position - Orientation.position, Orientation.up);
            EnemyCamera.rotation = Quaternion.Slerp(EnemyCamera.rotation, targetRotation, Time.deltaTime * camLerpSpeed);
        }
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation(Orientation.forward, Orientation.up);
            EnemyCamera.rotation = Quaternion.Slerp(EnemyCamera.rotation, targetRotation, Time.deltaTime * camLerpSpeed);
        }
    }

    public IEnumerator resetCamRoutine(float waitTil)
    {
        resetCamRot = false;
        yield return new WaitForSeconds(waitTil);
        resetCamRot = true;
    }
}
