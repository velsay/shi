using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;     // Başlangıç sağlığı
    public Image healthBar;         // Image tipi sağlık barı (Fill Method: Filled)

    public void TakeDamage(float damage)
    {
        if (health <= 0) return;

        health -= damage;
        if (health < 0) health = 0;

        Debug.Log(health);

        UpdateHealthBar();

        if (health <= 0)
        {
            Die();
        }

        StartCoroutine(IFrames());
    }

    void UpdateHealthBar()
    {
        // 0 - 1 arası değer verir fillAmount için
        healthBar.fillAmount = health / 100f;
    }

    void Die()
    {
        Debug.Log("Player has died!");
        // Game Over işlemleri buraya
    }

    IEnumerator IFrames()
    {
        // Layer 9: Player, Layer 10: Enemy olmalı
        Physics2D.IgnoreLayerCollision(9, 10, true);
        yield return new WaitForSeconds(1f);
        Physics2D.IgnoreLayerCollision(9, 10, false);
    }
}
