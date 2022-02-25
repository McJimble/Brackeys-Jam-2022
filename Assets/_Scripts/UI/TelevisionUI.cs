using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Canvas), typeof(AudioSource))]
public class TelevisionUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ScrollingRawImage backgroundImage;
    [SerializeField] private RawImage backgroundColorImage;
    [Space]
    [SerializeField] private RawImage sadFace;
    [SerializeField] private RawImage normalFace;
    [SerializeField] private RawImage happyFace;
    [Space]
    [SerializeField] private TextMeshProUGUI levelTitleText;
    [SerializeField] private TextMeshProUGUI levelNumberText;
    [Space]
    [SerializeField] private Light tvEmissionLight;
    [Min(0f)]
    [SerializeField] private float minEmissionIntensity = 60f;
    [SerializeField] private float maxEmissionIntensity = 100f;
    [SerializeField] private float emissionIntensitySpeed = 10f;

    public ScrollingRawImage BackgroundImage { get => backgroundImage; }

    private Canvas mainCanvas;
    private AudioSource audioSrc;

    private void OnValidate()
    {
        maxEmissionIntensity = Mathf.Max(minEmissionIntensity, maxEmissionIntensity);
    }

    private void Awake()
    {
        mainCanvas = GetComponent<Canvas>();
        audioSrc = GetComponent<AudioSource>();

        mainCanvas.renderMode = RenderMode.WorldSpace;
    }

    private void Start()
    {
        //InitGenericDisplay();
    }

    private void Update()
    {
        tvEmissionLight.intensity = Mathf.PingPong(Time.time * emissionIntensitySpeed, maxEmissionIntensity - minEmissionIntensity) + minEmissionIntensity;
    }

    public void InitGenericDisplay()
    {
        sadFace.transform.gameObject.SetActive(false);
        happyFace.transform.gameObject.SetActive(false);
        normalFace.transform.gameObject.SetActive(true);

        levelTitleText.text = LevelManager.Instance.LevelName;
        levelNumberText.text = LevelManager.Instance.LevelNumber.ToString("00");
    }

    public void PlayClipFromTV(AudioClip clip)
    {
        audioSrc.clip = clip;
        audioSrc.Play();
    }
}
