using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyClass
{
    Grunt,
    Tick,
    Drone,
    Tank
}
public class enemyAI : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    [SerializeField] Transform movePositionTransform;
    [SerializeField] Transform Player;
    [SerializeField] Transform thisCamera;
    [SerializeField] float radius;
    [SerializeField] LayerMask PlayerMask;
    [SerializeField] Room currentRoom;
    [Header("Enemy Logic")]
    [SerializeField] EnemyClass enemyClass; 
    [SerializeField] float DistanceToRun;
    [SerializeField] float rangeAroundAllies;
    [Header("Debug")]
    public bool hasSetNewDestintation;
    public bool dontChangeTargetPos;
    public bool Action;
    GameObject[] enemies;
    NavMeshPath path;
    int allyID;
    public bool stopForASec;


    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = Random.Range(2.5f, 3.5f);
    }
    void Update()
    {
        switch (enemyClass)
        {
            case EnemyClass.Grunt:
                GruntBehaviour();
            break;
        }
        navMeshAgent.destination = movePositionTransform.position;
        
    }

    void GruntBehaviour()
    {
        float Distance = Vector3.Distance(Player.position, transform.position);
        // if (Vector3.Distance(currentRoom.enemies[GetClosestAlly()].transform.position, movePositionTransform.position) < DistanceToRun && !stopForASec)
        // {
        //     movePositionTransform.position = -(currentRoom.enemies[GetClosestAlly()].transform.position - transform.position).normalized * DistanceToRun;
        //     stopForASec = true;
        //     StartCoroutine(StopForSecRoutine());
        // }
        // if (stopForASec)
        // {
        //     return;
        // }
        if (Vector3.Distance(transform.position, movePositionTransform.position) < 1)
        {
            Action = true;
        }
        if (Distance < DistanceToRun && !hasSetNewDestintation)
        {
            bool canSeePlayer = Physics.Raycast(thisCamera.position, Player.position - thisCamera.position, radius, PlayerMask);
            if (canSeePlayer)
            {
                if (currentRoom.enemyNumber == 1)
                {
                    foreach (GameObject enemy in currentRoom.enemies)
                    {
                        if (enemy.activeInHierarchy && !hasSetNewDestintation)
                        {
                            movePositionTransform.position = enemy.transform.position + new Vector3(
                                Random.Range(-rangeAroundAllies, rangeAroundAllies),
                                0,
                                Random.Range(-rangeAroundAllies, rangeAroundAllies));
                            hasSetNewDestintation = true;
                        }
                    }
                }
                else if (currentRoom.enemyNumber > 1)
                {
                    movePositionTransform.position = currentRoom.enemies[GetClosestAlly()].transform.position + new Vector3(
                                Random.Range(-rangeAroundAllies, rangeAroundAllies),
                                0,
                                Random.Range(-rangeAroundAllies, rangeAroundAllies));
                    hasSetNewDestintation = true;
                }
            }
        }
        if (hasSetNewDestintation && Vector3.Distance(transform.position, movePositionTransform.position) < 2)
        {
            movePositionTransform.position = transform.position;    
            hasSetNewDestintation = false;
            dontChangeTargetPos = false;
        }
        else if (Distance > DistanceToRun && Physics.CheckSphere(transform.position, radius, PlayerMask) && dontChangeTargetPos && !hasSetNewDestintation)
        {
            movePositionTransform.position = transform.position;
        }
        else if (!Physics.CheckSphere(transform.position, radius, PlayerMask) && !hasSetNewDestintation)
        {
            RaycastHit hit;
            Physics.Raycast(Player.position, Vector3.down, out hit, 100);
            if (hit.collider != null)
            {
                Debug.Log("ToPlayer");
                movePositionTransform.position = hit.point;
            }
            dontChangeTargetPos = true;
        }
    }

    int GetClosestAlly()
    {
        float startDistance = 500;
        int ID = 0;
        float[] distanceToAlly = new float[currentRoom.enemies.Length];
        for (int i = 0; i < currentRoom.enemies.Length; i++)
        {
            if (currentRoom.enemies[i].activeInHierarchy && currentRoom.enemies[i].gameObject != gameObject)
            {
                movePositionTransform.position = currentRoom.enemies[i].transform.position;
                distanceToAlly[i] = Vector3.Distance(movePositionTransform.position, transform.position);
                Debug.Log(distanceToAlly[i]);
                if (distanceToAlly[i] < startDistance)
                {
                    startDistance = distanceToAlly[i];
                    ID = i;
                }
            }
        }
        return ID;
    }

    // IEnumerator StopForSecRoutine()
    // {
    //     yield return new WaitForSeconds(1);
    //     stopForASec = false;
    // }
}
