using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button continueButton;
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

    public void Options()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting");
    }

    
}
