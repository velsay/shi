using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public Transform player;        // Oyuncu Transform'u
    public float moveSpeed = 3f;    // Boss'un hareket hızı
    public float attackDistance = 2f; // Saldırı mesafesi
    public float health = 500f;     // Boss'un sağlığı
    public float attackDamage = 50f; // Saldırı hasarı
    public GameObject reaper;      // Boss'un elindeki Reaper (şeytanın orak)

    private Animator animator;     // Boss'un Animator'u
    private bool isAttacking = false; // Boss saldırı yapıyor mu?

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (health <= 0)
        {
            Die();
            return;
        }

        MoveTowardPlayer();
        AttackPlayer();
    }

    void MoveTowardPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Eğer oyuncu belirli bir mesafedeyse, boss hareket etmeye başlar
        if (distance > attackDistance && !isAttacking)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            animator.SetFloat("MoveSpeed", 1f); // Yürüyüş animasyonunu başlat
        }
        else
        {
            animator.SetFloat("MoveSpeed", 0f); // Yürümeyi durdur
        }
    }

    void AttackPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackDistance && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Orak animasyonunu başlat
        animator.SetTrigger("Attack");

        // Reaper (şeytanın orak) animasyonunu başlat
        reaper.GetComponent<Animator>().SetTrigger("Attack");

        // Saldırı animasyonunu bitir
        yield return new WaitForSeconds(1f); // Orak animasyonunun süresi kadar bekle

        // Eğer oyuncu yakında ise, ona zarar ver
        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }

        isAttacking = false;
    }

    void Die()
    {
        animator.SetTrigger("Die");
        // Boss öldü
        Debug.Log("Boss defeated!");
        Destroy(gameObject, 2f); // 2 saniye sonra boss'u yok et
    }
}
