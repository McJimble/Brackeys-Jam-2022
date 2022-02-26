using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    AudioSource audioSource;
    bool noSavedLevel = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        if (PlayerPrefs.GetInt("CurrentLevel") == 0)
        {
            continueButton.GetComponent<Image>().color = Color.gray;
            noSavedLevel = true;
        }
    }
    public void StartGame()
    {
        audioSource.Play();
        mainMenu.SetActive(false);
        noSavedLevel = false;
        PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.DeleteKey("ElapsedTime");
        PlayerPrefs.DeleteKey("Deaths");
        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {  
        if (noSavedLevel) return;
        audioSource.Play();
        mainMenu.SetActive(false);
        SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentLevel"));
    }

    public void BackToMainMenu()
    {
        audioSource.Play();
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void Options()
    {
        audioSource.Play();
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void QuitGame()
    {
        audioSource.Play();
        Application.Quit();
        Debug.Log("Quitting");
    }

    
}
