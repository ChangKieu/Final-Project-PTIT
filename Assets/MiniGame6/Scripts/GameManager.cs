using DG.Tweening;
using MiniGame4;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiniGame6
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private Transform map;
        [SerializeField] private TimelineController timeline;
        private int currentLevel = 0;
        private PlayerController player;
        private ObstacleController[] obs;

        [Header("UI")]
        [SerializeField] private Button btnPlay;
        [SerializeField] private Sprite playSprite;
        private bool gamePlaying = false;

        [SerializeField] private GameObject homePanel;
        private string sceneName;
        private void Awake()
        {
            Instance = this;

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
            currentLevel = ProgressManager.GetProgress(sceneName);

            if (currentLevel >= map.childCount)
            {
                ProgressManager.SetProgress(sceneName, 0);
                currentLevel = 0;
            }

            currentLevel = 0;

            btnPlay.onClick.AddListener(PressPlay);
            map.GetChild(currentLevel).gameObject.SetActive(true);

            player = FindFirstObjectByType<PlayerController>();
            obs = FindObjectsByType<ObstacleController>(FindObjectsSortMode.None);

        }
        public void PressPlay()
        {
            if (gamePlaying) return;

            btnPlay.image.sprite = playSprite;
            gamePlaying = true;

            float delayTime = timeline.NormalizedTime * 2.5f;
            DOVirtual.DelayedCall(delayTime, () => {
                player.isMoving = true;
            });

            foreach (var ob in obs)
                ob.StartMoving();

            StartCoroutine(MoveTimelineToEnd());
        }

        private System.Collections.IEnumerator MoveTimelineToEnd()
        {
            RectTransform handle = timeline.playHandle;
            float speed = 900f;

            while (gamePlaying)
            {
                Vector2 pos = handle.anchoredPosition;
                pos.x += speed * Time.deltaTime;
                pos.x = Mathf.Clamp(pos.x, -2115f, 2115f);

                handle.anchoredPosition = pos;

                if (Mathf.Abs(pos.x - 2115f) < 0.01f)
                {
                    Debug.Log("Timeline reached the end");
                    Win();
                    yield break;
                }

                yield return null;
            }
        }

        public void Win()
        {
            if (!gamePlaying) return;

            currentLevel++;
            ProgressManager.SetProgress(sceneName, currentLevel);
            if (currentLevel >= map.childCount)
            {
                ProgressManager.SetDone(sceneName);
                LoadSceneManager.Instance.ShowPanelDone();
                return;
            }
            player.isMoving = false;
            gamePlaying = false;

            NextLevel();
        }

        public void Lose()
        {
            if (!gamePlaying) return;

            player.isMoving = false;
            gamePlaying = false;
            NextLevel();
        }

        public bool IsGamePlaying()
        {
            return gamePlaying;
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

