using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCon : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] float agroRage;
    [SerializeField] float attack;
    [SerializeField] float moveSpeed;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip Enemyhit;
    [SerializeField] private AudioClip Enemydead;

    public int health;
    private bool isDead = false;   // ✅ ตัวแปรกันบัค
    Animator anim;
    Rigidbody2D rb;

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

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (isDead) return; // ✅ ถ้าตายแล้ว ไม่ทำอะไรต่อ

        float disToPlayer = Vector2.Distance(transform.position, Player.position);
        if (disToPlayer < agroRage)
        {
            chasePlayer();
        }
        else
        {
            anim.SetBool("Run", false);
            rb.velocity = Vector2.zero;
        }

        if (disToPlayer < attack)
        {
            anim.SetTrigger("Attack");
            anim.SetBool("Run", false);
            rb.velocity = Vector2.zero;
        }

        if (health <= 0)
        {
            health = 0;
            Dead();

        }
    }

    void chasePlayer()
    {
        anim.SetBool("Run", true);
        if (transform.position.x < Player.position.x)
        {
            rb.velocity = new Vector2(moveSpeed, 0);
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            rb.velocity = new Vector2(-moveSpeed, 0);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    public void Dead()
    {
        if (isDead) return;
        isDead = true;

        anim.SetTrigger("dead");

        // ✅ เล่นเสียงตาย
        if (audioSource != null && Enemydead != null)
        {
            audioSource.PlayOneShot(Enemydead, 0.5f);
        }

        // ✅ หยุดขยับ
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        gameObject.tag = "Untagged";
        // ✅ ปิด Collider ทั้งหมดใน Enemy
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }

        StartCoroutine(EnemyDead());
    }

    IEnumerator EnemyDead()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // ✅ ถ้าตายแล้ว โดนดาเมจเพิ่มไม่ได้
        health -= damage;
        anim.SetTrigger("Hurt");
        Debug.Log("Enemy damage: " + damage);
        audioSource.PlayOneShot(Enemyhit);
        StartCoroutine(FlashRed());
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
        Gizmos.DrawWireSphere(transform.position, agroRage);


    }
}