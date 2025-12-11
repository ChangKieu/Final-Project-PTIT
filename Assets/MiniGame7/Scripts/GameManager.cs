using UnityEngine;
using System.Collections.Generic;
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
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private GameObject winEffect;

        [HideInInspector] public List<AnswerDrag> listAnswerDrag = new List<AnswerDrag>();

        private int currentIndex = 0;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            SetUp();
        }

        public void SetUp()
        {
            foreach (Transform child in answerPos)
                Destroy(child.gameObject);

            listAnswerDrag.Clear();

            currentIndex = PlayerPrefs.GetInt("Level", 0);

            if (currentIndex >= listQuestion.Length)
            {
                currentIndex = 0;
                PlayerPrefs.SetInt("Level", 0);
            }

            hintImg.sprite = listHint[currentIndex];
            string[] answers = listAnswers[currentIndex].answer;

            for (int i = 0; i < answers.Length; i++)
            {
                GameObject obj = Instantiate(answerPrefab, answerPos);
                AnswerDrag drag = obj.GetComponent<AnswerDrag>();
                drag.SetupAnswer(answers[i], i);
                listAnswerDrag.Add(drag);
            }

            ShuffleAnswers();
        }

        public void AlignAnswers()
        {
            float spacing = 175f;
            int count = listAnswerDrag.Count;

            float totalWidth = spacing * (count - 1);
            float startX = -totalWidth / 2f;

            for (int i = 0; i < count; i++)
            {
                listAnswerDrag[i].transform.localPosition =
                    new Vector3(startX + spacing * i, 0, 0);

                listAnswerDrag[i].currentIndex = i;
            }
        }

        public void ShuffleAnswers()
        {
            for (int i = listAnswerDrag.Count - 1; i > 0; i--)
            {
                int rand = Random.Range(0, i + 1);
                var temp = listAnswerDrag[i];
                listAnswerDrag[i] = listAnswerDrag[rand];
                listAnswerDrag[rand] = temp;
            }

            for (int i = 0; i < listAnswerDrag.Count; i++)
            {
                listAnswerDrag[i].transform.SetSiblingIndex(i);
            }

            AlignAnswers();
        }

        public void UpdateIndexOrder()
        {
            listAnswerDrag.Sort((a, b) =>
                a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

            AlignAnswers();
        }

        public void CheckAnswers()
        {
            for (int i = 0; i < listAnswerDrag.Count; i++)
            {
                if (i != listAnswerDrag[i].correctIndex)
                {
                    characterAnimator.SetBool("isCorrect", false);
                    return;
                }
            }

            characterAnimator.SetBool("isCorrect", true);
            winEffect.SetActive(true);
        }
    }
}
