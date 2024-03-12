using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 10; // Tốc độ chạy
    public float jumpForce = 10; // Độ cao khi nhảy
    private bool isGrounded; // Kiểm tra khi có sự va chạm vào map
    private Rigidbody2D rb;
    public Animator animator;


    private int CurrentWeaponNo = 0; // Theo dõi loại vũ khí hiện tại, mặc định là không cầm vũ khí
    private float[] weaponLayerWeights; // Mảng lưu trữ trọng số của các layer vũ khí

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

        // Khởi tạo mảng weaponLayerWeights với kích thước bằng với số lượng layer trong Animator
        weaponLayerWeights = new float[animator.layerCount];
        curentHealth = maxHealth;
        barUI.UpdateHealthBar(curentHealth, maxHealth);
        barUI.UpdateCoinhBar(0);

        gameOverUI.screenGameOver.SetActive(false);
    }

    void Update()
    {
        // test trừ máu
        if (Input.GetKeyUp(KeyCode.G)) {
            TakeDamage(maxDamage);
        }

        Move();
        Jump();
        ChangeWeapon();
        Attack();
        SetUpdateUI();
    }

    void ChangeDirection(float move)
    {
        // Đổi hướng nhân vật
        if (move > 0)
        {
            transform.localScale = new Vector2(1, 1); // Đổi X thành dương
        }
        else if (move < 0)
        {
            transform.localScale = new Vector2(-1, 1); // Đổi X thành âm
        }
    }
    void Move()
    {
        float move = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(move, 0);
        if (isGrounded)
        {
            rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
            animator.SetFloat("Speed", Mathf.Abs(move));
            ChangeDirection(move);
        }
        else
        {
            // Ngừng animation chạy khi nhân vật không di chuyển trên mặt đất
            animator.SetFloat("Speed", 0);
        }

    }
    void Jump()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            animator.SetBool("isJumping", true);
            isGrounded = false; // Cập nhật isGrounded ngay khi nhảy
        }
        else
        {
            // Ngừng animation nhảy khi nhân vật không nhảy
            if (isGrounded)
            {
                animator.SetBool("isJumping", false);
            }
        }
    }
    void ChangeWeapon()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentWeaponNo = 1; // Chuyển sang vũ khí 1
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentWeaponNo = 2; // Chuyển sang vũ khí 2
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            CurrentWeaponNo = 0; // Mặc định không cầm vũ khí
        }

        // Thiết lập trọng số của các layer vũ khí trong mảng weaponLayerWeights
        for (int i = 0; i < weaponLayerWeights.Length; i++)
        {
            // Nếu index của layer trùng với CurrentWeaponNo, thiết lập trọng số là 1
            if (i == CurrentWeaponNo)
            {
                weaponLayerWeights[i] = 1;
            }
            // Ngược lại, thiết lập trọng số là 0
            else
            {
                weaponLayerWeights[i] = 0;
            }

            // Thiết lập trọng số của layer trong Animator bằng giá trị từ mảng weaponLayerWeights
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

            // Lấy vị trí của chuột trong không gian của game
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Tính toán hướng từ vị trí của đối tượng đến vị trí của chuột
            Vector2 direction = (mousePosition - transform.position).normalized;

            // Tính toán góc quay để xoay mũi tên
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Xoay mũi tên
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

    //Check Point Pos
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
            // Di chuyển người chơi đến vị trí của điểm kiểm tra mới nhất đã lưu
            transform.position = newCheckPoint;


            // Reset trạng thái hoặc thông số khác của người chơi nếu cần
            curentHealth = maxHealth;
            barUI.UpdateHealthBar(curentHealth, maxHealth);


            // Thiết lập sức khỏe của các đối tượng EnemyAI
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

    //Set Actice UpdateUI

    public void SetUpdateUI() {
        if (zone.detecedCollider.Contains(GameObject.FindGameObjectWithTag("npc2").GetComponent<Collider2D>()))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Time.timeScale = 0f;
                updateUI.isActive(true);
            }
        }
        if (zone.detecedCollider.Contains(GameObject.FindGameObjectWithTag("npc1").GetComponent<Collider2D>()))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Time.timeScale = 0f;
                updateUI.isActive(true);
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
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            // Chỉ kích hoạt animation nhảy khi nhân vật đang ở mặt đất và đang không thực hiện nhảy
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.Space))
            {
                animator.SetBool("isJumping", false);
            }
        }

        // xử lý khi nhân vật chạm vào vàng
        if (collision.gameObject.CompareTag("Gold"))
        {
            Destroy(collision.gameObject);
            Debug.Log("Đã ăn vàng");
            barUI.UpdateCoinhBar(10);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("isJumping", true); // Bật animation nhảy khi nhân vật rời khỏi mặt đất
        }
    }

}

