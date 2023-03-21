using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_flying_Health : MonoBehaviour
{
    enemy_FOV fov;
    [SerializeField] spawnenemy respawn;
    [SerializeField] int MaxHealth = 40;
    [SerializeField] LayerMask explosionHitMask;
    [SerializeField] GameObject explosionAnim;
    enemy_flying_script en;
    public float explosionRad = 2.5f;
    [SerializeField] float explosionForce = 4;
    [SerializeField] int ExplosionDamage = 35;
    [SerializeField] LayerMask detonationMask;
    [Tooltip("leave at '0' if you want to have it at default size")]
    public float detonationRad;
    [Header("Debug, no touchy! >:(")]
    public int health;

    void Start()
    {
        en = GetComponent<enemy_flying_script>();
        respawn = GetComponentInParent<spawnenemy>();
        if (!respawn.hasBase) respawn.Initialize(en.PatrolPositions);
        respawn.hasBase = true;
        if (detonationRad == 0)
        {
            detonationRad = transform.lossyScale.x;
        }
        fov = GetComponentInChildren<enemy_FOV>();
        health = MaxHealth;
    }

    void Update()
    {
        if (health <= 0)
        {
            Die();
        }
        if (Physics.CheckSphere(transform.position, detonationRad, detonationMask) && fov.isTriggered)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public void Die()
    {
        Explode();
        respawn.respawn = true;
        respawn.Invoke("Respawn", respawn.respawnTime);
        Destroy(gameObject);
    }
    void Explode()
    {
        GameObject explosion = Instantiate(explosionAnim, transform.position, Quaternion.identity);
        Destroy(explosion, 1.5f);
        Collider[] explosionHit = Physics.OverlapSphere(transform.position, explosionRad, explosionHitMask);

        foreach (Collider col in explosionHit)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, col.transform.position - transform.position, out hit, explosionRad);
            hit.collider.GetComponentInChildren<Rigidbody>()?.AddForce(explosionForce * transform.position - hit.transform.position, ForceMode.Impulse);
            hit.collider.GetComponent<Health>()?.TakeDamage(ExplosionDamage);
        }
    }
}
