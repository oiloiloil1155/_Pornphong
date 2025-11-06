using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUnlockManager : MonoBehaviour
{
    public static LevelUnlockManager instance;

    private const string LevelClearedKey = "LevelCleared";
    private int levelsCleared = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLevelProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CanPlayLevel(int levelIndex)
    {
        // ด่านที่ 1 เล่นได้เสมอ
        if (levelIndex == 1)
        {
            return true;
        }

        // ด่านอื่นๆ จะเล่นได้ถ้าเคลียร์ด่านก่อนหน้าแล้ว
        return levelIndex <= levelsCleared + 1;
    }

    public void MarkLevelAsCleared(int levelIndex)
    {
        if (levelIndex > levelsCleared)
        {
            levelsCleared = levelIndex;
            SaveLevelProgress();
            Debug.Log("Level " + levelIndex + " cleared! Next level unlocked.");
        }
    }

    public void ResetLevelProgress()
    {
        levelsCleared = 0;
        PlayerPrefs.SetInt(LevelClearedKey, levelsCleared);
        PlayerPrefs.Save();
        Debug.Log("Level progress reset! Only Level 1 is unlocked.");
    }

    private void SaveLevelProgress()
    {
        PlayerPrefs.SetInt(LevelClearedKey, levelsCleared);
        PlayerPrefs.Save();
    }

    private void LoadLevelProgress()
    {
        levelsCleared = PlayerPrefs.GetInt(LevelClearedKey, 0); // Default เป็น 0
        Debug.Log("Loaded level progress: " + levelsCleared + " levels cleared.");
    }

    // ฟังก์ชันสำหรับโหลดด่าน
    public void LoadLevel(int levelIndex)
    {
        if (CanPlayLevel(levelIndex))
        {
            SceneManager.LoadScene("Level_" + levelIndex);
        }
        else
        {
            Debug.LogWarning("Level " + levelIndex + " is locked!");
            // แสดงข้อความว่าด่านยังไม่ถูกปลดล็อก
        }
    }
    public void OntartClick()
    {
        SceneManager.LoadScene("Menu");
    }
}