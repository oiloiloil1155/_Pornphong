using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject continuePanel;
    public GameObject TutorialPanel;


    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    private void Start()
    {
        ShowMainMenu(); // เริ่มที่เมนูหลัก

    }

    public void ShowTutorialMenu()
    {
        mainMenuPanel.SetActive(false);
        continuePanel.SetActive(false);
        TutorialPanel.SetActive(true);
        audioSource.PlayOneShot(clickSound);
    }
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        continuePanel.SetActive(false);
        TutorialPanel.SetActive(false);
       
    }

    public void ShowContinueMenu()
    {
        mainMenuPanel.SetActive(false);
        continuePanel.SetActive(true);
        TutorialPanel.SetActive(false);
        audioSource.PlayOneShot(clickSound);
    }

    public void QuitGame()
    {
        audioSource.PlayOneShot(clickSound);
        Application.Quit();
    }
}
