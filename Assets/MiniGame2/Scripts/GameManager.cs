using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] private Text higSscoreText;
        [SerializeField] private Text timerText;
        [SerializeField] private GameObject panelOver;

        private int score = 0;
        private float timeLeft = 60f;
        private bool isGameOver = false;
        [SerializeField] private GameObject homePanel;
        private string sceneName;

        private void Awake()
        {
            Instance = this;
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
                SpawnNewBall();
                LoadSceneManager.Instance.FadeInImage();
            }
        }

        private void Update()
        {
            if (isGameOver) return;

            timeLeft -= Time.deltaTime;

            int minutes = Mathf.FloorToInt(timeLeft / 60);
            int seconds = Mathf.FloorToInt(timeLeft % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            int highScore = PlayerPrefs.GetInt("HighScore2", 0);
            higSscoreText.text = "High Score: " + highScore;

            if (timeLeft <= 0)
                EndGame();
        }

        public void AddScore()
        {
            score++;
            int highScore = PlayerPrefs.GetInt("HighScore2", 0);
            if (score > highScore)
            {
                PlayerPrefs.SetInt("HighScore2", score);
                higSscoreText.text = "High Score: " + score;
            }

            scoreText.text = "Score: " + score;

            scoreText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5);
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

        private void EndGame()
        {
            isGameOver = true;
            
            panelOver.SetActive(true);
        }

        public bool IsGameOver()
        {
            return isGameOver;
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
