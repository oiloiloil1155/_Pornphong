using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossDarkOne : MonoBehaviour
{
    private List<GameObject> activeClones = new List<GameObject>();
    // Start is called before the first frame update
    [Header("Stats")]
    public int health = 100;
    public Text hpText;
    public Slider hpSlider;

    [Header("Player")]
    public Transform player;

    [Header("Detection")]
    public float agroRange = 12f;   // 👈 ระยะตรวจจับผู้เล่น
    private bool playerInRange = false;


    [Header("Magic Attack")]
    public GameObject fireballPrefab;
    public Transform shootPoint;
    public float fireballSpeed = 7f;
    public float attackCooldown = 2.5f;

    [Header("Phase 2 Settings")]
    public GameObject clonePrefab;
    public Transform[] cloneSpawnPoints;
    public float cloneDuration = 10f;
    private bool phase2Triggered = false;

    [Header("Phase 3 Settings")]
    public GameObject clonePrefab2;
    public Transform[] cloneSpawnPoints2;
    public float cloneDuration2 = 10f;
    private bool phase3Triggered = false;

    [Header("Teleport Settings")]
    public Transform[] warpPoints;
    private bool isUsingSkill = false;

    [Header("UI")]
    public GameObject winPanel;
    public float delayBeforeWinUI = 3f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip castSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip fireballSound;
    public AudioClip fateSound;
    [SerializeField] private AudioClip Amytalk;


    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;

    [Header("Player Control")]
    public Myscript1 playerScript;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        hpSlider.maxValue = health;
        hpSlider.value = health;
    }

    void Update()
    {
        hpText.text = "HP: " + health;
        hpSlider.value = health;

        if (isDead || isUsingSkill) return;
        if (player == null) return;

        FacePlayer();

        // ✅ ตรวจว่าผู้เล่นอยู่ในระยะหรือไม่
        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = (distance <= agroRange);

        if (!playerInRange) return; // ❌ ถ้ายังไม่เข้าระยะก็ไม่ทำอะไรเลย

        // เช็กเฟสของบอส
        if (!phase2Triggered && health <= 40)
        {
            StartCoroutine(EnterPhase2());
            phase2Triggered = true;
        }
        else if (!phase3Triggered && health <= 20)
        {
            StartCoroutine(EnterPhase3());
            phase3Triggered = true;
        }

        // ยิงเวทย์พื้นฐาน
        if (!isUsingSkill)
            isUsingSkill = true;
            anim.SetTrigger("Cast");
    }

    public void FireballAttack()
    {

        if (fireballPrefab != null && shootPoint != null)
        {
            Vector2 dir = (player.position - shootPoint.position).normalized;
            GameObject fb = Instantiate(fireballPrefab, shootPoint.position, Quaternion.identity);
            fb.GetComponent<Rigidbody2D>().velocity = dir * fireballSpeed;
        }
    }
    public void FireballAttackEnd()
    {
        isUsingSkill = false;
        Transform warp = warpPoints[Random.Range(0, warpPoints.Length)];
        transform.position = warp.position;
   
    }

    public void FireballAttackCastSFX()
    {
        if (audioSource && castSound)
            audioSource.PlayOneShot(castSound);
    }
    public void FireballAttackShotSFX()
    {
        if (audioSource && fireballSound)
            audioSource.PlayOneShot(fireballSound,0.5f);
    }
    public void FateSFX()
    {
        if (audioSource && fateSound)
            audioSource.PlayOneShot(fateSound);
    }





    private IEnumerator EnterPhase2()
    {
        isUsingSkill = true;
        anim.SetTrigger("TeleportOut");
        yield return new WaitForSeconds(0.5f);

        Transform warp = warpPoints[Random.Range(0, warpPoints.Length)];
        transform.position = warp.position;
        

        yield return new WaitForSeconds(0.5f);

        // เรียกร่างเงา
        foreach (Transform point in cloneSpawnPoints)
        {
            GameObject clone = Instantiate(clonePrefab, point.position, Quaternion.identity);
            activeClones.Add(clone);
            Destroy(clone, cloneDuration);
        }

        isUsingSkill = false;
    }

    private IEnumerator EnterPhase3()
    {
        isUsingSkill = true;
        anim.SetTrigger("TeleportOut");
        yield return new WaitForSeconds(0.5f);

        Transform warp = warpPoints[Random.Range(0, warpPoints.Length)];
        transform.position = warp.position;


        yield return new WaitForSeconds(0.5f);

        // เรียกร่างเงา
        foreach (Transform point in cloneSpawnPoints2)
        {
            GameObject clone = Instantiate(clonePrefab2, point.position, Quaternion.identity);
            activeClones.Add(clone);
            Destroy(clone, cloneDuration2);
        }

        isUsingSkill = false;
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        health -= dmg;
        if (audioSource && hurtSound)
            audioSource.PlayOneShot(hurtSound);

        StartCoroutine(FlashRed());

        if (health <= 0)
            Die();
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        anim.SetTrigger("Dead");
        if (audioSource && deathSound)
            audioSource.PlayOneShot(deathSound);
        
        ///ทำลายโคลนทั้งหมดท
        foreach (GameObject clone in activeClones)
        {
            if (clone != null)
                Destroy(clone);
        }
        activeClones.Clear();

        StartCoroutine(WinSequence());
    }

    private IEnumerator WinSequence()
    {
        if (winPanel != null)
        {
            UnlockNextLevel();
            yield return new WaitForSeconds(delayBeforeWinUI);
            winPanel.SetActive(true);
            Time.timeScale = 0f;
            if (playerScript != null) playerScript.enabled = false;
            if (audioSource && Amytalk)
                audioSource.PlayOneShot(Amytalk);
            Cursor.visible = true;
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
        if (player.position.x < transform.position.x)
            transform.eulerAngles = Vector3.zero;
        else
            transform.eulerAngles = new Vector3(0, 180, 0);
 
    }
    private void OnDrawGizmosSelected()
    {
        // 🔵 แสดงระยะการตรวจจับใน Scene
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, agroRange);
    }
}
