using DG.Tweening;
using UnityEngine;
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

        private void Awake()
        {
            Instance = this;
            SetUp();
        }
        private void SetUp()
        {
            currentLevel = PlayerPrefs.GetInt("Level", 0);
            if (currentLevel >= map.childCount)
            {
                currentLevel = 0;
                PlayerPrefs.SetInt("Level", 0);
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

            int nextLevel = currentLevel + 1;
            PlayerPrefs.SetInt("Level", nextLevel);
            PlayerPrefs.Save();
            player.isMoving = false;
            gamePlaying = false;
        }

        public void Lose()
        {
            if (!gamePlaying) return;
            Debug.Log("LOSE!");

            player.isMoving = false;
            gamePlaying = false;
        }

        public bool IsGamePlaying()
        {
            return gamePlaying;
        }


    }
}

