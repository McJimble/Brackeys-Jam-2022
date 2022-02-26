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
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject optionsMenu;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
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
        //audioSource.Play();
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

    public void LoadMenu()
    {
        audioSource.Play();
        isGamePaused = false;
        Time.timeScale = 1;
        PlayerPrefs.SetInt("CurrentLevel", SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(0);
    }

   
}
