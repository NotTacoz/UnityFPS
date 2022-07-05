using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] public float _maxHealth = 100.0f;
    [SerializeField] public float _defence = 0.0f;

    public void TakeDamage(float damage)
    {
        _maxHealth -= damage;
        if (_maxHealth <= 0)
        {
            Die();
        }
    }

    void Die ()
    {
        Destroy(gameObject);
    }
}
