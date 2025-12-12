using MiniGame4;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiniGame5
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private string[] listHint;
        [SerializeField] private string[] listAnswer;
        [SerializeField] private Text txtHint;
        [SerializeField] private GameObject letterPrefab;
        [SerializeField] private GameObject answerPrefab;

        [SerializeField] private Transform letterSpawnPoint;
        [SerializeField] private Transform answerSpawnPoint;

        private int currentIndex = 0;

        private List<LetterDrag> spawnedLetters = new List<LetterDrag>();
        private List<AnswerSlot> spawnedSlots = new List<AnswerSlot>();
        [SerializeField] private GameObject homePanel, winEffect;
        private string sceneName;
        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

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
                SpawnNewQuestion();
                LoadSceneManager.Instance.FadeInImage();
            }
        }

        public Transform LetterSpawnPointTransform()
        {
            return letterSpawnPoint;
        }

        private void SpawnNewQuestion()
        {
            ClearOldObjects();

            currentIndex = ProgressManager.GetProgress(sceneName);

            if (currentIndex >= listHint.Length)
            {
                ProgressManager.SetProgress(sceneName, 0);
                currentIndex = 0;
            }

            txtHint.text = listHint[currentIndex];
            string answer = listAnswer[currentIndex].Replace(" ", ""); 

            char[] letters = answer.ToCharArray();

            foreach (char c in letters)
            {
                GameObject slot = Instantiate(answerPrefab, answerSpawnPoint);
                AnswerSlot slotScript = slot.GetComponent<AnswerSlot>();
                slotScript.correctChar = c;
                spawnedSlots.Add(slotScript);
            }

            List<char> shuffled = new List<char>(letters);
            Shuffle(shuffled);

            foreach (char c in shuffled)
            {
                GameObject letterObj = Instantiate(letterPrefab, letterSpawnPoint);
                LetterDrag drag = letterObj.GetComponent<LetterDrag>();
                drag.SetLetter(c);
                drag.originalPositionInSpawn = drag.GetComponent<RectTransform>().anchoredPosition;

                spawnedLetters.Add(drag);
            }

            PositionLettersCentered();
        }

        private void PositionLettersCentered()
        {
            float spacing = 0;               
            float letterWidth = 64.4f;          

            int count = spawnedLetters.Count;
            float totalWidth = count * letterWidth + (count - 1) * spacing;

            float startX = -totalWidth / 2 + letterWidth / 2;

            for (int i = 0; i < count; i++)
            {
                RectTransform rt = spawnedLetters[i].GetComponent<RectTransform>();

                float x = startX + i * (letterWidth + spacing);

                rt.anchoredPosition = new Vector2(x, 0);
                spawnedLetters[i].originalPositionInSpawn = rt.anchoredPosition;
            }
        }


        private void ClearOldObjects()
        {
            foreach (Transform t in letterSpawnPoint)
                Destroy(t.gameObject);
            foreach (Transform t in answerSpawnPoint)
                Destroy(t.gameObject);

            spawnedLetters.Clear();
            spawnedSlots.Clear();
        }

        private void Shuffle(List<char> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = Random.Range(i, list.Count);
                char tmp = list[i];
                list[i] = list[rand];
                list[rand] = tmp;
            }
        }

        public void CheckWinCondition()
        {
            foreach (AnswerSlot slot in spawnedSlots)
            {
                if (!slot.IsCorrect())
                    return;
            }

            WinGame();
        }

        private void WinGame()
        {
            winEffect.SetActive(true);
            currentIndex++;
            ProgressManager.SetProgress(sceneName, currentIndex);
            if (currentIndex >= listHint.Length)
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
