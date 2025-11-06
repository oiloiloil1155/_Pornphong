using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTutorial : MonoBehaviour
{
    [Header("Fade Settings")]
    public Animator fadeAnimator;
    public float fadeDuration = 1f;

    [Header("Player Control")]
    public Myscript1 playerScript;
    public GameObject pressEText;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip Amytalk;

    [Header("UI")]
    public GameObject winPanel;

    private bool isPlayerInRange = false;
    private bool hasWon = false;

    void Update()
    {

        if (!isPlayerInRange || hasWon) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            hasWon = true;
            Cursor.visible = true;
            if (winPanel != null)
            {
                winPanel.SetActive(true);
                Time.timeScale = 0f;

                if (playerScript != null)
                    playerScript.enabled = false;

                if (audioSource != null && Amytalk != null)
                    audioSource.PlayOneShot(Amytalk);
            }
        }
    }

    public void Next()
    {
        Time.timeScale = 1f;
        if (playerScript != null)
            playerScript.enabled = true;

        StartCoroutine(LoadSceneWithDelay("IntoScene1"));
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        if (playerScript != null)
            playerScript.enabled = true;

        StartCoroutine(LoadSceneWithDelay("Menu"));
    }

    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        if (fadeAnimator != null)
            fadeAnimator.SetTrigger("FadeOut");

        yield return new WaitForSecondsRealtime(fadeDuration);
        SceneManager.LoadScene(sceneName);
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
