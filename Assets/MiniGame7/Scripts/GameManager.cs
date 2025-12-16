using MiniGame4;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiniGame7
{
    [System.Serializable]
    public struct AnswerData
    {
        public string[] answer;
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private string[] listQuestion;
        [SerializeField] private AnswerData[] listAnswers;
        [SerializeField] private Transform answerPos;
        [SerializeField] private GameObject answerPrefab;
        [SerializeField] private Sprite[] listHint;
        [SerializeField] private Image hintImg;
        [SerializeField] private Image checkImg;
        [SerializeField] private Sprite correctSprite, wrongSprite;
        [SerializeField] private GameObject winEffect;
        [SerializeField] private GameObject homePanel;

        [HideInInspector] public List<AnswerDrag> listAnswerDrag = new();
        [HideInInspector] public List<Vector2> slotPositions = new();

        private int currentIndex;
        private string sceneName;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

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

        public void SetUp()
        {
            foreach (Transform child in answerPos)
                Destroy(child.gameObject);

            listAnswerDrag.Clear();
            slotPositions.Clear();

            currentIndex = ProgressManager.GetProgress(sceneName);
            if (currentIndex >= listQuestion.Length)
                currentIndex = 0;

            hintImg.sprite = listHint[currentIndex];

            string[] answers = listAnswers[currentIndex].answer;

            for (int i = 0; i < answers.Length; i++)
            {
                GameObject obj = Instantiate(answerPrefab, answerPos);
                AnswerDrag drag = obj.GetComponent<AnswerDrag>();
                drag.SetupAnswer(answers[i], i);
                listAnswerDrag.Add(drag);
            }

            CreateSlots();
            RandomizeStartPositions();
        }

        private void CreateSlots()
        {
            float spacing = 175f;
            int count = listAnswerDrag.Count;
            float totalWidth = spacing * (count - 1);
            float startX = -totalWidth / 2f;

            for (int i = 0; i < count; i++)
            {
                slotPositions.Add(new Vector2(startX + spacing * i, 0));
            }
        }

        private void RandomizeStartPositions()
        {
            List<int> indices = new();
            for (int i = 0; i < slotPositions.Count; i++)
                indices.Add(i);

            for (int i = indices.Count - 1; i > 0; i--)
            {
                int r = Random.Range(0, i + 1);
                (indices[i], indices[r]) = (indices[r], indices[i]);
            }

            for (int i = 0; i < listAnswerDrag.Count; i++)
            {
                int slot = indices[i];
                listAnswerDrag[i].currentIndex = slot;
                listAnswerDrag[i].rect.anchoredPosition = slotPositions[slot];
            }
        }

        public void CheckAnswers()
        {
            for (int i = 0; i < listAnswerDrag.Count; i++)
            {
                if (listAnswerDrag[i].currentIndex != listAnswerDrag[i].correctIndex)
                {
                    AudioManager.Instance.PlayLose();
                    checkImg.sprite = wrongSprite;
                    return;
                }
            }

            AudioManager.Instance.PlayWin();
            checkImg.sprite = correctSprite;
            winEffect.SetActive(true);

            currentIndex++;
            ProgressManager.SetProgress(sceneName, currentIndex);

            if (currentIndex >= listQuestion.Length)
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
