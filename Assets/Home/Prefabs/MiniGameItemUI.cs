using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGameItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text txtProgress;

    private MiniGameData data;

    public void Setup(MiniGameData gameData)
    {
        data = gameData;
        icon.sprite = gameData.icon;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnClick);

        int progress = ProgressManager.GetProgress(gameData.sceneName);
        int done = ProgressManager.GetDone(gameData.sceneName);

        if(done > 0)
        {
            progressSlider.gameObject.SetActive(false);
            txtProgress.text = "Done";
            return;
        }

        if (gameData.totalLevels > 0)
        {
            progressSlider.maxValue = gameData.totalLevels;
            progressSlider.value = progress;
            txtProgress.text = $"{progress}/{gameData.totalLevels}";
        }
        else
        {
            progressSlider.maxValue = 1;
            progressSlider.value = 0;
            txtProgress.text = progress.ToString();
        }


    }

    public void OnClick()
    {
        MainMenuController.Instance.LoadMiniGame(data);
    }
}
