using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls level-specific scripting among typical game manager things.
/// 
/// Doesn't do DontDestroyOnLoad because that can get poopy and confusing.
/// </summary>
public class LevelManager : MonoBehaviour
{
    // All levels should be ordered sequentially in build settings, starting at this index.
    public static int MAIN_LEVELS_FIRST_BUILD_INDEX = 1;

    // Singleton stuff
    private static LevelManager instance;
    public static LevelManager Instance { get => instance; }

    [Header("Required")]
    [SerializeField] private int levelNumber;
    [SerializeField] private string levelName;
    public float TimeSpentInCurrentLevel { get; private set; }
    public int LevelNumber { get => levelNumber; }
    public string LevelName { get => levelName; }

    public static Scene GetSceneFromLevelNumber(int levelNumber)
    {
        return SceneManager.GetSceneByBuildIndex(levelNumber + MAIN_LEVELS_FIRST_BUILD_INDEX - 1);
    }

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
        {
            Debug.LogError("Destroying extra GameManager component; please make sure this causes no errors.");
            Destroy(this);
        }

        TimeSpentInCurrentLevel = 0f;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        TimeSpentInCurrentLevel += Time.deltaTime;
    }
}
