using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    private Path path;
    private Coroutine coroutine;
    public Seeker seeker;
    public DetectionZone detectionZone;

    public Animator animator;

    //di chuyển đến người chơi
    private bool reachDestination = false;
    public bool roaming = true; // di chuyển quanh người chơi
    public bool updateContinuesPath = false; // cập nhật đường đi liên tục
    public float moveSpeed = 3f; // tốc độ của quái
    public float nextWPDistance = 0.3f;
    public float maxDistanceToPlayer = 15f; // Khoảng cách tối đa giữa EnemyAI và Player để bắt đầu di chuyển về phía Player

    public float distanceToPlayer = 50f;// di chuyển cách người chơi 1 khoảng X
    public float desiredHeight = 50f; // di chuyển cách người chơi 1 khoảng Y

    //attack
    public bool isAttack = false;
    public bool attackFar = false;
    public int enemyDamage = 20;
    public float attackRange = 15f;
    public GameObject flyAttack;
    public float attackSpeed = 9f;
    public float timeBtwAttack = 2f;
    public float attackCooldown;

    //health
    public int maxHealth = 100;
    private int currentHealth;
    //gold
    public GameObject goldPrefab;
    public float goldDropChance = 0.7f;


    void Start()
    {
        InvokeRepeating("CalculatePath", 0f, 0.5f);
        currentHealth = maxHealth;
    }

    void Update()
    {
        attackCooldown -= Time.deltaTime;
        if(attackCooldown < 0f)
        {
            attackCooldown = timeBtwAttack;
            if (IsPlayerInRange(attackRange) && attackFar)
            {
                EnemyAttack();
            } else if (!attackFar)
            {
                EnemyAttack();
            }
        }
        ChangeDirection();
        EnemyRun();
    }
    //điều khuyển quái
    void ChangeDirection()
    {
        Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
        Vector2 directionToPlayer = ((Vector2)playerPos - (Vector2)transform.position).normalized;

        if (directionToPlayer.x >= 0.01f)
        {
            transform.localScale = new Vector2(1f, 1f);
        }
        else if (directionToPlayer.x <= -0.01f)
        {
            transform.localScale = new Vector2(-1f, 1f);
        }
    }

    void EnemyRun()
    {
        // Thực hiện animation run khi đang di chuyển và chưa đến điểm đích
        animator.SetBool("isRunning", path != null && path.vectorPath.Count > 0 && !reachDestination);
    }



    // Tấn công người chơi
    bool IsPlayerInRange(float range)
    {
        Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, playerPos);
        return distanceToPlayer <= range;
    }

    void EnemyAttack()
    {
        if (attackFar)
        {
            var attackTmp = Instantiate(flyAttack, transform.position, Quaternion.identity);

            attackTmp.GetComponent<EnemyBullet>().SetDamageToPlayer(enemyDamage);

            Rigidbody2D rb = attackTmp.GetComponent<Rigidbody2D>();
            Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
            Vector2 direction = (playerPos - transform.position).normalized;
            rb.AddForce(direction * attackSpeed, ForceMode2D.Impulse);
            animator.SetTrigger("Attack");
            // Dừng di chuyển của quái vật
            StopCoroutine(coroutine);
        }
        else
        {
            // Kiểm tra nếu người chơi nằm trong vùng detectionZone
            if (detectionZone.detecedCollider.Contains(FindObjectOfType<PlayerController>().GetComponent<Collider2D>()))
            {
                // Kích hoạt animation tấn công
                animator.SetTrigger("Attack");
                // Gây sát thương cho người chơi
                FindObjectOfType<PlayerController>().TakeDamage(enemyDamage);
                // Dừng di chuyển của quái vật
                StopCoroutine(coroutine);
            }
        }
    }


    // nhận damge
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

    IEnumerator DieCoroutine()
    {
        // Đặt trạng thái của animator thành "isDeath" là true
        animator.SetBool("isDeath", true);

        // Chờ đợi cho đến khi animation kết thúc
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Thực hiện hành động sau khi animation kết thúc
        DropGold();

        // Xóa đối tượng
        Destroy(gameObject);
    }

    void Die()
    {
        // Bắt đầu coroutine để xử lý việc chờ animation kết thúc
        StartCoroutine(DieCoroutine());
    }

    void DropGold()
    {
        // Tạo vàng tại vị trí hiện tại của quái vật
        Instantiate(goldPrefab, transform.position, Quaternion.identity);
    }
    //

    // Tìm đường đến người chơi

    void CalculatePath()
    {
        Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, playerPos); // Tính khoảng cách đến Player

        if (distanceToPlayer <= maxDistanceToPlayer) // Kiểm tra nếu khoảng cách nhỏ hơn hoặc bằng khoảng cách tối đa
        {
            Vector2 target = FindTarget();
            if (seeker.IsDone() && (updateContinuesPath || reachDestination))
            {
                seeker.StartPath(transform.position, target, OnPathComplete);
            }
        }
    }

    Vector2 FindTarget()
    {
        Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
        if (roaming)
        {
            // Tính toán hướng từ quái vật đến người chơi
            Vector2 directionToPlayer = ((Vector2)playerPos - (Vector2)transform.position).normalized;

            // Tính toán vị trí mục tiêu dựa trên hướng và khoảng cách, cũng như độ cao mong muốn
            Vector2 target = (Vector2)transform.position + directionToPlayer * distanceToPlayer;
            target += Vector2.up * desiredHeight;

            return target;
        }
        else
        {
            return (Vector2)playerPos;
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            MoveToTarget();
        }
    }

    void MoveToTarget()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(MoveToTargetCoroutine());
    }

    IEnumerator MoveToTargetCoroutine()
    {
        int currentWayPoint = 0;
        reachDestination = false;
        while (currentWayPoint < path.vectorPath.Count)
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - (Vector2)transform.position).normalized;
            Vector2 force = direction * moveSpeed * Time.deltaTime;
            transform.position += (Vector3)force;

            float distance = Vector2.Distance(transform.position, path.vectorPath[currentWayPoint]);
            if (distance < nextWPDistance)
            {
                currentWayPoint++;
            }
            yield return null;
        }
        reachDestination = true;
    }

    //
}
