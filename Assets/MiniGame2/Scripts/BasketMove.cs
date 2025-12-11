using UnityEngine;
using DG.Tweening;

namespace Minigame2
{
    public class BasketMove : MonoBehaviour
    {
        [SerializeField] private float moveRange = 7f;
        [SerializeField] private float minSpeed = 1f;
        [SerializeField] private float maxSpeed = 5f;

        private bool ballInside = false;
        private Tween moveTween; 

        private void Start()
        {
            MoveBasket();
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
            {
                if (moveTween != null && moveTween.IsActive())
                {
                    moveTween.Kill();
                    moveTween = null;
                }
            }
        }

        private void MoveBasket()
        {
            float targetX = Random.Range(-moveRange, moveRange);
            float duration = Random.Range(minSpeed, maxSpeed);

            moveTween = transform.DOMoveX(targetX, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (GameManager.Instance == null || !GameManager.Instance.IsGameOver())
                        MoveBasket();
                });
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Ball"))
            {
                if (other.transform.position.y >= transform.position.y)
                    ballInside = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Ball") && ballInside)
            {
                ballInside = false;
                if (other.transform.position.y <= transform.position.y)
                {
                    GameManager.Instance.AddScore();
                }
            }
        }
    }
}
