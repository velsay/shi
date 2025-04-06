using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class IsoCharacterController2D : MonoBehaviour
{
    public static IsoCharacterController2D instance;

    public Animator anime;

    [Header("Hareket Ayarları")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;

    [Header("Kılıç Ayarları")]
    public GameObject sword;
    public float slashDownTime = 0.1f;
    public float slashSwingTime = 0.1f;
    public float returnTime = 0.2f;
    public float downwardOffset = 0.2f;
    public float slashDistance = 0.5f;
    public float tiltAngle = 50f;

    [Header("Can Sistemi")]
    private int maxHealth = 100;
    private int currentHealth;

    private Rigidbody2D rb;
    private Vector2 inputDirection;
    private float currentSpeed;

    private Vector3 originalSwordPos;
    private Quaternion originalSwordRot;
    private bool isSlashing = false;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        sword.transform.SetParent(transform);
        originalSwordPos = sword.transform.localPosition;
        originalSwordRot = sword.transform.localRotation;

        currentHealth = maxHealth;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 rawInput = new Vector2(h, v).normalized;
        inputDirection = rawInput;

        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        UpdateAnimatorDirection();

        if (inputDirection.x > 0)
            spriteRenderer.flipX = false;
        else if (inputDirection.x < 0)
            spriteRenderer.flipX = true;

        Vector3 swordScale = sword.transform.localScale;
        swordScale.x = spriteRenderer.flipX ? -Mathf.Abs(swordScale.x) : Mathf.Abs(swordScale.x);
        sword.transform.localScale = swordScale;

        if (Input.GetMouseButtonDown(0) && !isSlashing)
        {
            StartCoroutine(SwordSlash());
        }

        Debug.DrawRay(transform.position, inputDirection, Color.green);
    }

    void FixedUpdate()
    {
        if (inputDirection != Vector2.zero)
        {
            rb.MovePosition(rb.position + inputDirection * currentSpeed * Time.fixedDeltaTime);
        }
    }

    void UpdateAnimatorDirection()
    {
        // Yürüyorsa true
        anime.SetBool("IsWalking", inputDirection != Vector2.zero);

        // Yön sadece tuş basıldığında güncellenir
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            anime.SetBool("isgoingup", true);
            anime.SetBool("isgoingdown", false);
            anime.SetBool("IsMovingSide", false);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            anime.SetBool("isgoingup", false);
            anime.SetBool("isgoingdown", true);
            anime.SetBool("IsMovingSide", false);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ||
                 Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            anime.SetBool("isgoingup", false);
            anime.SetBool("isgoingdown", false);
            anime.SetBool("IsMovingSide", true);
        }
    }

    IEnumerator SwordSlash()
    {
        isSlashing = true;
        float elapsed = 0f;

        Vector3 downPosition = originalSwordPos + new Vector3(0f, -downwardOffset, 0f);
        Quaternion downRotation = Quaternion.Euler(originalSwordRot.eulerAngles + new Vector3(0f, 0f, -tiltAngle));

        while (elapsed < slashDownTime)
        {
            float t = elapsed / slashDownTime;
            sword.transform.localPosition = Vector3.Lerp(originalSwordPos, downPosition, t);
            sword.transform.localRotation = Quaternion.Lerp(originalSwordRot, downRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sword.transform.localPosition = downPosition;
        sword.transform.localRotation = downRotation;

        elapsed = 0f;

        float direction = spriteRenderer.flipX ? -1f : 1f;
        Vector3 backPosition = originalSwordPos + new Vector3(direction * slashDistance, 0f, 0f);
        Quaternion finalRotation = originalSwordRot;

        while (elapsed < slashSwingTime)
        {
            float t = elapsed / slashSwingTime;
            sword.transform.localPosition = Vector3.Lerp(downPosition, backPosition, t);
            sword.transform.localRotation = Quaternion.Lerp(downRotation, finalRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sword.transform.localPosition = backPosition;
        sword.transform.localRotation = finalRotation;

        DamageEnemiesInRange();

        elapsed = 0f;

        while (elapsed < returnTime)
        {
            float t = elapsed / returnTime;
            sword.transform.localPosition = Vector3.Lerp(backPosition, originalSwordPos, t);
            sword.transform.localRotation = Quaternion.Lerp(finalRotation, originalSwordRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sword.transform.localPosition = originalSwordPos;
        sword.transform.localRotation = originalSwordRot;

        isSlashing = false;
    }

    void DamageEnemiesInRange()
    {
        float radius = 0.8f;
        Vector2 center = sword.transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(20);
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(10);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (currentHealth <= 0)
        {
            PlayerRespawnManager.Instance.RespawnPlayer();
        }

        // Opsiyonel: hasar efekti vs.
    }

    public void RestoreHealth()
    {
        currentHealth = maxHealth;
    }
}
