using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowWeapon : MonoBehaviour
{
    private int damageToEnemy = 5;
    public void SetDamageToEnemy(int damage)
    {
        damageToEnemy = damage;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyAI enemyAI = collision.gameObject.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(damageToEnemy);
                Destroy(gameObject);
            }
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
