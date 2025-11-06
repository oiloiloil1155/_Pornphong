using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSelectMenu : MonoBehaviour
{
    [Header("Fade Settings")]
    public Animator fadeAnimator;  
  
    public float fadeDuration = 1f;

    public Button[] levelButtons; 
    private int unlockedLevel;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    void Start()
    {
        // โหลดด่านที่ปลดล่าสุดจาก PlayerPrefs
        unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // ปลดล็อกปุ่มตามด่านที่เคยผ่าน
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i + 1 <= unlockedLevel)
                levelButtons[i].interactable = true;
            else
                levelButtons[i].interactable = false;
        }
    }
    public void LoadLevel(string levelName)
    {
        StartCoroutine(LoadSceneWithDelay(levelName));
    }


    public void ResetProgress()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();
    }
    private IEnumerator LoadSceneWithDelay(string levelName)
    {
        // สั่งให้ Animator เล่นอนิเมชัน FadeOut
        if (fadeAnimator != null)
            fadeAnimator.SetTrigger("FadeOut");
        audioSource.PlayOneShot(clickSound);

        // รอให้จบก่อนโหลดฉาก
        yield return new WaitForSecondsRealtime(fadeDuration);

        SceneManager.LoadScene(levelName);
    }

}
