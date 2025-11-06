using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class BossAssassin : MonoBehaviour
{
    [Header("Boss Stats")]
    [SerializeField] private Transform player;
    [SerializeField] private float agroRange = 10f;   // ระยะที่บอสเริ่มไล่
    [SerializeField] private float attackRange = 2f;  // ระยะโจมตี
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int health = 50;

    private bool isDead = false;
    private bool isAttacking = false;
    private bool isPhase2 = false;
    private bool canTeleport = true;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip enemyHit;
    [SerializeField] private AudioClip enemyDead;
    [SerializeField] private AudioClip Amytalk;
    [SerializeField] private AudioClip Fade;
    [SerializeField] private AudioClip AttackSfx;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public Text HP;
    public Slider sliderHP;
    public GameObject winPanel;
    public float delayBeforeWinUI = 3f; // เวลาให้เดินเล่นก่อนโชว์ UI

    public Myscript1 playerScript;

    [SerializeField] private float teleportCooldown = 5f;
    private float teleportTimer = 0f;


    private void Awake()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        sliderHP.maxValue = health;
        sliderHP.value = health;
    }

    private void Update()
    {

        HP.text = "HP: " + health;
        sliderHP.value = health;


        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= agroRange && distance > attackRange && !isAttacking)
        {
            ChasePlayer();
        }
        else if (distance <= attackRange && !isAttacking)
        {
            Attack();
        }
        else
        {
            anim.SetBool("Run", false);
            rb.velocity = Vector2.zero;
        }

        if (health <= 0)
        {
            Die();
        }
        if (health <= 15)
        {
            isPhase2 = true ;
        }


        ////เฟส2
        if (isPhase2 && canTeleport)
        {
            teleportTimer -= Time.deltaTime;
            if (teleportTimer <= 0f )
            {
                StartCoroutine(TeleportBehindPlayer());
                teleportTimer = teleportCooldown;
            }
        }
    }

    private void ChasePlayer()
    {
        anim.SetBool("Run", true);

        if (transform.position.x < player.position.x )
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    public void Attack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");
        rb.velocity = Vector2.zero;

    }
    public void AttackSFX()
    {
        if (audioSource != null && AttackSfx != null)
            audioSource.PlayOneShot(AttackSfx);

    }
    public void EndAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        anim.SetTrigger("Hurt");
        StartCoroutine(FlashRed());

        if (audioSource != null && enemyHit != null)
            audioSource.PlayOneShot(enemyHit);

        if (health <= 0)
            Die();
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        anim.SetTrigger("Dead");
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        gameObject.tag = "Untagged";
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        if (audioSource != null && enemyDead != null)
            audioSource.PlayOneShot(enemyDead);

       
        StartCoroutine(Endgame());
    }

    private IEnumerator TeleportBehindPlayer()
    {
        if (player == null || !canTeleport) yield break;

        canTeleport = false;
        isAttacking = true;

        anim.SetTrigger("Vanish");
        rb.velocity = Vector2.zero;
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(0.05f);

        float offset = 10.5f;
        Vector3 targetPos = player.position;

        // วาร์ปไปด้านหลังผู้เล่น (เฉพาะแกน X)
        if (player.position.x < transform.position.x)
            targetPos.x -= offset;
        else
            targetPos.x += offset;

        // ✅ ล็อก Y ไว้กับพื้น ไม่ตามผู้เล่นขึ้นฟ้า
        targetPos.y = transform.position.y;

        transform.position = targetPos;

        // ✅ หันหน้าเข้าหาผู้เล่นเสมอ
        if (player.position.x < transform.position.x)
            transform.eulerAngles = new Vector3(0, 0, 0); // หันซ้าย
        else
            transform.eulerAngles = new Vector3(0, 180, 0); // หันขวา

        yield return new WaitForSeconds(0.2f);

        spriteRenderer.enabled = true;
        anim.SetTrigger("Reappear");
        if (audioSource && Fade)
            audioSource.PlayOneShot(Fade);

        yield return new WaitForSeconds(0.3f);
        if (!isAttacking)
        {
            Attack();
        }

        yield return new WaitForSeconds(1f);

     
        canTeleport = true;
    }
    private IEnumerator Endgame()
    {
        UnlockNextLevel();
        yield return new WaitForSeconds(delayBeforeWinUI);

        // แสดง Win UI
        if (winPanel != null)
        {
            if (playerScript != null) playerScript.enabled = false;
            Cursor.visible = true;
            winPanel.SetActive(true);
            Time.timeScale = 0f; // หยุดเกมหลังจากขึ้น UI
            if (audioSource && Amytalk)
                audioSource.PlayOneShot(Amytalk);
        }
        Destroy(gameObject, 2f);
    }
    void UnlockNextLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (currentLevel >= unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);
            PlayerPrefs.Save();
        }
    }


    // แสดงระยะการตรวจจับใน Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, agroRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}