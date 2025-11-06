using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDarkoneClone : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform shootPoint;
    public float fireballSpeed = 6f;
    public float attackCooldown = 2.5f;
    
    [Header("Player")]
    public Transform player;
    
    private Animator anim;

    [Header("Detection")]
    public float agroRange = 12f;   // 👈 ระยะตรวจจับผู้เล่น
    private bool playerInRange = false;


    public AudioSource audioSource;
    public AudioClip castSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip fireballSound;
    public AudioClip fateSound;

    private SpriteRenderer spriteRenderer;
    private bool isShooting = false;
    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;
       
        FacePlayer();

        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = (distance <= agroRange);

        if (!playerInRange) return; // ❌ ถ้ายังไม่เข้าระยะก็ไม่ทำอะไรเลย

    

        // ยิงเวทย์เรื่อย ๆ
        if (!isShooting)
            StartCoroutine(AutoCast());
    }

    private IEnumerator AutoCast()
    {
        isShooting = true;
        anim.SetTrigger("Cast");
        yield return new WaitForSeconds(attackCooldown);
        isShooting = false;
    }

    // 🔥 เรียกจาก Animation Event ตอนร่ายเวทย์
    public void FireballAttack()
    {
        if (fireballPrefab != null && shootPoint != null && player != null)
        {
            Vector2 dir = (player.position - shootPoint.position).normalized;
            GameObject fb = Instantiate(fireballPrefab, shootPoint.position, Quaternion.identity);
            fb.GetComponent<Rigidbody2D>().velocity = dir * fireballSpeed;
        }
    }

    // 🔊 เสียงตอนร่ายคาถา
    public void FireballAttackCastSFX()
    {
        if (audioSource && castSound)
            audioSource.PlayOneShot(castSound);
    }

    // 🔊 เสียงตอนปล่อยลูกไฟ
    public void FireballAttackShotSFX()
    {
        if (audioSource && fireballSound)
            audioSource.PlayOneShot(fireballSound, 0.5f);
    }
    public void FateSFX()
    {
        if (audioSource && fateSound)
            audioSource.PlayOneShot(fateSound);
    }
    public void FireballAttackEnd()
    {
        isShooting = false;

    }

    private void FacePlayer()
    {
        if (player.position.x < transform.position.x)
            transform.eulerAngles = Vector3.zero;
        else
            transform.eulerAngles = new Vector3(0, 180, 0);

    }


    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
    private void OnDrawGizmosSelected()
    {
        // 🔵 แสดงระยะการตรวจจับใน Scene
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, agroRange);
    }
}
