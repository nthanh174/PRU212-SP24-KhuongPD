using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 10;
    public float jumpForce = 10;
    private Rigidbody2D rb;
    public Animator animator;


    private int CurrentWeaponNo = 0;
    private float[] weaponLayerWeights;

    public GameObject bowWeapon;
    public float attackSpeed = 10f;
    public float attackedRate = 2f;
    public float nextAttackTime = 0f;
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;

/*    public int bow = 1/10;
    public int kinght = 1/5;*/

    public int defaulDamage = 20;
    public int maxDamage = 20;
    public int defaulHealth = 100;
    public int maxHealth = 100;
    public int curentHealth;
    public BarUI barUI;


    private Vector2 newCheckPoint;
    public GameOverUI gameOverUI;
    public UpdateUI updateUI;
    public DetectionZone zone;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        weaponLayerWeights = new float[animator.layerCount];
        curentHealth = maxHealth;
        barUI.UpdateHealthBar(curentHealth, maxHealth);
        barUI.UpdateCoinhBar(0);

        gameOverUI.screenGameOver.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.G)) {
            TakeDamage(maxDamage);
        }

        ChangeWeapon();
        Attack();
        SetUpdateUI();
        Teleport();
    }

    void ChangeDirection(float move)
    {
        if (move > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else if (move < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }
    void ChangeWeapon()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentWeaponNo = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentWeaponNo = 2;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            CurrentWeaponNo = 0;
        }

        for (int i = 0; i < weaponLayerWeights.Length; i++)
        {
            if (i == CurrentWeaponNo)
            {
                weaponLayerWeights[i] = 1;
            }
            else
            {
                weaponLayerWeights[i] = 0;
            }
            animator.SetLayerWeight(i, weaponLayerWeights[i]);
        }
    }
    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0) && CurrentWeaponNo != 0)
            {
                animator.SetTrigger("Attack1");
                PerformAttack(1, CurrentWeaponNo);
            }
            else if (Input.GetMouseButtonDown(1) && CurrentWeaponNo != 0)
            {
                animator.SetTrigger("Attack2");
                PerformAttack(2, CurrentWeaponNo);
            }
        }
    }
    void PerformAttack(int attackType, int weapoNo)
    {
        if (weapoNo == 2)
        {
            var attackTmp = Instantiate(bowWeapon, transform.position, Quaternion.identity);

            attackTmp.GetComponent<BowWeapon>().SetDamageToEnemy(maxDamage);

            Rigidbody2D rb = attackTmp.GetComponent<Rigidbody2D>();

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 direction = (mousePosition - transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            attackTmp.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            rb.AddForce(direction.normalized * attackSpeed, ForceMode2D.Impulse);
            animator.SetTrigger("Attack1");

            nextAttackTime = Time.time + 2f / attackedRate;
        }
        else
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            foreach (Collider2D hit in hitEnemies)
            {
                EnemyAI enemyAI = hit.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    if(attackType == 1)
                    {
                        enemyAI.TakeDamage(maxDamage + maxDamage / 5);
                    }
                    else
                    {
                        enemyAI.TakeDamage(maxDamage + maxDamage / 2);
                    }
                }
            }
            nextAttackTime = Time.time + (attackType == 1 ? 1f : 3f) / attackedRate;
        }
    }
    public void TakeDamage(int damage)
    {
        curentHealth -= damage;
        if (curentHealth <= 0)
        {
            curentHealth = 0;
            gameOverUI.GameOver();
        }
        barUI.UpdateHealthBar(curentHealth, maxHealth);
    }

    public void UpdateCheckPoint(Vector2 checkPoint)
    {
        if(newCheckPoint != checkPoint)
        {
            newCheckPoint = checkPoint;
            Debug.Log("Check Point: " + newCheckPoint);
        }

    }
    public void ReturnToCheckPoint()
    {
        if (newCheckPoint != null)
        {
            transform.position = newCheckPoint;

            curentHealth = maxHealth;
            barUI.UpdateHealthBar(curentHealth, maxHealth);

            GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemyObject in enemyObjects)
            {
                EnemyAI enemyAIComponent = enemyObject.GetComponent<EnemyAI>();
                if (enemyAIComponent != null)
                {
                    enemyAIComponent.SetHealth(enemyAIComponent.maxHealth);
                    enemyAIComponent.ReturnToPosSpam();
                    StopCoroutine(enemyAIComponent.coroutine);
                }
            }
        }
        else
        {
            Debug.LogWarning("Không có điểm kiểm tra nào được lưu.");
        }
    }

    public void SetUpdateUI() {
        if (zone.detecedCollider.Contains(GameObject.FindGameObjectWithTag("npc2").GetComponent<Collider2D>()))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Time.timeScale = 0f;
                updateUI.isActive(true);
            }
        }
        if (zone.detecedCollider.Contains(GameObject.FindGameObjectWithTag("npc1").GetComponent<Collider2D>()))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Time.timeScale = 0f;
                updateUI.isActive(true);
            }
        }
    }

    public void Teleport()
    {
        if (zone.detecedCollider.Contains(GameObject.FindGameObjectWithTag("Gate1").GetComponent<Collider2D>()))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                transform.position = GameObject.FindGameObjectWithTag("Gate2").transform.position;
            }
        }
        else if (zone.detecedCollider.Contains(GameObject.FindGameObjectWithTag("Gate2").GetComponent<Collider2D>()))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                transform.position = GameObject.FindGameObjectWithTag("Gate1").transform.position;
            }
        }
        else if (zone.detecedCollider.Contains(GameObject.FindGameObjectWithTag("Gate3").GetComponent<Collider2D>()))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                transform.position = GameObject.FindGameObjectWithTag("Gate4").transform.position;
            }
        }
        else if (zone.detecedCollider.Contains(GameObject.FindGameObjectWithTag("Gate4").GetComponent<Collider2D>()))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                transform.position = GameObject.FindGameObjectWithTag("Gate3").transform.position;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint is null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Gold"))
        {
            Destroy(collision.gameObject);
            Debug.Log("Đã ăn vàng");
            barUI.UpdateCoinhBar(10);
        }
    }

}

