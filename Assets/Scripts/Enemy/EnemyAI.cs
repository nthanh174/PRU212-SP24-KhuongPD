using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    private Path path;
    public Coroutine coroutine;
    public Seeker seeker;
    public DetectionZone detectionZone;

    public Animator animator;

    //di chuyển đến người chơi
    private bool reachDestination = false;
    public bool roaming = true;
    public bool updateContinuesPath = false;
    public float moveSpeed = 3f;
    public float nextWPDistance = 0.3f;
    public float maxDistanceToPlayer = 15f;
    public float maxDistancePos = 20f;

    private bool isReturning = false;

    public float widthToPlayer = 15f;
    public float heightToPlayer = 10f;

    //attack
    public bool isAttack = false;
    public bool attackFar = false;
    public int enemyDamage = 20;
    public float attackRange = 15f;
    public GameObject flyAttack;
    public float attackSpeed = 9f;
    public float timeBtwAttack = 2f;
    public float attackCooldown;
    private bool isAttackByPlayer;

    //health
    public TextMeshPro txtHealth;
    public int maxHealth = 100;
    public int currentHealth;
    //gold
    public GameObject goldPrefab;
    public float goldDropChance = 0.7f;

    void Start()
    {
        InvokeRepeating("CalculatePath", 0f, 0.5f);
        currentHealth = maxHealth;
        SetHealth(currentHealth);
    }

    void Update()
    {
        attackCooldown -= Time.deltaTime;
        if(attackCooldown < 0f)
        {
            attackCooldown = timeBtwAttack;
            if (IsPlayerInRange(attackRange) && isAttack)
            {
                EnemyAttack();
            }
        }
        ChangeDirection();
        EnemyRun();
    }
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
        foreach (Transform child in transform)
        {
            child.localScale = new Vector2(1 / transform.localScale.x, 1);
        }
    }

    void EnemyRun()
    {
        animator.SetBool("isRunning", path != null && path.vectorPath.Count > 0 && !reachDestination);
    }

    bool IsPlayerInRange(float range)
    {
        Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, playerPos);
        return distanceToPlayer <= range;
    }

    void EnemyAttack()
    {
        if (attackFar && !isReturning)
        {
            var attackTmp = Instantiate(flyAttack, transform.position, Quaternion.identity);

            attackTmp.GetComponent<EnemyBullet>().SetDamageToPlayer(enemyDamage);

            Rigidbody2D rb = attackTmp.GetComponent<Rigidbody2D>();
            Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
            Vector2 direction = (playerPos - transform.position).normalized;
            rb.AddForce(direction * attackSpeed, ForceMode2D.Impulse);
            animator.SetTrigger("Attack");
            StopCoroutine(coroutine);
        }
        else
        {
            if (detectionZone.detecedCollider.Contains(FindObjectOfType<PlayerController>().GetComponent<Collider2D>()))
            {
                animator.SetTrigger("Attack");
                FindObjectOfType<PlayerController>().TakeDamage(enemyDamage);
                StopCoroutine(coroutine);
            }
        }
    }

    public void SetHealth(int health)
    {
        currentHealth = health;
        txtHealth.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }

    public void ReturnToPosSpam()
    {
        transform.position = transform.parent.position;
    }

    public void TakeDamage(int damage)
    {
        if (!isReturning)
        {
            currentHealth -= damage;
        }
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        if (currentHealth/maxHealth < 1)
        {
            isAttackByPlayer = true;
        }
        SetHealth(currentHealth);
    }

    IEnumerator DieCoroutine()
    {
        animator.SetBool("isDeath", true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        DropGold();
        Destroy(gameObject);
    }

    void Die()
    {
        StartCoroutine(DieCoroutine());
    }

    void DropGold()
    {
        Instantiate(goldPrefab, transform.position, Quaternion.identity);
    }

    void CalculatePath()
    {
        Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, playerPos);

        Vector2 target = Vector2.zero;

        if (isAttackByPlayer && !isReturning)
        {
            target = FindTarget();
            if (distanceToPlayer <= maxDistanceToPlayer)
            {
                isAttackByPlayer = false;
            }

        }else if (distanceToPlayer <= maxDistanceToPlayer && !isReturning)
        {
            target = FindTarget();
        }else if (distanceToPlayer > maxDistanceToPlayer && !isReturning)
        {
            target = (Vector2)transform.parent.position;
            SetHealth(maxHealth);
        }

        if (isReturning)
        {
            target = (Vector2)transform.parent.position;
            SetHealth(maxHealth);
        }

        float distanceToPos = Vector3.Distance(transform.position, transform.parent.position);
        if (distanceToPos >= maxDistancePos)
        {
            target = (Vector2)transform.parent.position;
            isAttackByPlayer = false;
            isReturning = true;
        }

        if (distanceToPos <= 2f)
        {
            isReturning = false;
        }

        if (seeker.IsDone() && (updateContinuesPath || reachDestination))
        {
            seeker.StartPath(transform.position, target, OnPathComplete);
        }
    }

    Vector2 FindTarget()
    {
        Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
        if (roaming)
        {
            Vector2 directionToPlayer = ((Vector2)playerPos - (Vector2)transform.position).normalized;
            Vector2 target = (Vector2)transform.position + directionToPlayer * widthToPlayer;
            
            if(playerPos.y + heightToPlayer < target.y) {
                target.y = playerPos.y - heightToPlayer;
            } else
            {
                target.y = playerPos.y + heightToPlayer;
            }
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

}
