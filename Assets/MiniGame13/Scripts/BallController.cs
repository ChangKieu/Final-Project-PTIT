using DG.Tweening;
using UnityEngine;

namespace MiniGame13
{

    public class BallController : MonoBehaviour
    {
        public Rigidbody2D rb;

        [Header("Hit Force")]
        public float hitForceX = 6f;   
        public float hitForceY = 5f;  

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Ground"))
            {
                transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
                {
                
                });
                if (transform.position.x > 0)
                    GameManager.Ins.PlayerLose();
                else
                    GameManager.Ins.PlayerWin();

                return;
            }

            if (collision.collider.CompareTag("Player") && !collision.gameObject.GetComponent<PlayerController>().CanJump())
            {
                rb.linearVelocity = new Vector2(-hitForceX, hitForceY);
                return;
            }

            if (collision.collider.CompareTag("Bot") && !collision.gameObject.GetComponent<BotController>().CanJump())
            {
                rb.linearVelocity = new Vector2(hitForceX, hitForceY);
                return;
            }
        }
    }
}

