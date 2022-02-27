using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Stuff")]
    [SerializeField] Button newGameButton;
    [SerializeField] Button continueButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button quitButton;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;

    [Header("Special VFX")]
    [SerializeField] private float titleRotateSpeed;
    [SerializeField] private float titleRotAmount;
    [Space]
    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private float playerMoveAmount;
    [Space]
    [SerializeField] private GameObject titleObj;
    [SerializeField] private GameObject playerMainObj;
    [SerializeField] private GameObject continueMesh;
    [SerializeField] private GameObject optionsMesh;
    [SerializeField] private GameObject quitMesh;

    AudioSource audioSource;
    bool noSavedLevel = false;

    private Vector3 playerStartPos;
    private Vector3 playerDisplacement;

    private Quaternion titleStartRot;
    private Quaternion titleDisplaceRot;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("CurrentLevel") == 0)
        {
            continueButton.GetComponent<Image>().color = Color.gray;
            noSavedLevel = true;
        }

        playerStartPos = playerMainObj.transform.position;
        playerDisplacement = playerStartPos;
        playerDisplacement.y += playerMoveAmount;

        titleStartRot = titleObj.transform.rotation;
        titleDisplaceRot = Quaternion.Euler(titleStartRot.eulerAngles.x, titleStartRot.eulerAngles.y, titleStartRot.eulerAngles.z + titleRotAmount);

        continueMesh.SetActive(false);
        optionsMesh.SetActive(true);
        quitMesh.SetActive(false);
    }

    private void Update()
    {
        if (playerMainObj)
            playerMainObj.transform.position = Vector3.Lerp(playerStartPos, playerDisplacement, Mathf.PingPong(Time.time * playerMoveSpeed, 1f));

        if (titleObj)
            titleObj.transform.rotation = Quaternion.Lerp(titleStartRot, titleDisplaceRot, Mathf.PingPong(Time.time * titleRotateSpeed, 1f));
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

    public void OnPointerEnter(Button eventData)
    {
        Button hoverButton = eventData;

        if (hoverButton == continueButton || hoverButton == newGameButton)
        {
            continueMesh.SetActive(true);
            optionsMesh.SetActive(false);
            quitMesh.SetActive(false);
        }
        else if (hoverButton == optionsButton)
        {
            continueMesh.SetActive(false);
            optionsMesh.SetActive(true);
            quitMesh.SetActive(false);
        }
        else if (hoverButton == quitButton)
        {
            continueMesh.SetActive(false);
            optionsMesh.SetActive(false);
            quitMesh.SetActive(true);
        }
    }
}
