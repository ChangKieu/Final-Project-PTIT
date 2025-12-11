using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGame8
{
    [System.Serializable]
    public struct DiffInfo
    {
        public Vector2 position;   
        public Vector2 size;       
    }

    [System.Serializable]
    public class LevelData
    {
        public Sprite imageLeft;
        public Sprite imageRight;

        public DiffInfo[] differences; 
    }


    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private LevelData[] levels;

        [SerializeField] private Image imgLeft;
        [SerializeField] private Image imgRight;

        [SerializeField] private GameObject diffPointPrefab;
        [SerializeField] private Transform pointsLeftParent;
        [SerializeField] private Transform pointsRightParent;

        [SerializeField] private Transform starContainer;
        [SerializeField] private GameObject winEffect;

        private int currentLevel = 0;
        private int foundCount = 0;
        private int totalPoints = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            LoadLevel();
        }

        public void LoadLevel()
        {
            winEffect.SetActive(false);
            currentLevel = PlayerPrefs.GetInt("Level", 0);
            if (currentLevel >= levels.Length)
            {
                currentLevel = 0;
                PlayerPrefs.SetInt("Level", 0);
            }

            foundCount = 0;

            for (int i = 0; i < starContainer.childCount; i++)
                starContainer.GetChild(i).GetChild(0).gameObject.SetActive(false);

            var data = levels[currentLevel];
            imgLeft.sprite = data.imageLeft;
            imgRight.sprite = data.imageRight;

            foreach (Transform t in pointsLeftParent) Destroy(t.gameObject);
            foreach (Transform t in pointsRightParent) Destroy(t.gameObject);

            totalPoints = data.differences.Length;

            CreateDiffPoints(data);
        }

        private void CreateDiffPoints(LevelData data)
        {
            Debug.Log(data.differences.Length);
            foreach (var diff in data.differences)
            {
                // ------ LEFT ------
                var left = Instantiate(diffPointPrefab, pointsLeftParent);
                var leftRT = left.GetComponent<RectTransform>();

                leftRT.anchoredPosition = diff.position;
                leftRT.sizeDelta = diff.size;

                var leftPoint = left.GetComponent<DifferencePoint>();
                leftPoint.Init();

                // ------ RIGHT ------
                var right = Instantiate(diffPointPrefab, pointsRightParent);
                var rightRT = right.GetComponent<RectTransform>();

                Vector2 rightPos = diff.position;
                rightPos.x += 540f;   

                rightRT.anchoredPosition = rightPos;
                rightRT.sizeDelta = diff.size;

                var rightPoint = right.GetComponent<DifferencePoint>();
                rightPoint.Init();

                leftPoint.pair = rightPoint;
                rightPoint.pair = leftPoint;
            }
        }



        public void FoundPoint()
        {
            foundCount++;

            if (foundCount - 1 < starContainer.childCount)
                starContainer.GetChild(foundCount - 1).GetChild(0).gameObject.SetActive(true);

            if (foundCount >= totalPoints)
            {
                Debug.Log("WIN LEVEL!");
                NextLevel();
            }
        }

        private void NextLevel()
        {
            winEffect.SetActive(true);
            currentLevel++;
            if (currentLevel >= levels.Length)
            {
                return;
            }

            DOVirtual.DelayedCall(1f, () =>
            {
                PlayerPrefs.SetInt("Level", currentLevel);
                LoadLevel();
            });
        }
    }

}
