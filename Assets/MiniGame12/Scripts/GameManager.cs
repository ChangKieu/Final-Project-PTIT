using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiniGame12
{
    [System.Serializable]
    public class LevelData
    {
        public string txtHint;
        public string[] listHint;
        public string answer;
    }

    public class GameManager : MonoBehaviour
    {
        [Header("Level Setup")]
        [SerializeField] private LevelData[] levels;

        [Header("UI")]
        [SerializeField] private Text txtHint;
        [SerializeField] private GameObject hintPrefab;
        [SerializeField] private Transform hintParent;
        [SerializeField] private InputField inputAnswer;
        [SerializeField] private Button btnCheck;

        private int currentLevel;

        [SerializeField] private GameObject homePanel, winEffect;
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
                SetUp();
                LoadSceneManager.Instance.FadeInImage();
            }
        }
        private void SetUp()
        {
            LoadLevel();
            btnCheck.onClick.AddListener(CheckAnswer);
        }
        private void LoadLevel()
        {
            currentLevel = ProgressManager.GetProgress(sceneName);

            if (currentLevel >= levels.Length)
            {
                ProgressManager.SetProgress(sceneName, 0);
                currentLevel = 0;
            }

            foreach (Transform child in hintParent)
                Destroy(child.gameObject);

            txtHint.text = levels[currentLevel].txtHint;

            foreach (var hint in levels[currentLevel].listHint)
            {
                GameObject obj = Instantiate(hintPrefab, hintParent);
                Text txt = obj.GetComponentInChildren<Text>();
                if (txt != null)
                    txt.text = hint;
            }

            inputAnswer.text = "";
        }

        private void CheckAnswer()
        {
            string userInput = inputAnswer.text;
            string correct = levels[currentLevel].answer;

            userInput = Normalize(userInput);
            correct = Normalize(correct);

            Debug.Log($"User: {userInput} - Correct: {correct}");
            if (userInput == correct)
            {
                Next();
            }
            else
            {
                Debug.Log("Wrong");
            }
        }

        private string Normalize(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";

            s = s.ToLowerInvariant();

            string normalizedFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (char c in normalizedFormD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            string withoutDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

            string cleaned = Regex.Replace(withoutDiacritics, @"[^a-z0-9]+", "");

            return cleaned;
        }

        private void Next()
        {
            winEffect.SetActive(true);

            currentLevel++;
            ProgressManager.SetProgress(sceneName, currentLevel);
            if (currentLevel >=levels.Length)
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
