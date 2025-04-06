using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UIElements;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class IsoCharacterController2D : MonoBehaviour
{
    public static IsoCharacterController2D instance;
    public UnityEngine.UI.Image canbar;
    public Animator anime;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;

    [Header("Sword Settings")]
    public GameObject sword;
    public GameObject hitboxPrefab;
    public float slashDownTime = 0.1f;
    public float slashSwingTime = 0.1f;
    public float returnTime = 0.2f;
    public float downwardOffset = 0.2f;
    public float slashDistance = 0.5f;
    public float tiltAngle = 50f;
    public float hitboxOffsetDistance = 0.5f;

    [Header("Health System")]
    private int maxHealth = 100;
    private int currentHealth;

    private Rigidbody2D rb;
    private Vector2 inputDirection;
    private float currentSpeed;

    private Vector3 originalSwordPos;
    private Quaternion originalSwordRot;
    private bool isSlashing = false;
    private bool canSlash = true;

    private SpriteRenderer spriteRenderer;

    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    // Add a new variable to track facing direction
    private Vector2 facingDirection = Vector2.right; // Default facing right (can be adjusted later)

    // Track respawn point
    private Vector3 respawnPoint;

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

        // Set initial respawn point to the player's starting position
        respawnPoint = transform.position;
    }

    void Update()
    {
        // Get input for movement
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 rawInput = new Vector2(h, v).normalized;
        inputDirection = rawInput;

        // Track the facing direction based on input
        if (inputDirection != Vector2.zero)
        {
            facingDirection = inputDirection;  // Update facing direction when the player moves
        }

        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        UpdateAnimatorDirection();

        if (inputDirection.x > 0)
            spriteRenderer.flipX = false;
        else if (inputDirection.x < 0)
            spriteRenderer.flipX = true;

        Vector3 swordScale = sword.transform.localScale;
        swordScale.x = spriteRenderer.flipX ? -Mathf.Abs(swordScale.x) : Mathf.Abs(swordScale.x);
        sword.transform.localScale = swordScale;

        if (Input.GetMouseButtonDown(0) && canSlash)
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
        anime.SetBool("IsWalking", inputDirection != Vector2.zero);

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
        anime.SetTrigger("Attack");
        canSlash = false;
        isSlashing = true;

        // Use the facing direction for the attack
        Vector2 attackDirection = facingDirection != Vector2.zero ? facingDirection : Vector2.right;

        // Calculate the spawn position for the hitbox
        Vector2 spawnPosition = (Vector2)transform.position + attackDirection.normalized * hitboxOffsetDistance;

        // Instantiate the hitbox and set its position
        GameObject hitbox = Instantiate(hitboxPrefab, spawnPosition, Quaternion.identity);

        // Set the hitbox as a child of the character's transform
        hitbox.transform.SetParent(transform);

        // Make the hitbox face the right direction
        hitbox.transform.localScale = new Vector3(
            attackDirection.x > 0 ? Mathf.Abs(hitbox.transform.localScale.x) : -Mathf.Abs(hitbox.transform.localScale.x),
            hitbox.transform.localScale.y,
            hitbox.transform.localScale.z
        );

        // Optional: Reset hitbox local position and rotation to match the parent if needed
        hitbox.transform.localPosition = spawnPosition - (Vector2)transform.position;
        hitbox.transform.localRotation = Quaternion.identity;

        hitEnemies.Clear();

        float hitboxDuration = 0.2f;
        float timer = 0f;

        while (timer < hitboxDuration)
        {
            DamageEnemiesInRange(hitbox);
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(hitbox);

        isSlashing = false;
        yield return new WaitForSeconds(slashSwingTime);
        canSlash = true;
    }

    void DamageEnemiesInRange(GameObject hitbox)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitbox.transform.position, 0.8f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy") && !hitEnemies.Contains(hit))
            {
                EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(20);
                    hitEnemies.Add(hit);
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
    }

    public void RestoreHealth()
    {
        currentHealth = maxHealth;
    }

    // Update the respawn point when the player hits a checkpoint
    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }

    // Respawn the player at the last checkpoint
    public void RespawnPlayer()
    {
        transform.position = respawnPoint;
        currentHealth = maxHealth;
        canbar.fillAmount = 0.99f;
    }
}
