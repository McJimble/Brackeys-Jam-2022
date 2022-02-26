using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Canvas), typeof(AudioSource))]
public class TelevisionUI : MonoBehaviour
{
    [SerializeField] private ScrollingRawImage backgroundImage;
    [SerializeField] private RawImage backgroundColorImage;
    [Space]
    [SerializeField] private GameObject faceIconMask;
    [SerializeField] private RawImage sadFace;
    [SerializeField] private RawImage normalFace;
    [SerializeField] private RawImage happyFace;
    [Min(0f)]
    [SerializeField] private float imgBlinkSpeed = 0.18f;
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
    public float ImgBlinkSpeed { get => imgBlinkSpeed; set => imgBlinkSpeed = value; }

    private Canvas mainCanvas;
    private AudioSource audioSrc;
    private RawImage activeImage;
    private bool activeImageVisible = true;

    private void OnValidate()
    {
        maxEmissionIntensity = Mathf.Max(minEmissionIntensity, maxEmissionIntensity);
    }

    private void Awake()
    {
        mainCanvas = GetComponent<Canvas>();
        audioSrc = GetComponent<AudioSource>();

        activeImage = normalFace;
    }

    private void Start()
    {
        InitGenericDisplay();
    }

    private void Update()
    {
        tvEmissionLight.intensity = Mathf.PingPong(Time.time * emissionIntensitySpeed, maxEmissionIntensity - minEmissionIntensity) + minEmissionIntensity;


    }

    public void InitGenericDisplay()
    {
        ResetToNormalFace();

        levelTitleText.text = LevelManager.Instance.LevelName;
        levelNumberText.text = LevelManager.Instance.LevelNumber.ToString("00");
    }

    public void PlaySadFace(float dispTime)
    {
        StopAllCoroutines();
        StartCoroutine(SadFaceRoutine(dispTime));
    }

    public void PlayHappyFace()
    {
        StopAllCoroutines();
        StartCoroutine(HappyFaceRoutine());
    }

    public void ResetToNormalFace()
    {
        sadFace.transform.gameObject.SetActive(false);
        happyFace.transform.gameObject.SetActive(false);
        normalFace.transform.gameObject.SetActive(true);
    }

    private IEnumerator SadFaceRoutine(float dispTime)
    {
        WaitForSeconds waitTime = new WaitForSeconds(dispTime);

        SetActiveImage(sadFace);
        sadFace.transform.gameObject.SetActive(true);
        happyFace.transform.gameObject.SetActive(false);
        normalFace.transform.gameObject.SetActive(false);

        yield return waitTime;

        sadFace.transform.gameObject.SetActive(false);
        happyFace.transform.gameObject.SetActive(false);
        normalFace.transform.gameObject.SetActive(true);
        SetActiveImage(normalFace);
    }

    private IEnumerator HappyFaceRoutine(float dispTime = -1f, float timeUntilBlink = -1f)
    {
        sadFace.transform.gameObject.SetActive(false);
        normalFace.transform.gameObject.SetActive(false);
        happyFace.transform.gameObject.SetActive(true);
        SetActiveImage(happyFace);


        if (timeUntilBlink >= 0f)
        {
            WaitForSeconds waitUntilBlink = new WaitForSeconds(timeUntilBlink);
            yield return waitUntilBlink;
        }

        if (dispTime >= 0f)
        {
            WaitForSeconds waitTime = new WaitForSeconds(dispTime);
            yield return waitTime;
        }

        happyFace.transform.gameObject.SetActive(false);
        normalFace.transform.gameObject.SetActive(true);
        SetActiveImage(normalFace);
    }

    private void SetActiveImage(RawImage img)
    {
        activeImage = img;
        activeImageVisible = true;
    }

    public void PlayClipFromTV(AudioClip clip)
    {
        audioSrc.clip = clip;
        audioSrc.Play();
    }
}
