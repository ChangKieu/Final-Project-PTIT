using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace MiniGame14
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private int type = 0;
        [SerializeField] private Button btnLeft, btnRight;
        public float moveDuration = 0.2f;

        private int currentLane = 2; 
        private RectTransform rect;
        private Image img;

        private Tween moveTween;

        private readonly float laneValue = 104.5f;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            img = GetComponent<Image>();
            MoveToLane(currentLane);
            btnLeft.onClick.AddListener(MoveLeft);
            btnRight.onClick.AddListener(MoveRight);
        }

        void Update()
        {
            if (GameManager.Instance.IsGameOver()) return;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                MoveLeft();

            if (Input.GetKeyDown(KeyCode.RightArrow))
                MoveRight();
        }

        public void MoveLeft()
        {
            if (GameManager.Instance.IsGameOver()) return;

            if (currentLane > 1)
            {
                currentLane--;
                MoveToLane(currentLane);
            }
        }

        public void MoveRight()
        {
            if (GameManager.Instance.IsGameOver()) return;

            if (currentLane < 4)
            {
                currentLane++;
                MoveToLane(currentLane);
            }
        }

        private void MoveToLane(int lane)
        {
            float targetX = 0f;

            switch (lane)
            {
                case 1: targetX = -laneValue * 3f; break; 
                case 2: targetX = -laneValue; break;  
                case 3: targetX = laneValue; break; 
                case 4: targetX = laneValue * 3f; break;  
            }

            if (moveTween != null)
                moveTween.Kill();

            moveTween = rect.DOAnchorPosX(targetX, moveDuration)
                            .SetEase(Ease.OutQuad);
        }

        public int GetPlayerType()
        {
            return type;
        }

        public void SetPlayer(int t, Sprite sp)
        {
            type = t;
            img.sprite = sp;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            ObstacleController ob = col.GetComponent<ObstacleController>();
            if (ob != null)
            {
                GameManager.Instance.HitObstacle(ob.GetObstacleType());
            }
        }
    }

}
