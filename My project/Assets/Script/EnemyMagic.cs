using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMagic : MonoBehaviour
{
    [Header("Stats")]
    public int health = 20;
    public float attackRange = 10f;
    public float attackCooldown = 2f;
    private bool isDead = false;

    [Header("Attack")]
    public GameObject fireballPrefab;     // 🔥 ลูกไฟ
    public Transform shootPoint;          // ตำแหน่งยิง
    public float fireballSpeed = 6f;

    private float cooldownTimer = 0f;
    private bool isCasting = false;

    private Animator anim;
    private Transform player;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip castSound;
    public AudioClip chargeSound;
    public AudioClip hitSound;
    public AudioClip deathSound;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (isDead || player == null) return;

        cooldownTimer -= Time.deltaTime;
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            // หันหน้าเข้าหาผู้เล่น
            if (player.position.x < transform.position.x)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);

            // ร่ายเวทเมื่อคูลดาวน์หมด
            if (cooldownTimer <= 0f && !isCasting)
            {
                StartCoroutine(CastSpell());
            }
        }
    }

    private IEnumerator CastSpell()
    {
        isCasting = true;
        anim.SetTrigger("Cast");
        

   

        cooldownTimer = attackCooldown;
        yield return new WaitForSeconds(attackCooldown);
        isCasting = false;
    }

    // เรียกตอนปล่อยลูกไฟ (จาก Animation Event ก็ได้)
    private void ChargeSound()
    {
        if (audioSource && chargeSound)
            audioSource.PlayOneShot(chargeSound);
    }
    private void ShootFireball()
    {
        if (fireballPrefab == null) return;

        float direction = transform.localScale.x > 0 ? 1f : -1f;
        Quaternion rotation = Quaternion.identity;
        GameObject fireball = Instantiate(fireballPrefab, shootPoint.position, rotation);

        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(fireballSpeed * direction, 0f);

        // พลิกลูกไฟตามทิศทาง
        Vector3 scale = fireball.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        fireball.transform.localScale = scale;

        // เล่นเสียง
        if (audioSource && castSound)
            audioSource.PlayOneShot(castSound,0.5f);
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        health -= dmg;
        StartCoroutine(FlashRed());
        if (audioSource && hitSound)
            audioSource.PlayOneShot(hitSound);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        anim.SetTrigger("Dead");

        // ปิด Collider ทั้งหมด
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        if (audioSource && deathSound)
            audioSource.PlayOneShot(deathSound, 0.5f);

        Destroy(gameObject, 2f);
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
