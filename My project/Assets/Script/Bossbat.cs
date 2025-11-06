using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bossbat : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform[] waypoints; // จุดบินไปมา
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float waitTime = 1f; // เวลาพักเมื่อถึงจุด
    private int currentWaypoint = 0;
    private bool isWaiting = false;

    [Header("Stats")]
    public int health = 30;
    private bool isDead = false;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bossRoar;  // เสียงตอนปรากฏตัว
    [SerializeField] private AudioClip bossHit;   // เสียงโดนตี
    [SerializeField] private AudioClip bossDead;  // เสียงตอนตาย
    [SerializeField] private AudioClip Amytalk;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    Animator anim;

    public Text HP;
    public Slider sliderHP;
    public GameObject winPanel;
    public float delayBeforeWinUI = 3f; // เวลาให้เดินเล่นก่อนโชว์ UI

    [Header("Player Control")]
    public Myscript1 playerScript;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        if (audioSource != null && bossRoar != null)
            audioSource.PlayOneShot(bossRoar); // เสียงเตือนตอนเจอบอส

        sliderHP.maxValue = health;
        sliderHP.value = health;

        anim = GetComponent<Animator>();
    }

    private void Update()
    {

        HP.text = "HP: " + health;
        sliderHP.value = health;

        if (isDead || isWaiting) return;

        MovePattern();
        
        if (health <= 15)
        {
            moveSpeed = 50;
            waitTime = 0.5f;
        }
        
    }

    private void MovePattern()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        currentWaypoint++;
        if (currentWaypoint >= waypoints.Length)
            currentWaypoint = 0;

        isWaiting = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        if (audioSource != null && bossHit != null)
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
        
        isDead = true;
        
        if (audioSource != null && bossDead != null)
            audioSource.PlayOneShot(bossDead);

        anim.SetTrigger("dead");
       
        StartCoroutine(Endgame());
           
        

       
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
        Destroy(gameObject, 1f);
    }


}

