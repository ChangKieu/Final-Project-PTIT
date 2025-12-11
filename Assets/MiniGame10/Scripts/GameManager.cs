using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MiniGame10
{
    [System.Serializable]
    public class LevelData
    {
        public List<Sprite> listSprite;
        public string levelName;
    }

    public class GameManager : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private List<LevelData> levelDatas;
        [SerializeField] private CardController prefabCard;
        [SerializeField] private Transform cardContainer;
        [SerializeField] private Sprite backSprite;
        [SerializeField] private Sprite bg4Card, bg5Card, bg6Card;

        [Header("Grid")]
        [SerializeField] private GridLayoutGroup grid;

        [SerializeField] private GameObject winEffect;
        [SerializeField] private Text txtLevel, txtTime;
        private List<CardController> spawnedCards = new List<CardController>();
        private CardController lastWrongCard;

        private int nextNumber = 1;
        private int levelCardCount = 4;
        private int currentLevel = 0;

        private bool isGameOver = false;

        private int timeRemaining = 0;
        private Coroutine timerRoutine;

        void Start()
        {
            SetUp();
        }

        private void SetUp()
        {
            isGameOver = false;
            SetupGrid();
            CreateLevel();
            SetupTimer();
        }

        void SetupGrid()
        {
            currentLevel = PlayerPrefs.GetInt("Level", 0);
            if (currentLevel >= levelDatas.Count)
            {
                currentLevel = 0;
                PlayerPrefs.SetInt("Level", 0);
            }

            levelCardCount = levelDatas[currentLevel].listSprite.Count;
            txtLevel.text = "Màn " + (currentLevel + 1).ToString("D2") + ": " + levelDatas[currentLevel].levelName;

            if (levelCardCount == 4)
                SetCardContainer(bg4Card, 1);
            else if (levelCardCount == 5)
                SetCardContainer(bg5Card, 1);
            else
                SetCardContainer(bg6Card, 2);
        }

        void SetCardContainer(Sprite bgSprite, int constraintCount)
        {
            Image img = cardContainer.GetComponent<Image>();
            img.sprite = bgSprite;
            img.SetNativeSize();
            img.rectTransform.sizeDelta *= 0.65f;
            grid.constraintCount = constraintCount;
        }

        void CreateLevel()
        {
            nextNumber = 1;
            spawnedCards.Clear();

            List<int> nums = new List<int>();
            for (int i = 0; i < levelCardCount; i++)
                nums.Add(i);

            Shuffle(nums);

            foreach (int i in nums)
            {
                var card = Instantiate(prefabCard, cardContainer);
                card.number = i + 1;
                card.frontSprite = levelDatas[currentLevel].listSprite[i];
                card.backSprite = backSprite;
                spawnedCards.Add(card);
            }
        }

        // -------------------------
        // 🔥 TIMER SETUP
        // -------------------------
        void SetupTimer()
        {
            if (timerRoutine != null)
                StopCoroutine(timerRoutine);

            if (levelCardCount == 4) timeRemaining = 30;
            else if (levelCardCount == 5) timeRemaining = 45;
            else timeRemaining = 60;

            txtTime.text = timeRemaining + "s";

            timerRoutine = StartCoroutine(TimerCountdown());
        }

        IEnumerator TimerCountdown()
        {
            while (timeRemaining > 0 && !isGameOver)
            {
                yield return new WaitForSeconds(1f);

                timeRemaining--;
                txtTime.text = timeRemaining + "s";
            }

            if (!isGameOver && timeRemaining <= 0)
                Lose();
        }

        // -------------------------

        public void OnCardClicked(CardController card)
        {
            if (isGameOver) return;

            card.FlipOpen();

            if (card.number != nextNumber)
            {
                lastWrongCard = card;
                Invoke(nameof(CloseAllOpenNonLocked), 0.35f);
                return;
            }

            card.LockCard();
            nextNumber++;

            if (nextNumber > levelCardCount)
                OnWin();
        }

        private void CloseAllOpenNonLocked()
        {
            foreach (var c in spawnedCards)
            {
                if (c == null) continue;
                if (c.IsFlipped)
                {
                    c.FlipClose();
                }
            }

            lastWrongCard = null;
            nextNumber = 1;
        }

        void OnWin()
        {
            if (isGameOver) return;
            isGameOver = true;
            winEffect.SetActive(true);
            currentLevel++;
            PlayerPrefs.SetInt("Level", currentLevel);
            Invoke(nameof(NextLevel), 1f);
        }

        void Lose()
        {
            if (isGameOver) return;

            isGameOver = true;
            Debug.Log("YOU LOSE!");
            // popupLose.SetActive(true);
        }

        void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = Random.Range(i, list.Count);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }

        void NextLevel()
        {
            foreach (var card in spawnedCards)
                Destroy(card.gameObject);

            winEffect.SetActive(false);
            SetUp();
        }
    }
}
