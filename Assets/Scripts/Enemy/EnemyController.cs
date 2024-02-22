using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private AIPath aiPath;
    [SerializeField]
    private int maxHealth = 100;
    private int curentHealth;

    void Update()
    {
        ChangeDirection();
    }
    void ChangeDirection()
    {
        // Đổi hướng quái
        if (aiPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector2(1f, 1f); // Đổi X thành dương
        }
        else if (aiPath.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new Vector2(-1f, 1f); // Đổi X thành âm
        }
    }
    void TakeDamage(int damage)
    {
        curentHealth -= damage;
        if (curentHealth <= 0)
        {
            curentHealth = 0;
            Die();
        }
    }
    void Die()
    {
        Debug.Log(" be attacked");
    }
}
