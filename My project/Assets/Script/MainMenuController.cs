using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Fade Settings")]
    public Animator fadeAnimator;  // ลาก FadePanel ที่มี Animator ใส่ช่องนี้ใน Inspector
    public float fadeDuration = 1f;


    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    public void NewGame()
    {
        StartCoroutine(LoadSceneWithDelay("IntoScene1"));
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();
        
    }

    public void Tutorial()
    {
        StartCoroutine(LoadSceneWithDelay("Tutorial Scene"));
    }

    public void QuitGame()
    {
        Debug.Log("Game Closed!");
        Application.Quit();
    }

    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        // สั่งให้ Animator เล่นอนิเมชัน FadeOut
        if (fadeAnimator != null)
            fadeAnimator.SetTrigger("FadeOut");
        audioSource.PlayOneShot(clickSound);
        // รอให้จบก่อนโหลดฉาก
        yield return new WaitForSecondsRealtime(fadeDuration);

        SceneManager.LoadScene(sceneName);
    }
}