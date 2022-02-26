using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;
    public float gameTimeScale = 1f;
    Player player;
    AudioSource audioSource;
    OnScreenUI onScreenUI;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject optionsMenu;

    private void Awake()
    {
        onScreenUI = FindObjectOfType<OnScreenUI>();
        audioSource = GetComponent<AudioSource>();
        player = FindObjectOfType<Player>();
    }
    private void Update()
    {
        if (player.CharacterInputs.Player.Pause.WasPerformedThisFrame())
        {
            if (isGamePaused)
            {
                //ResumeGame();
            }
            else
            {
                //PauseGame();
            }
        }
    }




    public void ResumeGame()
    {
        audioSource.Play();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        Time.timeScale = gameTimeScale;
        isGamePaused = false;   
    }



    public void OptionMenu()
    {
        audioSource.Play();
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    private void PauseGame()
    {

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isGamePaused = true;

    }
    public void BackToPauseMenu()
    {
        audioSource.Play();
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void RestartLevel()
    {
        ReloadScene();
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ResumeGame();
    }

    public void LoadMenu()
    {
        audioSource.Play();
        isGamePaused = false;
        Time.timeScale = 1;
        PlayerPrefs.SetInt("CurrentLevel", SceneManager.GetActiveScene().buildIndex);
        
        SceneManager.LoadScene(0);
    }

   
}
