using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
    [SerializeField] private string[] levelNames;

    [Header("TEMPORARY")]
    [SerializeField] private int debugOverrideCurrentLevel;

    private int currentLevel = 1;
    private int currentLevelName;

    public float TimeSpentInCurrentLevel { get; private set; }
    public int LevelNumber { get => currentLevel; }
    public string LevelName { get => levelNames[currentLevel - 1]; }

    public static Scene GetSceneFromLevelNumber(int levelNumber)
    {
        return SceneManager.GetSceneByBuildIndex(levelNumber + MAIN_LEVELS_FIRST_BUILD_INDEX - 1);
    }

    public static int GetLevelNumberFromCurrentScene()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);

        TimeSpentInCurrentLevel = 0f;
        currentLevel = GetLevelNumberFromCurrentScene();

        if (currentLevel >= SceneManager.sceneCountInBuildSettings)
        {
            currentLevel = debugOverrideCurrentLevel;
        }
    }

    private void Start()
    {

    }

    private void Update()
    {
        TimeSpentInCurrentLevel += Time.deltaTime;
    }

    public void RespawnPlayer(Player player)
    {
        StartCoroutine(RespawnPlayerSequence(player));
    }

    private IEnumerator RespawnPlayerSequence(Player player)
    {
        float timeElapsed = 0;
        player.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        player.transform.position = player.SpawnPoint.position;

        player.gameObject.SetActive(true);
        player.AttachedMotor.AttachedRB.velocity = Vector3.zero;
        player.CharacterInputs.Disable();

        player.transform.forward = Vector3.left;
        yield return new WaitForSeconds(.75f);
        while (timeElapsed < .5f)
        {

            player.transform.position = Vector3.Lerp(player.SpawnPoint.position, player.ShovePoint.position, timeElapsed / .5f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        player.transform.position = player.ShovePoint.position;
        player.CharacterInputs.Enable();
    }

    public void NextLevel()
    {
        ++currentLevel;
    }
}
