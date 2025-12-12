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
        [SerializeField] private GameObject winEffect;

        private int currentIndex = 0;
        private bool isAnswering = false;

        [SerializeField] private GameObject homePanel;
        private string sceneName;

        private void Start()
        {
            sceneName = SceneManager.GetActiveScene().name;
            if (PlayerPrefs.GetInt("Menu" + sceneName, 0) == 0)
            {
                homePanel.SetActive(true);
                LoadSceneManager.Instance.FadeIn();
            }
            else
            {
                PlayerPrefs.SetInt("Menu" + sceneName, 0);
                homePanel.SetActive(false);
                LoadQuestion();
                LoadSceneManager.Instance.FadeInImage();
            }
        }

        private void LoadQuestion()
        {
            isAnswering = false;

            currentIndex = ProgressManager.GetProgress(sceneName);

            if (currentIndex >= miniGame4Datas.Length)
            {
                ProgressManager.SetProgress(sceneName, 0);
                currentIndex = 0;
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
            int rand = Random.Range(0, 2);
            if (rand == 0) return;
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
            ProgressManager.SetProgress(sceneName, currentIndex);
            if (currentIndex >= miniGame4Datas.Length)
            {
                ProgressManager.SetDone(sceneName);
                LoadSceneManager.Instance.ShowPanelDone();
                return;
            }
            NextLevel();
        }

        public void NextLevel()
        {
            PlayerPrefs.SetInt("Menu" + sceneName, 1);

            LoadSceneManager.Instance.LoadSceneImg(sceneName);
        }
        public void LoadExit()
        {
            PlayerPrefs.SetInt("Menu" + sceneName, 0);

            LoadSceneManager.Instance.LoadScene(sceneName);
        }
        public void LoadHome()
        {
            LoadSceneManager.Instance.LoadScene("Home");
        }
    }
}
