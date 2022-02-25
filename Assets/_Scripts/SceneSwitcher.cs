using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] int sceneToLoad = -1;
    Player player;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent<Player>(out player))
        {
            audioSource.Play();

            StartCoroutine(LoadNextScene());



        }
    }

    private IEnumerator LoadNextScene()
    {
        if (sceneToLoad < 0)
        {
            Debug.LogError("Scene to load not set.");
            yield break;
        }
        DontDestroyOnLoad(this.gameObject);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);


        Destroy(this.gameObject);


    }
}

  

  
