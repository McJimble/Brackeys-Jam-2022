using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Controls background that appears during main gameplay using mostly UI elements.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class GameBackground : MonoBehaviour
{
    [Header("Background Stars")]
    [Tooltip("If this has a RotateRandomly component, it will be enabled when instantiated.")]
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private Transform[] backgroundStarPositions;
    [Tooltip("If this is greater than the above array's size, this value will be floored to its length at runtime.")]
    [SerializeField] private int numberOfStarsToSpawn;
    [Min(0f)]
    [SerializeField] private float minScaleFactor = 0.5f;
    [SerializeField] private float maxScaleFactor = 1.5f;

    [Header("Images")]
    [SerializeField] private RawImage backgroundImage;
    [SerializeField] private ScrollingRawImage[] cloudsOverlay;
    [Min(0f)]
    [SerializeField] private float cloudsOverlayFrameDuration = 0.2f;
    [Tooltip("All cloud overlay raw images are overrided to scroll by this speed in pixels per second.")]
    [SerializeField] private Vector2 cloudOverlayScrollSpeed;

    private Canvas mainCanvas;
    private GameObject[] spawnedStars;
    private float nextCloudFrameTimer = 0;  // Time until next frame is set active
    private int cloudsOverlayFrame = 0;     // Current frame in animation we are on.

    public Vector2 CloudOverlayScrollSpeed { get => cloudOverlayScrollSpeed;
        set
        {
            cloudOverlayScrollSpeed = value;
            foreach (var img in cloudsOverlay)
                img.ScrollingPerSecond = cloudOverlayScrollSpeed;
        }
    }

    private void OnValidate()
    {
        // Don't let someone set this below the min.
        maxScaleFactor = Mathf.Max(minScaleFactor, maxScaleFactor);
    }

    private void Awake()
    {
        // Canvas setup in case not done properly through inspector
        mainCanvas = GetComponent<Canvas>();
        mainCanvas.sortingOrder = System.Math.Min(mainCanvas.sortingOrder, -1);
        mainCanvas.worldCamera = Camera.main;
        mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;

        numberOfStarsToSpawn = System.Math.Clamp(numberOfStarsToSpawn, 0, backgroundStarPositions.Length);

        foreach (var img in cloudsOverlay)
            img.ScrollingPerSecond = cloudOverlayScrollSpeed;

        nextCloudFrameTimer = cloudsOverlayFrameDuration;
    }

    private void Start()
    {
        SetupStars();
    }

    private void Update()
    {
        // Every time specified amount of time elapses, go to next cloud frame but
        // continue scrolling all of them.
        if (nextCloudFrameTimer <= 0f)
        {
            NextCloudAnimationFrame();
        }

        nextCloudFrameTimer -= Time.deltaTime;
    }


    /// <summary>
    /// Spawns star prefabs in random positions in the background. Call this again to
    /// destroy any current ones.
    /// </summary>
    public void SetupStars()
    {
        // Destroy existing stars slowly in case that tanks FPS
        float secsPerStar = 0;
        if (spawnedStars != null)
        {
            foreach (var obj in spawnedStars)
            {
                obj.SetActive(false);
                Destroy(obj, secsPerStar);
                secsPerStar += 0.5f;
            }
        }

        // Random elements we can get.
        List<int> remainingPositions = new List<int>(backgroundStarPositions.Length);
        for (int i = 0; i < backgroundStarPositions.Length; ++i)
            remainingPositions.Add(i);

        // Get random transform from array of spawn positions, then spawn a star there.
        spawnedStars = new GameObject[numberOfStarsToSpawn];
        for (int i = 0; i < numberOfStarsToSpawn; ++i)
        {
            int randElement = Random.Range(0, remainingPositions.Count - 1);
            int randValue = remainingPositions[randElement];
            Debug.Log(randValue);
            remainingPositions.RemoveAt(randElement);

            spawnedStars[i] = Instantiate(starPrefab, backgroundStarPositions[randValue].position, Random.rotation, backgroundStarPositions[randValue]);
            spawnedStars[i].transform.localScale = spawnedStars[i].transform.localScale * Random.Range(minScaleFactor, maxScaleFactor);

            RotateRandomly randRot;
            if (spawnedStars[i].TryGetComponent<RotateRandomly>(out randRot))
            {
                randRot.SetIsRotating(true);
            }
        }
    }

    /// <summary>
    /// Disables current cloud image and enables the next one; accounts for looping back to
    /// the first frame.
    /// </summary>
    public void NextCloudAnimationFrame()
    {
        cloudsOverlay[cloudsOverlayFrame].Img.enabled = false;

        cloudsOverlayFrame = (cloudsOverlayFrame + 1) % cloudsOverlay.Length;

        cloudsOverlay[cloudsOverlayFrame].Img.enabled = true;

        nextCloudFrameTimer = cloudsOverlayFrameDuration;
    }
}
