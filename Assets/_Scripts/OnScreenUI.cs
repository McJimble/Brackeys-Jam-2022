using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnScreenUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI deathCounterText;
    public float elapsedTime;
    private TimeSpan timePlaying;
    public string timePlayingStr;
    private float deathCounter = 0;
    private void Start()
    {
        timerText.text = "Elapsed: 00:00.00";
        elapsedTime = PlayerPrefs.GetFloat("ElapsedTime");
        deathCounter = PlayerPrefs.GetFloat("Deaths");
    }


    private void Update()
    {
        elapsedTime += Time.deltaTime;
        timePlaying = TimeSpan.FromSeconds(elapsedTime);
        timePlayingStr = "Elapsed: " + timePlaying.ToString("mm':'ss'.'ff");
        timerText.text = timePlayingStr;
        deathCounterText.text = "Deaths: " + deathCounter.ToString();
    }

    public void IncreaseDeathCounter()
    {
        deathCounter++;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat("Deaths", deathCounter);
        PlayerPrefs.SetFloat("ElapsedTime", elapsedTime);
    }


}
