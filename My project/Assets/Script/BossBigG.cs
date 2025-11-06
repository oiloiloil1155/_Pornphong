using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossBigG : MonoBehaviour
{


    [Header("Stats")]
    public int health = 30;
    private bool isDead = false;

    [Header("Attack Pattern")]
    public float attackDelay = 2f;
    private bool isAttacking = false;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bossHit;
    [SerializeField] private AudioClip bossDead;
    [SerializeField] private AudioClip Amytalk;
    [SerializeField] private AudioClip BreakSFX;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Animator anim;

    public Text HP;
    public Slider sliderHP;
    public GameObject winPanel;
    public float delayBeforeWinUI = 3f;

    private float damageCooldown = 0.5f;
    private float damageTimer = 0f;

    [Header("Player Control")]
    public Myscript1 playerScript;


    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        sliderHP.maxValue = health;
        sliderHP.value = health;

        anim = GetComponent<Animator>();


        // เริ่มลูปโจมตี
       
    }

    private void Update()


    {

        HP.text = "HP: " + health;
        sliderHP.value = health;
        if(!isAttacking)
        StartCoroutine(AttackLoop());
        
        if (damageTimer > 0f)
            damageTimer -= Time.deltaTime;


    }

    public void TakeDamage(int damage)
    {

        if (isDead || damageTimer > 0f) return; // 💥 กันโดนซ้ำ

        damageTimer = damageCooldown;

        

        health -= damage;
        if (audioSource && bossHit)
            audioSource.PlayOneShot(bossHit);

        StartCoroutine(FlashRed());

        if (health <= 0)
            Dead();
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private void Dead()
    {
        if (isDead) return;
        isDead = true;

        if (audioSource && bossDead)
            audioSource.PlayOneShot(bossDead);

        anim.SetTrigger("dead");
        StopAllCoroutines(); // หยุดสุ่มโจมตี

        StartCoroutine(Endgame());
    }

    private IEnumerator Endgame()
    {
        UnlockNextLevel();
        yield return new WaitForSeconds(delayBeforeWinUI);

        if (winPanel != null)
        {
            if (playerScript != null) playerScript.enabled = false;
            if (audioSource && Amytalk)
                audioSource.PlayOneShot(Amytalk);
            winPanel.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
        }

        Destroy(gameObject, 1f);
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

    // เรียกจาก Animation Event เพื่อบอกว่าจบการโจมตีแล้ว
    public void EndAttack()
    {
        isAttacking = false;
    }
    public void BreakSfx()
    {
        if (audioSource && BreakSFX)
            audioSource.PlayOneShot(BreakSFX);
    }

    // 🔥 ลูปโจมตีแบบ Coroutine
    private IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(2f); // รอเปิดตัวก่อนโจมตี

        while (!isDead)
        {
            if (!isAttacking)
            {
                isAttacking = true;

                if (health > 25)
                {
                    int set = Random.Range(1, 3); // 1–2
                    anim.SetTrigger("AttackSet" + set);
                }
                else
                {
                    int set = Random.Range(3, 5); // 3–4
                    anim.SetTrigger("AttackSet" + set);
                }
            }

            // เว้นช่วงก่อนสุ่มโจมตีต่อ
            yield return new WaitForSeconds(attackDelay);
        }
    }

}
