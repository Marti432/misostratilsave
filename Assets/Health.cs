using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int MaxHealth = 40;
    [Header("Debug, no touchy! >:(")]
    public int health;

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    void LateUpdate()
    {
        if (health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Destroy(gameObject);
    }
}
