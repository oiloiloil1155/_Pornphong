using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class NPCTalk : MonoBehaviour
{
    [Header("UI")]
    public GameObject playerCanvas;         // UI ของผู้เล่น 
    public GameObject[] talkPanels;         // ใส่ Canvas บทพูดทั้งหมดเรียงลำดับ
    public GameObject pressEText;           // ข้อความ "กด E เพื่อคุย"

    [Header("Player Control")]
    public Myscript1 playerScript;          // Script ของผู้เล่น (ไว้ปิดการควบคุม)

    private bool isPlayerInRange = false;
    private bool isTalking = false;
    private int currentTalkIndex = 0;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip Amytalk;

    void Start()
    {
        // ปิดทุกบทสนทนาเมื่อเริ่มเกม
        foreach (GameObject panel in talkPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        if (pressEText != null)
            pressEText.SetActive(false);
    }

    void Update()
    {
        if (!isPlayerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
            {
                StartDialogue();
            }
            else
            {
                NextDialogue();
            }
        }
    }

    void StartDialogue()
    {
        if (audioSource != null && Amytalk != null)
            audioSource.PlayOneShot(Amytalk);

        isTalking = true;
        currentTalkIndex = 0;

        // ปิด UI ผู้เล่นและควบคุมตัวละคร
        playerCanvas?.SetActive(false);
        if (playerScript != null) playerScript.enabled = false;

        // หยุดเวลาในเกม
        Time.timeScale = 0f;

        ShowCurrentPanel();
    }

    void NextDialogue()
    {
        // ปิด panel ปัจจุบัน
        if (talkPanels[currentTalkIndex] != null)
            talkPanels[currentTalkIndex].SetActive(false);

        currentTalkIndex++;

        // ถ้ามีหน้าถัดไป → แสดงต่อ
        if (currentTalkIndex < talkPanels.Length)
        {
            ShowCurrentPanel();
            if (audioSource != null && Amytalk != null)
                audioSource.PlayOneShot(Amytalk);
        }
        else
        {
            EndDialogue();
        }
    }

    void ShowCurrentPanel()
    {
        if (talkPanels[currentTalkIndex] != null)
            talkPanels[currentTalkIndex].SetActive(true);
    }

    void EndDialogue()
    {
        isTalking = false;
        Time.timeScale = 1f;

        // เปิดควบคุมตัวละครและ UI ผู้เล่นกลับมา
        if (playerScript != null) playerScript.enabled = true;
        playerCanvas?.SetActive(true);

        // ปิดทุก panel เผื่อไว้
        foreach (GameObject panel in talkPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        isPlayerInRange = true;
        pressEText?.SetActive(true);

        if (playerScript == null)
            playerScript = collision.GetComponent<Myscript1>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        isPlayerInRange = false;
        pressEText?.SetActive(false);
    }
}
