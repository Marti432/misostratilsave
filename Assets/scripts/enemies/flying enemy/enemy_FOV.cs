using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_FOV : MonoBehaviour
{
    public float radius;
    [Range(0,360)]
    public float angle;
    AudioSource audioSource;
    [SerializeField] AudioClip alert;
    [SerializeField] float timeToSpotTarget;

    [HideInInspector] public GameObject TargetRef;

    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstructionMask;

    [Header("Debug")]
    public bool canSeeTarget;
    public bool isTriggered;

    private void Start()
    {
        TargetRef = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        yield return new WaitForSeconds(timeToSpotTarget);
        if (canSeeTarget)
        {
            isTriggered = true;
        }
    }
    private void Update()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask) && !canSeeTarget)
                {
                    canSeeTarget = true;
                    if (!isTriggered) audioSource.PlayOneShot(alert);
                    StartCoroutine(FOVRoutine());
                }
                else if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeeTarget = false;
            }
            else 
            {
                canSeeTarget = false;
            }
        }
        else if (canSeeTarget)
        {
            canSeeTarget = false;
        }
    }
}
