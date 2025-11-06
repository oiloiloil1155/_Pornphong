using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    [Header("Fade Settings")]
    public Animator fadeAnimator;  // ลาก FadePanel ที่มี Animator ใส่ช่องนี้ใน Inspector
    public float fadeDuration = 1f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;
    public void Next()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneWithDelay("IntoScene2"));
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneWithDelay("Menu"));
    }

    private IEnumerator LoadSceneWithDelay(string sceneName)
    {

        
        audioSource.PlayOneShot(clickSound);
        // สั่งให้ Animator เล่นอนิเมชัน FadeOut
        if (fadeAnimator != null)
            fadeAnimator.SetTrigger("FadeOut");

        // รอให้จบก่อนโหลดฉาก
        yield return new WaitForSecondsRealtime(fadeDuration);

        SceneManager.LoadScene(sceneName);
    }
}
