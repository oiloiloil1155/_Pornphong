using System.Collections;
using UnityEngine;

public class EnemyArcher : MonoBehaviour
{
    [Header("Stats")]
    public int health ;
    public float attackRange ;
    public float attackCooldown ;
    private bool isDead = false;

    [Header("Attack")]
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public float arrowSpeed ;

    private float cooldownTimer = 0f;
    private Animator anim;
    private Transform player;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip Enemyhit;
    public AudioClip Enemydead;
    private bool isShooting = false;
    
   

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
            // หันหน้าตามตำแหน่งผู้เล่น
            if (player.position.x < transform.position.x)
                transform.localScale = new Vector3(-1, 1, 1); // ซ้าย
            else
                transform.localScale = new Vector3(1, 1, 1);  // ขวา

            // ยิงถ้าคูลดาวน์หมด และยังไม่ได้ยิงอยู่
            if (cooldownTimer <= 0f && !isShooting)
            {
                StartCoroutine(ShootCoroutine());
            }
        }
    }

    // เรียกจาก Animation Event ตอนยิง
    public void ShootArrow()
    {
       
        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        Rigidbody2D rbArrow = arrow.GetComponent<Rigidbody2D>();

        // ใช้ localScale.x ของศัตรูเพื่อกำหนดทิศทาง
        float direction = transform.localScale.x > 0 ? 1f : -1f;

        // ให้ลูกธนูพุ่งไปทางที่ศัตรูหัน
        rbArrow.velocity = new Vector2(arrowSpeed * direction, 0f);

        // พลิกลูกธนูตามทิศ
        Vector3 arrowScale = arrow.transform.localScale;
        arrowScale.x = Mathf.Abs(arrowScale.x) * direction;
        arrow.transform.localScale = arrowScale;

        // เล่นเสียงยิงธนู
        if (audioSource != null && shootSound != null)
            audioSource.PlayOneShot(shootSound);
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        health -= dmg;
        if (health <= 0) gameObject.tag = "Untagged";
        // ✅ ปิด Collider ทั้งหมดใน Enemy
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }
        Die();

        StartCoroutine(FlashRed());
        audioSource.PlayOneShot(Enemyhit);
    }

    private void Die()
    {
        isDead = true;
        if (audioSource != null && Enemydead != null)
        {
            audioSource.PlayOneShot(Enemydead, 0.5f);
        }
        anim.SetTrigger("Dead");
        Destroy(gameObject, 2f);
    }
    private IEnumerator ShootCoroutine()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        yield return new WaitForSeconds(attackCooldown);

        isShooting = false; // พร้อมยิงอีกครั้ง
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