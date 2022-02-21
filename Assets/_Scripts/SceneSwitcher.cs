using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    
    [SerializeField] int sceneToLoad = -1;
    Player player;

    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent<Player>(out player))
        {


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

  

  
