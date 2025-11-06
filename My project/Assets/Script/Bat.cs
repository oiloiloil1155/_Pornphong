using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bat : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] float agroRange = 10f;     // ระยะมองเห็นผู้เล่น
    [SerializeField] float moveSpeed = 3f;      // ความเร็วบิน
    [SerializeField] int damage = 2;            // ดาเมจที่ทำเมื่อชน
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip alertSound;
    [SerializeField] private AudioClip Enemyhit;
    [SerializeField] private AudioClip Enemydead;

    public int health = 10;
    private bool isDead = false;
    private bool hasAlerted = false;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;



    private void Awake()
    {
        Player = FindObjectOfType<Myscript1>().transform;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        rb.gravityScale = 0; // ✅ ปิดแรงโน้มถ่วง

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }


    void Update()
    {
        if (isDead) return;

        float disToPlayer = Vector2.Distance(transform.position, Player.position);

        if (disToPlayer < agroRange)
        {
            if (!hasAlerted)
            {
                PlayAlert();
                hasAlerted = true;
            }
            ChasePlayer();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        if (health <= 0)
        {
            health = 0;
            Dead();
        }
    }

    void ChasePlayer()
    {
        // ✅ บินตรงเข้าหาผู้เล่น
        Vector2 direction = (Player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        // ✅ พลิกหันหน้าตามแกน X
        if (rb.velocity.x > 0)
            transform.eulerAngles = new Vector3(0, 180, 0);
        else if (rb.velocity.x < 0)
            transform.eulerAngles = new Vector3(0, 0, 0);
    }


    public void Dead()
    {
        if (isDead) return;
        isDead = true;

        if (anim != null) anim.SetTrigger("dead");

        // ✅ เล่นเสียงตาย
        if (audioSource != null && Enemydead != null)
            audioSource.PlayOneShot(Enemydead, 0.3f);

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        StartCoroutine(EnemyDead());
    }

    IEnumerator EnemyDead()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;

        if (anim != null) anim.SetTrigger("Hurt");

        if (audioSource != null && Enemyhit != null)
            audioSource.PlayOneShot(Enemyhit);

        StartCoroutine(FlashRed());
    }
    private void PlayAlert()
    {
        if (audioSource != null && alertSound != null)
        {
            audioSource.PlayOneShot(alertSound, 0.6f);
           
        }
    }


    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, agroRange);

        
    }
}

