using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SkipInto : MonoBehaviour
{
    [Header("Fade Settings")]
    public Animator fadeAnimator;  // ลาก FadePanel ที่มี Animator ใส่ช่องนี้ใน Inspector
    public float fadeDuration = 1f;

    [Header("Video Settings")]
    public VideoPlayer videoPlayer;     
    public string nextSceneName;        

    private bool isSkipped = false;


    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;
    void Start()
    {
        // เมื่อวิดีโอเล่นจบ ให้เรียกฟังก์ชัน EndCutscene()
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    void Update()
    {
        // ถ้าผู้เล่นกดปุ่ม E
        if (Input.GetKeyDown(KeyCode.E) && !isSkipped)
        {
            audioSource.PlayOneShot(clickSound);
            SkipCutscene();
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (!isSkipped)
        {
            EndCutscene();
        }
    }

    void SkipCutscene()
    {
        isSkipped = true;
        EndCutscene();
    }

    void EndCutscene()
    {
        
        if (fadeAnimator != null)
            fadeAnimator.SetTrigger("FadeOut");
        SceneManager.LoadScene(nextSceneName);
    }
}
