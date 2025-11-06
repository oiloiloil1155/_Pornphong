using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using static UnityEngine.GraphicsBuffer;

public class Myscript1 : MonoBehaviour
{
    [Header("UI Game Over")]
    public GameObject gameOverPanel;
    [Header("UI Pause Menu")]
    public GameObject pauseMenuPanel;
    private bool isPaused = false;
    [Header("Fade Settings")]
    public Animator fadeAnimator;
    public float fadeDuration = 1f;
    private Coroutine gameOverCoroutine;

    public float moveX;
    private Rigidbody2D rb;
    public float speed;
    public float jumpForce;
    public bool isJumping ;
    private Animator anim;
    public bool Onground;
    public bool isAttacking = false;
    public bool isDead = false;

    [SerializeField] private float healthbar;
    public int Potion;
    public int Knife;

    public int maxKnife = 5;
    public int maxPotion = 3;



    public Text HP;
    public Text cPotion;
    public Text cKnife;
    public Slider sliderHP;
    [SerializeField] private bool healing = false;


    [SerializeField] private bool isRight = true;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideCooldown = 2f; // คูลดาวน์ 2 วิ
    private float slideTimer = 0f;
    [SerializeField] private bool isSlide = false;

    public Transform wallCheckpos;
    public Vector2 wallCheckSize = new Vector2(1.65f, 0.58f);
    public LayerMask wallLayer;

    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    public float wallSlideSpeed = 2;
    bool isWallSliding;
    
    [SerializeField] Vector3 footOffset;
    [SerializeField] private float footRadius;
    [SerializeField] LayerMask groundLayerMask;

    private float timeBtwAttack;
    public float startTimeBtwAttack;
    public Transform attackPos;
    public LayerMask whatIsEnemies;
    public float attackRange;
    public int damage;

    public GameObject projectilePrefab;       // กระสุน Prefab
    public Transform projectileSpawn;          // ตำแหน่งยิง
    public float projectileSpeed;       // ความเร็วกระสุน
    private float rangedCooldown = 0.5f;
    private float rangedTimer = 0f;

    [SerializeField] private float damageCooldown = 1f; // เวลา delay 1 วิ
    private float damageTimer = 0f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip throwknife;
    [SerializeField] private AudioClip hurt;
    [SerializeField] private AudioClip pickupitem;
    [SerializeField] private AudioClip heal;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;

    [SerializeField] private GameObject healAra;

    [Header("Cheats")]
    [SerializeField] private Transform bossRoomSpawn; // ลาก Transform จุดหน้าห้องบอสใน Inspector
    [SerializeField] private int pressesNeeded = 3;    // จำนวนครั้งที่ต้องกด (ค่าเริ่มต้น 3)
    [SerializeField] private float pressWindow = 1.5f; // เวลาหน้าต่าง (วินาที) ถ้ากดครบภายในเวลานี้จะวาป

    private int oPressCount = 0;
    private float oTimer = 0f;
    private bool TPCount;


    private void Awake()
    {
        // ซ่อน Cursor ตอนเริ่มเกม
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined; 
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sliderHP.maxValue = healthbar;
        sliderHP.value = healthbar;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;

      
    }
    private void Update()
    {
     


        if (damageTimer > 0)
            damageTimer -= Time.deltaTime;

        if (slideTimer > 0)
            slideTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isDead &&!isPaused && pauseMenuPanel != null && !gameOverPanel.activeSelf)
            {
                PauseGame();
            }
            else if (!isDead && isPaused)
            {
                ResumeGame();
            }
        }

        if (oPressCount > 0)
        {
            oTimer -= Time.unscaledDeltaTime; // ใช้ unscaled เพื่อให้ทำงานแม้ Time.timeScale ถูกเปลี่ยน (pause/game over)
            if (oTimer <= 0f)
            {
                // หมดหน้าต่างแล้ว รีเซ็ตตัวนับ
                oPressCount = 0;
            }
        }

        // ถ้ากด O
        if (Input.GetKeyDown(KeyCode.O))
        {
            
            oPressCount++;
            oTimer = pressWindow; // รีเซ็ตเวลาเมื่อกดแต่ละครั้ง

            if (oPressCount >= pressesNeeded )
            {
                // เงื่อนไขพิเศษ: ป้องกันตอนตาย/ขณะหยุดเล่น
                if (!isDead && !isPaused && !TPCount) // หรือเงื่อนไขอื่นตามที่ต้องการ
                {
                    DoCheatTeleport();
                }
                oPressCount = 0; // รีเซ็ตเสมอหลังทำงาน
                oTimer = 0f;
            }
        }

        HP.text = "HP: " + healthbar;
        cPotion.text = "x" + Potion;
        cKnife.text = "x" + Knife;
        if (healthbar <= 0)
        {
            healthbar = 0;
            Dead();
        }
        if (healthbar > 25) {
            healthbar = 25;
        }
       
        
        
        if (Potion <= 0)
        {
            Potion = 0;
        }

        if (Potion > 3)
        {
            Potion = 3;
        }

        sliderHP.value = healthbar;
       
        if (Input.GetKeyDown(KeyCode.Q) && Onground && !isJumping && !isSlide && Potion != 0)
        {
            healing = true;
            anim.SetBool("Heal", true);
            rb.velocity = Vector2.zero;
            StartCoroutine(StopHeal());
            

        }
        IEnumerator StopHeal()
        {
            yield return new WaitForSeconds(0.5f);
            anim.SetBool("Heal", false);
            healing = false;
            
        }
      
           
        


        moveX = Input.GetAxis("Horizontal");
        if (!isAttacking && !healing && !isSlide)
        {
            rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
        }
        else 
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // หยุดเดินชั่วคราว
        }

        anim.SetBool("run", moveX != 0);
        anim.SetBool("ground", Onground);

        if(Onground)
        {
            isJumping = false;
        }

        if (Input.GetButtonDown("Jump") && !isJumping && Onground)
        {
            Jump();
            isAttacking = false;
        }
        else if (Input.GetButtonDown("Jump") && isWallSliding)
        {
            WallJump();
        }

        if (moveX > 0.01f)
        {
            isRight = true;
            transform.localScale = Vector3.one;
        }
        else if (moveX < -0.01f)
        {
            isRight = false;
            transform.localScale = new Vector3(-1, 1, 1);

        }

        if (!isSlide && timeBtwAttack <= 0)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                

                if (Onground)
                {
                    isAttacking = true;
                    rb.velocity = Vector2.zero; // หยุดเดินชั่วคราว
                    anim.SetTrigger("Attack");
                    StartCoroutine(AttackCoroutine());
                }
                else
                {
                    anim.SetTrigger("JumpAttack");
                    
                    // อย่ารีเซ็ต isAttacking ตรงนี้ ให้ EndAttack() จัดการ
                }

                timeBtwAttack = startTimeBtwAttack;
                
            }
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.L) && slideTimer <= 0)
        {
            Slide();
            slideTimer = slideCooldown;
        }
        ProcessWallSlide();
        ProcessWallJump();

        if (!isWallJumping)
        {
            if (Onground && isAttacking)
            {
                // ถ้าโจมตีบนพื้น → หยุดเดิน
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else
            {
                // ปกติ หรือโจมตีกลางอากาศ → เคลื่อนที่ตาม input
                rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
            }
        }


        //ตีไกล
        if (rangedTimer > 0 ) rangedTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.K) && rangedTimer <= 0 && Knife != 0 && !isSlide)
        {
            if (Onground)
            {
                // ขว้างบนพื้น
                anim.SetTrigger("RangeAttack");
            }
            else
            {
                // ขว้างกลางอากาศ
                anim.SetTrigger("JumpThrow");
            }

            RangedAttack();
            rangedTimer = rangedCooldown;

        }


    }


    private void Healing()
    {
        healthbar = healthbar + 15;
        Potion = Potion - 1;
        audioSource.PlayOneShot(heal);
        healAra.SetActive(true);
        StartCoroutine(StopHealAra());
        
    }
    IEnumerator StopHealAra()
    {
        yield return new WaitForSeconds(0.6f);
        healAra.SetActive(false);
        isAttacking = false;
    }
    private void StopRunHealing()
    {
        isAttacking = true;
    }


    private void DoCheatTeleport()
    {
        if (bossRoomSpawn == null)
        {
            Debug.LogWarning("BossRoomSpawn not set on Myscript1.");
            return;
        }

        // ปิดการเคลื่อนที่ชั่วคราว / รีเซ็ต velocity ถ้าต้องการ
        rb.velocity = Vector2.zero;
        TPCount = true;
        // ย้ายตำแหน่งผู้เล่นไปตำแหน่ง bossRoomSpawn
        transform.position = bossRoomSpawn.position;

        // ถ้าต้องการ รีเซ็ตสถานะบางอย่าง เช่น Onground / isJumping ฯลฯ
        Onground = false; // หรือเช็ค/เซ็ตตามระบบที่มี
        isAttacking = false;


        Debug.Log("Cheat: Teleported to boss room.");
    }



    private void RangedAttack()
    {
       
        isAttacking = true;                // ล็อกการเคลื่อนไหว
        rb.velocity = Vector2.zero;        // หยุดตัวละครไม่ให้เดิน
        anim.SetTrigger("RangeAttack");
        float direction = isRight ? 1f : -1f;

        // ถ้าหันขวา → Quaternion.identity
        // ถ้าหันซ้าย → พลิก 180 องศา (แกน Y)
        Quaternion rotation = isRight ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

        // สร้าง Projectile โดยใส่ rotation ที่ถูกต้อง
        GameObject proj = Instantiate(projectilePrefab, projectileSpawn.position, rotation);

        Rigidbody2D rbProj = proj.GetComponent<Rigidbody2D>();
        rbProj.velocity = new Vector2(projectileSpeed * direction, 0f);
    }

    private void RangedAttackSFX()
    {
        audioSource.PlayOneShot(throwknife);
    }



    public void EndRangedAttack()
    {
        
        isAttacking = false;               // คืนการเคลื่อนไหว
        anim.SetTrigger("End");  // ป้องกัน animation ค้าง
        Knife = Knife - 1;
    }






    private void Jump()
    {
        isJumping = true;
        rb.AddForce(new Vector2(rb.velocity.x, jumpForce));
        anim.SetTrigger("jump");
        Onground = false;

    }
    private void JumpSound()
    {
        if (audioSource && jumpSound)
            audioSource.PlayOneShot(jumpSound);

    }
    private void DashSound()
    {
        if (audioSource && dashSound)
            audioSource.PlayOneShot(dashSound);

    }

    private void WallJump()
    {
        rb.AddForce(new Vector2(rb.velocity.x, jumpForce));
        anim.SetTrigger("walljump");
        Onground = false;
        if (wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0;

            if (transform.localScale.x != wallJumpDirection)
            {

            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        }
        
    }
   

    public void Attack()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            EnemyCon swordEnemy = enemiesToDamage[i].GetComponent<EnemyCon>();
            Bat batEnemy = enemiesToDamage[i].GetComponent<Bat>();
            Bossbat bossbatEnemy = enemiesToDamage[i].GetComponent<Bossbat>();
            EnemyArcher bowmanEnemy = enemiesToDamage[i].GetComponent<EnemyArcher>();
            EnemyMagic magicEnemy = enemiesToDamage[i].GetComponent<EnemyMagic>();
            BossAssassin bossAssassin = enemiesToDamage[i].GetComponent<BossAssassin>();
            BossBigG bossBigG = enemiesToDamage[i].GetComponent<BossBigG>();
            BossSamurai bossSamurai = enemiesToDamage[i].GetComponent<BossSamurai>();
            BossDarkOne bossDarkone = enemiesToDamage[i].GetComponent<BossDarkOne>();

            if (swordEnemy != null)
            {
                swordEnemy.TakeDamage(damage);
            }
            else if (batEnemy != null)
            {
                batEnemy.TakeDamage(damage);
            }
            else if (bossbatEnemy != null)
            {
                bossbatEnemy.TakeDamage(damage);
            }
            else if (bowmanEnemy != null)
            {
                bowmanEnemy.TakeDamage(damage);
            }
            else if (bossAssassin != null)
            {
                bossAssassin.TakeDamage(damage);
            }
            else if (bossBigG != null)
            {
                bossBigG.TakeDamage(damage);
            }
            else if (bossSamurai != null)
            {
                bossSamurai.TakeDamage(damage);
            }
            else if (magicEnemy != null)
            {
                magicEnemy.TakeDamage(damage);
            }
            else if (bossDarkone != null)
            {
                bossDarkone.TakeDamage(damage);
            }



        }
    }
   

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger(Onground ? "Attack" : "JumpAttack");

        yield return new WaitForSeconds(0.3f); // เวลาของ Animation
        isAttacking = false;
    }
    public void Slide()
    {
        isSlide = true;
        anim.SetBool("Dash", true);
        if (isRight)
        {
            rb.AddForce(Vector2.right * slideSpeed);
        }
        else if (!isRight)
        {
            rb.AddForce(Vector2.left * slideSpeed);
        }
        StartCoroutine(StopSlide());
    }
   
    IEnumerator StopSlide()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Dash", false);
        isSlide = false;
    }


            /////เก็บของ
    public void AddPotion(int amount)
    {
        Potion += amount;
        if (Potion > maxPotion) Potion = maxPotion;   // จำกัดไม่เกิน 3
        cPotion.text = "x" + Potion;
        audioSource.PlayOneShot(pickupitem,0.5f);
    }

    public void AddKnife(int amount)
    {
        Knife += amount;
        if (Knife > maxKnife) Knife = maxKnife;       // จำกัดไม่เกิน 5
        cKnife.text = "x" + Knife;
        audioSource.PlayOneShot(pickupitem, 0.5f);
    }




    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckpos.position, wallCheckSize, 0, wallLayer);
    }


    private void ProcessWallSlide()
    {
        if (!Onground & WallCheck() & moveX != 0)
        {
            isWallSliding = true;
            anim.SetBool("wallslide", true);
            
            rb.velocity =new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
            anim.SetBool("wallslide", false);
            
        }
    }
    private void ProcessWallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false ;
       
    }
    public void EndAttack()
    {
        isAttacking = false; // จบโจมตี สามารถเคลื่อนที่ได้อีก
        anim.SetTrigger("EndAttack");
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (damageTimer > 0f) return; // ถ้ายังไม่ครบ cooldown → ไม่โดนซ้ำ

        bool gotHit = false;
        float damage = 0f;

        if (collision.CompareTag("EnemyAttack"))
        {
            damage = 3f;
            gotHit = true;
        }
        else if (collision.CompareTag("Armmer Enemy Attack"))
        {
            damage = 4f;
            gotHit = true;
        }
        
        // … (อื่น ๆ เหมือนเดิม)

        if (gotHit)
        {
            Debug.Log("Hit by: " + collision.name);
            anim.SetTrigger("hurt");
            healthbar -= damage;
            audioSource.PlayOneShot(hurt);
            StartCoroutine(FlashRed());
            damageTimer = damageCooldown; // ✅ รีเซ็ต cooldown ทุกครั้ง
        }


        if (collision.gameObject.tag == "Enemy" && damageTimer <= 0f)
        {
            anim.SetTrigger("hurt");
            healthbar -= 2;
            
            Bossbat bossbat = collision.GetComponent<Bossbat>();
            if (bossbat != null)
            {
                healthbar -= 1;
            }
            BossBigG bossBigG = collision.GetComponent<BossBigG>();
            if (bossBigG != null)
            {
                healthbar -= 3;
            }
            audioSource.PlayOneShot(hurt);
            damageTimer = damageCooldown; // ✅ รีเซ็ตคูลดาวน์
            StartCoroutine(FlashRed());

            BossSamurai bossSamurai = collision.GetComponent<BossSamurai>();
            if (bossSamurai != null)
            {
                healthbar -= 2;
            }
            audioSource.PlayOneShot(hurt);
            damageTimer = damageCooldown; // ✅ รีเซ็ตคูลดาวน์
            StartCoroutine(FlashRed());
        }
        if (collision.gameObject.tag == "Samurai Attack" && damageTimer <= 0f)
        {
            Debug.Log("Hit by: " + collision.name);
            anim.SetTrigger("hurt");
            healthbar -= 4;
            StartCoroutine(FlashRed());
            audioSource.PlayOneShot(hurt);
            damageTimer = 0.3f; // ✅ สั้นลง ให้โดนได้อีกในจังหวะถัดไป
        }
        if (collision.gameObject.tag == "Trap" && damageTimer <= 0f)
        {
            anim.SetTrigger("hurt");
            healthbar -= 10;

            damageTimer = damageCooldown; // ✅ รีเซ็ตคูลดาวน์
            StartCoroutine(FlashRed());
            audioSource.PlayOneShot(hurt);
        }

        if (collision.gameObject.tag == "EnemyArrow" )
        {
            anim.SetTrigger("hurt");
            healthbar = healthbar - 5;
            StartCoroutine(FlashRed());
            audioSource.PlayOneShot(hurt);
            damageTimer = damageCooldown;
        }
        if (collision.gameObject.tag == "Fireball")
        {
            anim.SetTrigger("hurt");
            healthbar = healthbar - 8;
            StartCoroutine(FlashRed());
            audioSource.PlayOneShot(hurt);
            damageTimer = damageCooldown;
        }
        if (collision.gameObject.tag == "EnemyAssassinAttack" && damageTimer <= 0f)
        {
            anim.SetTrigger("hurt");
            healthbar = healthbar - 5;
            StartCoroutine(FlashRed());
            audioSource.PlayOneShot(hurt);
            damageTimer = damageCooldown;
        }
        if (collision.gameObject.tag == "SamuraiSlash" && damageTimer <= 0f)
        {
            anim.SetTrigger("hurt");
            healthbar = healthbar - 15;
            StartCoroutine(FlashRed());
            audioSource.PlayOneShot(hurt);
            damageTimer = damageCooldown;
        }
        if (collision.gameObject.tag == "Fireballboss" && damageTimer <= 0f)
        {
            anim.SetTrigger("hurt");
            healthbar = healthbar - 5;
            StartCoroutine(FlashRed());
            audioSource.PlayOneShot(hurt);
            damageTimer = 0.3f;
        }

        if (collision.gameObject.tag == "Deadzone")
        {
            anim.SetTrigger("hurt");
            healthbar = healthbar - 99;
            StartCoroutine(FlashRed());
            audioSource.PlayOneShot(hurt);
            damageTimer = damageCooldown;
        }

    }
    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    public void Dead()
    {
        if (isDead) return;
        isDead = true;

        if (anim != null)
            anim.SetTrigger("Dead");

        gameOverCoroutine = StartCoroutine(ShowGameOverPanel());

    }
    private IEnumerator ShowGameOverPanel()
    {
        yield return new WaitForSecondsRealtime(1f);

        if (!isDead) yield break; // ถ้าโหลดใหม่ไปแล้ว → หยุด Coroutine

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        isPaused = true;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;// ซ่อน Cursor
        Cursor.lockState = CursorLockMode.Confined;
        isPaused = false;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    public void RestartLevel()
    {
        if (gameOverCoroutine != null)
        {
            StopCoroutine(gameOverCoroutine);
            gameOverCoroutine = null;
        }

        // รีเซ็ตสถานะ
        isDead = false;
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        StartCoroutine(LoadSceneWithDelay());
    }
    private IEnumerator LoadSceneWithDelay()
    {
        audioSource.PlayOneShot(clickSound);
        if (fadeAnimator != null)
            fadeAnimator.SetTrigger("FadeOut");

        yield return new WaitForSecondsRealtime(fadeDuration);

        // โหลด Scene ก่อนปรับค่า Time.timeScale
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // หลังโหลด Scene รีเซ็ตค่า Time และ Cursor อีกครั้งเพื่อชัวร์
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;


    }


    public void GoToMenu()
    {
        Time.timeScale = 1f; // รีเซ็ตเวลา
        StartCoroutine(LoadMenuCoroutine());

    }
    private IEnumerator LoadMenuCoroutine()
    {
        audioSource.PlayOneShot(clickSound);
        if (fadeAnimator != null)
            fadeAnimator.SetTrigger("FadeOut");

        // รอ Fade เล่นแบบ RealTime
        yield return new WaitForSecondsRealtime(fadeDuration);

        // โหลด Scene
        SceneManager.LoadScene("Menu");
    }




    private void FixedUpdate()
    {
       Collider2D hitCollider = Physics2D.OverlapCircle(transform.position+footOffset,footRadius,groundLayerMask);
        Onground = hitCollider != null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckpos.position, wallCheckSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + footOffset, footRadius);
    }
  
}
