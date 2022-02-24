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
    bool noSavedLevel = false;

    
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
        noSavedLevel = false;
        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        if (noSavedLevel) return;
        SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentLevel"));
    }

    public void BackToMainMenu()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void Options()
    {
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting");
    }

    
}
