using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

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

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            SpawnNewQuestion();
        }

        public Transform LetterSpawnPointTransform()
        {
            return letterSpawnPoint;
        }

        private void SpawnNewQuestion()
        {
            ClearOldObjects();

            currentIndex = PlayerPrefs.GetInt("Level", 0);
            if (currentIndex >= listHint.Length)
            {
                currentIndex = 0;
                PlayerPrefs.SetInt("Level", 0);
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

            StartCoroutine(WinGame());
        }

        private IEnumerator WinGame()
        {
            Debug.Log("YOU WIN!");

            yield return new WaitForSeconds(1f);

            SpawnNewQuestion();
        }
    }
}
