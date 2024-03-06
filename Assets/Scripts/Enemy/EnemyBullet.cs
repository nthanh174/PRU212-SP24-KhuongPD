using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private int damageToPlayer = 5;

    // Phương thức để thiết lập giá trị damageToPlayer
    public void SetDamageToPlayer(int damage)
    {
        damageToPlayer = damage;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(damageToPlayer);
                Destroy(gameObject);
            }
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
