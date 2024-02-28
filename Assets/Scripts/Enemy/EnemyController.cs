using Pathfinding;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private AIPath aiPath;
    [SerializeField] private Animator animator;
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private float goldDropChance = 0.7f;

    private void Start()
    {
        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        ChangeDirection();
        EnemyRun();
    }

    private void ChangeDirection()
    {
        if (aiPath != null)
        {
            if (aiPath.desiredVelocity.x >= 0.01f)
            {
                transform.localScale = new Vector2(1f, 1f);
            }
            else if (aiPath.desiredVelocity.x <= -0.01f)
            {
                transform.localScale = new Vector2(-1f, 1f);
            }
        }
    }

    private void EnemyRun()
    {
        if (aiPath != null)
        {
            float velocityMagnitude = aiPath.velocity.magnitude;
            if (velocityMagnitude > 0.1f)
            {
                animator.SetBool("isRunning", true);
            }
            else
            {
                animator.SetBool("isRunning", false);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Enemy takes damage: " + damage);
        currentHealth -= damage;
        Debug.Log("Enemy Health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        animator.SetBool("isDie", true);
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length); // Hủy GameObject sau khi hoàn thành animation "Die"
        DropGold();
    }

    private void DropGold()
    {
        // Tạo vàng tại vị trí hiện tại của quái vật
        Instantiate(goldPrefab, transform.position, Quaternion.identity);
    }
}
