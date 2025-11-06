using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class BossSamurai : MonoBehaviour
{
    [Header("Boss Stats")]
    [SerializeField] private Transform player;
    [SerializeField] private float agroRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int health = 50;

    private bool isDead = false;
    private bool isAttacking = false;
    private bool isUsingSkill = false;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip enemyHit;
    [SerializeField] private AudioClip enemyDead;
    [SerializeField] private AudioClip enemyattack;
    [SerializeField] private AudioClip enemyslash;
    [SerializeField] private AudioClip enemycharge;
    [SerializeField] private AudioClip Amytalk;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public Text HP;
    public Slider sliderHP;
    public GameObject winPanel;
    public float delayBeforeWinUI = 3f;

    // ✅ ระบบท่าพิเศษ
    private bool usedSkill35 = false;
    private bool usedSkill25 = false;
    private bool usedSkill15 = false;

    // ✅ จุดวาร์ป
    [Header("Special Skill Warp Points")]
    public Transform warpPoint1;
    public Transform warpPoint2;
    public Transform warpPoint3;

    [Header("Skill  Settings")]
    public GameObject swordWavePrefab; // ✅ prefab ของคลื่นดาบ
    public Transform shootPoint;        // ✅ จุดปล่อยคลื่นดาบ
    public float swordWaveSpeed = 10f;  // ✅ ความเร็วคลื่นดาบ

    [Header("Player Control")]
    public Myscript1 playerScript;

    private float defaultAgroRange;


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

        defaultAgroRange = agroRange;
    }

    private void Update()
    {
        HP.text = "HP: " + health;
        sliderHP.value = health;

        if (isUsingSkill) return; // ถ้าเพิ่งเรียกสกิล จะหยุดการเคลื่อนไหว

        if (isDead || player == null) return;
        if (isUsingSkill) return; // ❌ หยุดทุกอย่างระหว่างใช้สกิล

        float distance = Vector2.Distance(transform.position, player.position);

        // ✅ เช็กเงื่อนไขใช้ท่าพิเศษ
        CheckSpecialSkill();

        if (distance <= agroRange && distance > attackRange && !isAttacking && !isUsingSkill)
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
        if (isUsingSkill)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("Run", false); 
            return;
        }
    }

    private void ChasePlayer()
    {
        anim.SetBool("Run", true);
        if (transform.position.x < player.position.x)
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

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
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

    private IEnumerator Endgame()
    {
        UnlockNextLevel();
        yield return new WaitForSeconds(delayBeforeWinUI);

        if (winPanel != null)
        {
            if (playerScript != null) playerScript.enabled = false;
            Cursor.visible = true;
            winPanel.SetActive(true);
            Time.timeScale = 0f;
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
    private void FacePlayer()
    {
        if (player == null) return;
        if (transform.position.x < player.position.x)
            transform.eulerAngles = new Vector3(0, 180, 0);
        else
            transform.eulerAngles = new Vector3(0, 0, 0);
    }

    // ✅ ตรวจ HP เพื่อใช้สกิลพิเศษ
    private void CheckSpecialSkill()
    {
       
        if (isUsingSkill) return;

        if (!usedSkill35 && health <= 35 && health > 25)
        {
            StartCoroutine(WarpAndUseSkill("Skill_1", warpPoint1));
            usedSkill35 = true;
        }
        else if (!usedSkill25 && health <= 25 && health > 15)
        {
            StartCoroutine(WarpAndUseSkill("Skill_2", warpPoint2));
            usedSkill25 = true;
            agroRange = 0f;
        }
        else if (!usedSkill15 && health <= 15)
        {
            StartCoroutine(WarpAndUseSkill("Skill_1", warpPoint1));
            usedSkill15 = true;
        }
    }

    // ✅ ฟังก์ชันวาร์ปและใช้สกิล
    private IEnumerator WarpAndUseSkill(string animTrigger, Transform warpPoint)
    {
        if (isUsingSkill) yield break;
        isUsingSkill = true;
        isAttacking = true;
        rb.velocity = Vector2.zero;
        agroRange = 0f;

        // วาร์ปไปตำแหน่งที่ตั้งไว้
        if (warpPoint != null)
        {
            anim.SetTrigger("TeleportOut"); // ถ้ามีอนิเมชันวาร์ปออก
            yield return new WaitForSeconds(0.5f);
            transform.position = warpPoint.position;
            FacePlayer();
            yield return new WaitForSeconds(0.2f);
            anim.SetTrigger("TeleportIn"); // ถ้ามีอนิเมชันวาร์ปเข้า
        }

        // ใช้สกิลหลัก
        yield return new WaitForSeconds(0.3f);
        anim.SetTrigger(animTrigger);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(5.3f); // เวลาสกิลจบ
       
        EndSkill();


    }


    private void EndSkill()
    {
       
        isAttacking = false;
        isUsingSkill = false;
        agroRange = defaultAgroRange;


    }
    private void Stoprun()
    {


        agroRange = 0f;


    }
    private void EnemychargeSound()
    {

        if (audioSource && enemycharge)
            audioSource.PlayOneShot(enemycharge);


    }
    private void EnemyattackSound()
    {
        if (audioSource && enemyattack)
            audioSource.PlayOneShot(enemyattack);


    }
    private void EnemyslashSound()
    {

        if (audioSource && enemyslash)
            audioSource.PlayOneShot(enemyslash);


    }

    public void ShootSwordWave()
    {
        if (swordWavePrefab == null || shootPoint == null) return;

        int direction = (transform.eulerAngles.y == 180) ? 1 : -1;

        Vector3 spawnPos = shootPoint.position + new Vector3(0.5f * direction, 0, 0);
        GameObject wave = Instantiate(swordWavePrefab, spawnPos, Quaternion.identity);

        Rigidbody2D waveRb = wave.GetComponent<Rigidbody2D>();
        if (waveRb != null)
            waveRb.velocity = new Vector2(swordWaveSpeed * direction, 0);

        // ปรับขนาดให้ถูกทางโดยไม่กลับหัว
        Vector3 scale = wave.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        wave.transform.localScale = scale;
    }


private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, agroRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // แสดงจุดวาร์ป
        if (warpPoint1 != null)
            Gizmos.DrawSphere(warpPoint1.position, 0.2f);
        if (warpPoint2 != null)
            Gizmos.DrawSphere(warpPoint2.position, 0.2f);
       
    }

}
