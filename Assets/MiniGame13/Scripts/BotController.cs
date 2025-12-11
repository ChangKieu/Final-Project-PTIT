using UnityEngine;

namespace MiniGame13
{
    public class BotController : MonoBehaviour
    {
        public Transform ball;
        public Rigidbody2D rb;

        public float moveSpeed = 6f;
        public float jumpForce = 7f;

        public float minX = -7.2f;
        public float maxX = -1.4f;

        private Vector3 startPos;
        private bool canJump = true;
        private Rigidbody2D ballRb;

        private void Start()
        {
            startPos = transform.position;
            rb = GetComponent<Rigidbody2D>();

            if (ball != null)
                ballRb = ball.GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (ball == null) return;

            AutoMove();
            SmartJump();
        }

        void AutoMove()
        {
            // Bóng chưa sang sân bot → không di chuyển
            if (ball.position.x > maxX) return;

            float tx = Mathf.Clamp(ball.position.x, minX, maxX);
            Vector3 target = new Vector3(tx, transform.position.y, transform.position.z);

            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        }

        float jumpCooldown = 0.3f;
        float lastJumpTime = 0f;

        void SmartJump()
        {
            if (!canJump) return;
            if (ballRb == null) return;

            if (Time.time - lastJumpTime < jumpCooldown) return;
            if (ball.position.x > maxX) return;

            bool ballMovingToBot = ballRb.linearVelocity.x < 0;

            float predictedX = ball.position.x
                               + ballRb.linearVelocity.x * 0.3f; 

            bool ballWillBeInRange =
                predictedX > minX && predictedX < maxX &&
                Mathf.Abs(predictedX - transform.position.x) < 2f;

            bool ballHighEnough = ball.position.y > transform.position.y + 1f;

            if (ballMovingToBot && ballHighEnough && ballWillBeInRange)
            {
                canJump = false;
                lastJumpTime = Time.time;

                rb.linearVelocity = new Vector2(-2.5f, jumpForce);
            }
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Ground"))
                canJump = true;
        }

        public void ResetPos()
        {
            transform.position = startPos;
            rb.linearVelocity = Vector2.zero;
        }

        public bool CanJump() { return canJump; }
    }

}
