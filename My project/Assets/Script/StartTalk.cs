using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartTalk : MonoBehaviour
{
    [Header("UI")]
    public GameObject playerCanvas;         // UI ของผู้เล่น (พลังชีวิต, ปุ่ม ฯลฯ)
    public GameObject[] talkPanels;         // ใส่ Canvas บทพูดทั้งหมดเรียงลำดับ

    [Header("Player Control")]
    public Myscript1 playerScript;          // Script ของผู้เล่น (ไว้ปิดการควบคุม)

    private bool isTalking = false;
    private int currentTalkIndex = 0;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip Amytalk;

    void Start()
    {
        // ปิดทุก panel ก่อน
        foreach (GameObject panel in talkPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
     

        // เริ่มบทพูดทันที
        StartCoroutine(StartAfterFrame());
    }

    IEnumerator StartAfterFrame()
    {
        
        yield return null;
        StartDialogue();
    }

    void Update()
    {
        // ถ้ากำลังอยู่ในบทพูด → รอให้กด E เพื่อข้ามไป panel ต่อไป
        if (isTalking && Input.GetKeyDown(KeyCode.E))
        {
            NextDialogue();
           
        }
    }

    void StartDialogue()
    {
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
}

