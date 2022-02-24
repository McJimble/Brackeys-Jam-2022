using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;
    public float gameTimeScale = 1f;
    Player player;

    [SerializeField] GameObject pauseMenu;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }
    private void Update()
    {
        if (player.CharacterInputs.Player.Pause.WasPerformedThisFrame())
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }


    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = gameTimeScale;
        isGamePaused = false;
       
    }

    private void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isGamePaused = true;

    }

    public void LoadMenu()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        PlayerPrefs.SetInt("CurrentLevel", SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(0);
    }

   
}
