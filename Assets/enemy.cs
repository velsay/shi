using Unity.VisualScripting;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public Transform player;
       // Oyuncu nesnesinin Transform'u
    public float moveSpeed = 2f;   // Düşmanın hareket hızı
    public float followDistance = 8f;  // Takip mesafesi
    public float smoothTime = 0.3f;  // Hareketin yumuşaklığı

    private Vector3 velocity = Vector3.zero;
    void Start()
{
    if (player == null)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }
}

    void Update()
    {
        // Oyuncu ile düşman arasındaki mesafeyi kontrol et
        float distance = Vector3.Distance(transform.position, player.position);

        // Eğer oyuncu belirli bir mesafeye gelirse takip başlasın
        if (distance <= followDistance)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        // Oyuncu ile aynı X ve Y pozisyonuna gelmesini sağla (Z pozisyonunu korur)
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, moveSpeed);
    }

    // Düşman oyuncuya çarptığında hasar vermek için
    private void OnCollisionStay2D(Collision2D collision)

    {
        
    }
  void OnTriggerStay2D(Collider2D collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10);
        }
    }
}

}

