using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiniGame13
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Ins;

        [Header("Setup")]
        [SerializeField] private Transform spawnPosBall;
        [SerializeField] private GameObject ballPrefab;

        [Header("Refs")]
        [SerializeField] private PlayerController player;
        [SerializeField] private BotController bot;
        [SerializeField] private Text txtScore;

        private int scorePlayer = 0;
        private int scoreBot = 0;

        private int lastWinner = 1;
        private GameObject currentBall;
        private bool isGameOver = true;

        [SerializeField] private Sprite[] winEmo;
        [SerializeField] private Sprite loseEmo;
        [SerializeField] private GameObject homePanel, winEffect;
        private string sceneName;
        private void Awake()
        {
            Ins = this;
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
                SetUp();
                LoadSceneManager.Instance.FadeInImage();
            }
        }
        private void SetUp()
        {
            isGameOver = false;
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
            AudioManager.Instance.PlayLose();
            scoreBot++;
            lastWinner = -1;

            player.SetEmo(loseEmo);
            bot.SetEmo(winEmo[Random.Range(0, winEmo.Length)]);

            CheckEnd();
            ResetRound();
        }

        public void PlayerWin()
        {
            AudioManager.Instance.PlayWin();
            scorePlayer++;
            lastWinner = 1;

            player.SetEmo(winEmo[Random.Range(0, winEmo.Length)]);
            bot.SetEmo(loseEmo);

            CheckEnd();
            ResetRound();
        }

        void ResetRound()
        {
            
            UpdateScoreText();

            Destroy(currentBall);
            DOVirtual.DelayedCall(1f, () => {
                player.ResetPos();
                bot.ResetPos();
            });
            Invoke(nameof(SpawnBall), 1.5f);
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

                winEffect.SetActive(true);

                ProgressManager.SetDone(sceneName);
                LoadSceneManager.Instance.ShowPanelDone();
                return;
            }

            if (scoreBot >= 3)
            {
                isGameOver = true;
                NextLevel();
            }
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
