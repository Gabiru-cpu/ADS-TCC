using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10f; // Adjust the damage value as needed

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Enemy") && !other.CompareTag("EnemyHit"))
        {
            // Destroy the projectile when it hits something other than the player or enemy
            Destroy(gameObject);
        }
    }
}
