using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Minigame2
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("References")]
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private Transform ballSpawnPoint;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text timerText;
        [SerializeField] private GameObject panelWin, panelLose;

        private int score = 0;
        private float timeLeft = 60f;
        private bool isGameOver = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SpawnNewBall();
        }

        private void Update()
        {
            if (isGameOver) return;

            timeLeft -= Time.deltaTime;

            int minutes = Mathf.FloorToInt(timeLeft / 60);
            int seconds = Mathf.FloorToInt(timeLeft % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (timeLeft <= 0)
                EndGame(false);
        }

        public void AddScore()
        {
            score++;
            scoreText.text = "Score: " + score;

            scoreText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5);

            if (score >= 10)
            {
                EndGame(true);
            }
        }

        public void SpawnBall()
        {
            if(isGameOver) return;
            DOVirtual.DelayedCall(1f, () => SpawnNewBall());
        }

        private void SpawnNewBall()
        {
            if (isGameOver) return;

            Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
        }

        private void EndGame(bool win)
        {
            isGameOver = true;
            if(win)
            {
                panelWin.SetActive(true);
            }
            else
            {
                panelLose.SetActive(true);
            }

        }

        public bool IsGameOver()
        {
            return isGameOver;
        }
    }
}
