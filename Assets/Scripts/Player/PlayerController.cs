using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 10; // Tốc độ chạy
    [SerializeField]
    private float jumpForce = 10; // Độ cao khi nhảy
    private bool isGrounded; // Kiểm tra khi có sự va chạm vào map
    private Rigidbody2D rb;
    [SerializeField]
    private Animator animator;

    private int CurrentWeaponNo = 0; // Theo dõi loại vũ khí hiện tại, mặc định là không cầm vũ khí
    private float[] weaponLayerWeights; // Mảng lưu trữ trọng số của các layer vũ khí

    [SerializeField]
    private float attackedRate = 2f;
    private float nextAttackTime = 0f;
    [SerializeField]
    private Transform attackPoint;
    [SerializeField]
    private float attackRange = 1f;
    [SerializeField]
    private LayerMask enemyLayers;

    [SerializeField]
    private int maxHealth = 100;
    private int curentHealth;
    [SerializeField]
    private BarController barController;

    [SerializeField]
    private GameObject screenGameOver;
    private bool isGameOver = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // Khởi tạo mảng weaponLayerWeights với kích thước bằng với số lượng layer trong Animator
        weaponLayerWeights = new float[animator.layerCount];
        curentHealth = maxHealth;
        barController.UpdateHealthBar(curentHealth, maxHealth);
        barController.UpdateCoinhBar(0);
        screenGameOver.SetActive(false);
    }

    void Update()
    {
        // test trừ máu
        if (Input.GetKeyUp(KeyCode.G)) {
            TakeDamage(20);
        }

        Move();
        Jump();
        /*        Rolling();*/
        ChangeWeapon();
        Attack();
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
        // Kiểm tra người chơi nhấn các phím tương ứng với vũ khí
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentWeaponNo = 1; // Chuyển sang vũ khí 1
        }
        else if (Input.GetKeyDown(KeyCode.C)) // Sử dụng phím "C" để chuyển đổi vũ khí
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
                PerformAttack(1);
            }
            else if (Input.GetMouseButtonDown(1) && CurrentWeaponNo != 0)
            {
                animator.SetTrigger("Attack2");
                PerformAttack(2);
            }
        }
    }
    void PerformAttack(int attackType)
    {
        nextAttackTime = Time.time + (attackType == 1 ? 1f : 3f) / attackedRate;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D hit in hitEnemies)
        {
            EnemyAI enemyAI = hit.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                // Gọi phương thức TakeDamage() của đối tượng enemy
                enemyAI.TakeDamage(20); // Truyền vào lượng sát thương cần gây ra
            }
        }
    }
    public void TakeDamage(int damage)
    {
        curentHealth -= damage;
        if (curentHealth <= 0 && !isGameOver)
        {
            curentHealth = 0;
            GameOver();
        }
        barController.UpdateHealthBar(curentHealth, maxHealth);
    }
    void GameOver()
    {
        // Dừng tất cả hoạt động trong game
        Time.timeScale = 0f;

        // Hiện panel game over
        screenGameOver.SetActive(true);

        // Đặt trạng thái game over
        isGameOver = true;
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
            barController.UpdateCoinhBar(10);
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

