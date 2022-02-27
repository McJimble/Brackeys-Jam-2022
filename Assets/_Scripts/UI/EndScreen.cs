using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI congratsText;
    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private float hueShiftSpeed = 0.01f;

    private void Awake()
    {
        timeText.text = "Elapsed: 00:00.00";
        float elapsedTime = PlayerPrefs.GetFloat("ElapsedTime");
        int deaths = (int)PlayerPrefs.GetFloat("Deaths");
        var timePlaying = TimeSpan.FromSeconds(elapsedTime);
        timeText.text = "Time: " + timePlaying.ToString("mm':'ss'.'ff") + " Deaths: " + deaths;

        
        PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.DeleteKey("ElapsedTime");
        PlayerPrefs.DeleteKey("Deaths");
    }

    private void Update()
    {
        float h, s, v;
        Color.RGBToHSV(timeText.color, out h, out s, out v);

        h = (h + (hueShiftSpeed * Time.deltaTime)) % 360f;

        timeText.color = Color.HSVToRGB(h, s, v);
    }
}
