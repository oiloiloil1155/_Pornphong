using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroGame : MonoBehaviour
{
    public float waitTime = 5f; // ระยะเวลาวิดีโอ (วินาที)

    void Start()
    {
        StartCoroutine(GoToMenu());
    }

    IEnumerator GoToMenu()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("Menu"); // ไป Scene 8
    }
}
