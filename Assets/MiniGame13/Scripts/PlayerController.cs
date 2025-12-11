using UnityEngine;

namespace MiniGame13
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Refs")]
        public Transform ball;
        public Rigidbody2D rb;

        [Header("Config")]
        public float moveSpeed = 6f;
        public float jumpForce = 7f;
        public float minX = 1.4f;
        public float maxX = 7.2f;

        private Vector3 startPos;
        private bool canJump = true;

        private void Start()
        {
            startPos = transform.position;
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            HandleMoveInput();
            HandleJumpInput();
            ClampPosition();
        }

        void HandleMoveInput()
        {
            float move = Input.GetAxisRaw("Horizontal"); 

            rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
        }

        void HandleJumpInput()
        {
            if (Input.GetMouseButtonDown(0) && canJump)
            {
                canJump = false;
                rb.linearVelocity = new Vector2(2f, jumpForce);
            }
        }

        void ClampPosition()
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.position = pos;
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
