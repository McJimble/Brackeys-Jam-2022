using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    AudioSource audioSource;
    AudioClip winClip;
    [SerializeField] int sceneToLoad = -1;
    Player player;

    [Space]
    [SerializeField] private float exitTransitionTime = 1.6f;
    [SerializeField] private float enterTransitionTime = 1.6f;
    [SerializeField] private bool playEnterOnStart = true;

    [SerializeField] private Animator transitionAnimator;
    private int exitTransitionHash;
    private int enterTransitionHash;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        exitTransitionHash = Animator.StringToHash("ExitTransition");
        enterTransitionHash = Animator.StringToHash("EnterTransition");

        winClip = audioSource.clip;
    }

    private void Start()
    {
        if (playEnterOnStart)
            transitionAnimator.Play(enterTransitionHash);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out player))
        {
            StartCoroutine(LoadNextScene());
        }
    }

    private IEnumerator LoadNextScene()
    {
        float startTransitionTime = 1.1f;
        WaitForSeconds waitUntilTransition = new WaitForSeconds(startTransitionTime);
        WaitForSeconds waitUntilAnimDone = new WaitForSeconds(exitTransitionTime);
        TelevisionUI tv = FindObjectOfType<TelevisionUI>();
        Player player = FindObjectOfType<Player>();

        tv.PlayClipFromTV(winClip);
        tv.PlayHappyFace(-1, startTransitionTime);

        player.CharacterInputs.Disable();

        yield return waitUntilTransition;

        SceneEndTransition();

        yield return waitUntilAnimDone;

        if (sceneToLoad < 0)
        {
            Debug.LogError("Scene to load not set.");
            yield break;
        }
        LevelManager.Instance.NextLevel();

        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        
        Destroy(this.gameObject);
    }

    private void SceneEndTransition()
    {
        transitionAnimator.Play(exitTransitionHash);
    }
}

  

  
