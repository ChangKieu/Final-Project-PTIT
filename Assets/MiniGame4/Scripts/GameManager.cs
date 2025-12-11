using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MiniGame4
{
    [System.Serializable]
    public struct MiniGame4Data
    {
        public string title;
        public string titleRule;
        public string descriptionRule;
        public Sprite option1; 
        public Sprite option2;
    }

    public class GameManager : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private MiniGame4Data[] miniGame4Datas;

        [Header("UI")]
        [SerializeField] private Text txtTile;
        [SerializeField] private Text txtRule;
        [SerializeField] private Text txtDescription;

        [SerializeField] private Button btnOption1;
        [SerializeField] private Button btnOption2;

        [Header("Effects")]
        [SerializeField] private GameObject winEffect, homePanel;

        private int currentIndex = 0;
        private bool isAnswering = false;

        private string sceneName;

        private void Start()
        {
            sceneName = SceneManager.GetActiveScene().name;
            if (PlayerPrefs.GetInt("Menu", 0) == 0)
            {
                homePanel.SetActive(true);
            }
            else
            {
                PlayerPrefs.SetInt("Menu", 0);
                homePanel.SetActive(false);
                LoadQuestion();
            }
            LoadSceneManager.Instance.FadeIn();
        }

        private void LoadQuestion()
        {
            isAnswering = false;

            currentIndex = PlayerPrefs.GetInt($"MiniGame_{sceneName}_Progress", 0);

            if (currentIndex >= miniGame4Datas.Length)
            {
                PlayerPrefs.SetInt($"MiniGame_{sceneName}_Progress", 0);
                PlayerPrefs.Save();
            }

            var data = miniGame4Datas[currentIndex];

            txtTile.text = data.title;
            txtRule.text = "Nguyên tắc " + (currentIndex + 1) + ": " + data.titleRule;
            txtDescription.text = data.descriptionRule;

            btnOption1.image.sprite = data.option1; 
            btnOption2.image.sprite = data.option2; 

            winEffect.SetActive(false);
            Show(false);

            ShuffleButtons();

            btnOption1.onClick.RemoveAllListeners();
            btnOption2.onClick.RemoveAllListeners();

            btnOption1.onClick.AddListener(() => OnChoose(true));
            btnOption2.onClick.AddListener(() => OnChoose(false));
        }

        private void ShuffleButtons()
        {
            RectTransform r1 = btnOption1.GetComponent<RectTransform>();
            RectTransform r2 = btnOption2.GetComponent<RectTransform>();

            Vector2 temp = r1.anchoredPosition;
            r1.anchoredPosition = r2.anchoredPosition;
            r2.anchoredPosition = temp;


        }

        private void Show(bool value)
        {
            btnOption1.transform.GetChild(0).gameObject.SetActive(value);
            btnOption2.transform.GetChild(0).gameObject.SetActive(value);
        }

        private void OnChoose(bool isCorrect)
        {
            if (isAnswering) return;
            isAnswering = true;

            if (isCorrect)
            {
                winEffect.SetActive(true);
            }
            else
            {
                Show(true);
            }

            Invoke(nameof(NextQuestion), 1f);
        }

        private void NextQuestion()
        {
            currentIndex++;
            PlayerPrefs.SetInt($"MiniGame_{sceneName}_Progress", currentIndex);
            PlayerPrefs.Save();
            NextLevel();
        }

        private void LoadSceneWithTransition(bool isReplay)
        {
            PlayerPrefs.SetInt("Menu", isReplay ? 1 : 0);

            LoadSceneManager.Instance.LoadScene(sceneName);
        }

        public void NextLevel()
        {
            LoadSceneWithTransition(true);
        }
        public void LoadExit()
        {
            LoadSceneWithTransition(false);
        }
        public void LoadHome()
        {
            LoadSceneManager.Instance.LoadScene("Home");
        }
    }
}
