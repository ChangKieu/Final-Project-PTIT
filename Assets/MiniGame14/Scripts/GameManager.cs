using UnityEngine;

namespace MiniGame14 
{

    [System.Serializable]
    public class PlayerLevel
    {
        public Sprite[] listSprite;   
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        [Header("Player System")]
        [SerializeField] private PlayerLevel[] playerLevels;
        [SerializeField] private PlayerController player;

        [Header("Obstacle")]
        public GameObject obstaclePrefab;
        public Sprite[] obstacleSprites;

        public RectTransform[] lanes;
        public float spawnInterval = 1.2f;

        [Header("Speed Control")]
        public float baseSpeed = 7f;
        public float speedIncrease = 0.5f;

        private float gameSpeed;
        private bool isGameOver = false;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {
            gameSpeed = baseSpeed;
            SpawnObstacle();
            RandomPlayerType();
        }

        private void Update()
        {
            if (!isGameOver)
                gameSpeed += speedIncrease * Time.deltaTime;
        }

        void RandomPlayerType()
        {
            int type = Random.Range(0, playerLevels.Length);
            Sprite spr = playerLevels[type].listSprite[Random.Range(0, playerLevels[type].listSprite.Length)];

            player.SetPlayer(type, spr);
        }

        void SpawnObstacle()
        {
            if (isGameOver) return;

            int[] laneOrder = { 0, 1, 2, 3 };
            ShuffleArray(laneOrder);

            for (int type = 0; type < 4; type++)
            {
                int laneIndex = laneOrder[type];

                GameObject obj = Instantiate(obstaclePrefab, lanes[laneIndex].parent);

                RectTransform rect = obj.GetComponent<RectTransform>();
                rect.anchoredPosition = lanes[laneIndex].anchoredPosition;

                ObstacleController oc = obj.GetComponent<ObstacleController>();
                oc.SetObstacleType(type, obstacleSprites[type]);
                oc.Init(gameSpeed);

            }
        }

        void ShuffleArray(int[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int rand = Random.Range(0, i + 1);
                int temp = array[i];
                array[i] = array[rand];
                array[rand] = temp;
            }
        }


        public void HitObstacle(int obstacleType)
        {
            if (obstacleType == player.GetPlayerType())
            {
                SpawnObstacle();
                RandomPlayerType();
            }
            else
            {
                isGameOver = true;
            }
        }
        public bool IsGameOver()
        {
            return isGameOver;
        }
    }
}


