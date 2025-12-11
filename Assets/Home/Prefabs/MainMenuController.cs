using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance;

    [Header("Database")]
    [SerializeField] private MiniGameData[] database;

    [Header("UI")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private MiniGameItemUI itemPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadMenu();
    }

    private void LoadMenu()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var gameData in database)
        {
            MiniGameItemUI item = Instantiate(itemPrefab, contentParent);
            item.Setup(gameData);
        }
    }

    public void LoadMiniGame(MiniGameData data)
    {
        LoadSceneManager.Instance.LoadScene(data.sceneName);
    }
}
