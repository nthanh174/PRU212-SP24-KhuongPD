using Pathfinding;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private AIPath aiPath;
    public Animator animator;
    public int maxHealth = 100;
    private int currentHealth;

    public GameObject goldPrefab;
    public float goldDropChance = 0.7f;

    public Transform player;
    public float attackDistance = 10f; // Khoảng cách để tấn công người chơi
    private bool isAttacking = false; // Biến để kiểm tra xem quái vật có đang tấn công không

    void Start()
    {
        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (aiPath != null)
        {
            ChangeDirection();
            if (!isAttacking) // Kiểm tra xem quái vật có đang tấn công không
            {
                CheckForPlayer();
            }
        }
    }

    void ChangeDirection()
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

    void EnemyRun()
    {
        float velocityMagnitude = aiPath.desiredVelocity.magnitude;
        Debug.Log("Enemy Speed: " + velocityMagnitude);
        animator.SetFloat("Speed", velocityMagnitude);
    }

    void CheckForPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position); // Tính khoảng cách đến người chơi

        if (distanceToPlayer <= attackDistance)
        {
            // Nếu khoảng cách nhỏ hơn hoặc bằng khoảng cách tấn công, quái vật sẽ tấn công
            AttackPlayer();
        }
        else
        {
            // Nếu khoảng cách lớn hơn khoảng cách tấn công, quái vật sẽ di chuyển đến người chơi
            EnemyRun();
        }
    }
    void AttackPlayer()
    {
        // Thực hiện hành động tấn công
        Debug.Log("Enemy attacks player!");
        animator.SetTrigger("Attack"); // Kích hoạt trigger trong animator để chạy animation tấn công
        isAttacking = true; // Đặt trạng thái tấn công
    }

    public void EndAttackAnimation()
    {
        isAttacking = false; // Kết thúc animation tấn công, đặt trạng thái tấn công lại là false
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


    void Die()
    {
        // Thực hiện hành động khi quái vật chết
        Debug.Log("Enemy dies!");
        animator.SetBool("isDie", true);
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
        DropGold();
    }

    void DropGold()
    {
        // Tạo vàng tại vị trí hiện tại của quái vật
        Instantiate(goldPrefab, transform.position, Quaternion.identity);
    }
}
