using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameProgress : MonoBehaviour
{
    private const string LEVEL_KEY = "UnlockedLevel";

    // คืนค่าด่านที่ปลดล่าสุด (เริ่มต้น = 1)
    public static int GetUnlockedLevel()
    {
        return PlayerPrefs.GetInt(LEVEL_KEY, 1);
    }

    // ปลดล็อกด่านถัดไป
    public static void UnlockNextLevel(int currentLevel)
    {
        int unlocked = GetUnlockedLevel();
        if (currentLevel >= unlocked)
        {
            PlayerPrefs.SetInt(LEVEL_KEY, currentLevel + 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // รีเซ็ตความคืบหน้า
    public static void ResetProgress()
    {
        PlayerPrefs.SetInt(LEVEL_KEY, 1);
        PlayerPrefs.Save();
    }
}
