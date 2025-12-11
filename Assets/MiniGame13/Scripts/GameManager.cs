using UnityEngine;
using UnityEngine.UI;

namespace MiniGame13
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Ins;

        [Header("Setup")]
        public Transform spawnPosBall;
        public GameObject ballPrefab;

        [Header("Refs")]
        public PlayerController player;
        public BotController bot;
        public Text txtScore;

        private int scorePlayer = 0;
        private int scoreBot = 0;

        private int lastWinner = 1;
        private GameObject currentBall;
        private bool isGameOver = false;
        private void Awake()
        {
            Ins = this;
        }

        private void Start()
        {
            UpdateScoreText();
            SpawnBall();
        }

        public void SpawnBall()
        {
            if (isGameOver) return;
            GameObject ball = Instantiate(ballPrefab, spawnPosBall.position, Quaternion.identity);
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();

            currentBall = ball;

            Vector3 dir;

            if (lastWinner == 0)
                dir = Vector3.right;
            else if (lastWinner == 1)
                dir = Vector3.left; 
            else
                dir = Vector3.right;

            rb.linearVelocity = dir * 4f + Vector3.up * 2f;

            player.ball = ball.transform;
            bot.ball = ball.transform;
        }

        public void PlayerLose()
        {
            scoreBot++;
            lastWinner = -1;
            CheckEnd();
            ResetRound();
        }

        public void PlayerWin()
        {
            scorePlayer++;
            lastWinner = 1;
            CheckEnd();
            ResetRound();
        }

        void ResetRound()
        {
            player.ResetPos();
            bot.ResetPos();
            UpdateScoreText();

            Destroy(currentBall);

            Invoke(nameof(SpawnBall), 1f);
        }

        void UpdateScoreText()
        {
            txtScore.text = scoreBot + " - " + scorePlayer;
        }

        void CheckEnd()
        {
            if (scorePlayer >= 3)
            {
                isGameOver = true;
                Debug.Log("PLAYER WIN GAME!");
            }

            if (scoreBot >= 3)
            {
                isGameOver = true;
                Debug.Log("BOT WIN GAME!");
            }
        }
    }

}
