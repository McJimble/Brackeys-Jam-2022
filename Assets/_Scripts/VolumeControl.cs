using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] string _volumeParameter = "MasterVolume";
    [SerializeField] AudioMixer _mixer;
    [SerializeField] Slider _slider;
    [SerializeField] float _multiplier = 30f;
 
    private bool _disableToggleEvent;

    private void Awake()
    {
        _slider.onValueChanged.AddListener(HandleSliderValueChanged);
    }


    private void OnDisable()
    {
        PlayerPrefs.SetFloat(_volumeParameter, _slider.value);
    }

    private void HandleSliderValueChanged(float value)
    {
        _mixer.SetFloat(_volumeParameter, Mathf.Log10(value) * _multiplier);

        if (value == 0)
        {
            _mixer.SetFloat(_volumeParameter, -80);
        }
        else if (value == 100)
        {
            _mixer.SetFloat(_volumeParameter, 10);
        }
      
    }

    private void Start()
    {
        _slider.value = PlayerPrefs.GetFloat(_volumeParameter, _slider.value);
    }
}
