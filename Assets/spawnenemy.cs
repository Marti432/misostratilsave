using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnenemy : MonoBehaviour
{
    [SerializeField] enemy_flying_script Enemy;
    [Header("No touchy >:(")]
    [SerializeField] Transform[] patrolPoints;
    public float respawnTime;
    public bool hasBase;
    public bool respawn;
    public void Initialize(Transform[] _patrolPoints)
    {
        patrolPoints = _patrolPoints;
    }

    public void Respawn()
    {
        enemy_flying_script spawnedEnemy = Instantiate(Enemy, transform.position, Quaternion.identity, transform) as enemy_flying_script;
        spawnedEnemy.PatrolPositions = patrolPoints;
        respawn = false;
    }

}
