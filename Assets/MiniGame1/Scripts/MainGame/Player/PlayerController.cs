using UnityEngine;
using UnityEngine.UI;

namespace Minigame1
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private LayerMask groundLayer, jumpLayer;
        [SerializeField] private Transform groundCheck;

        private Rigidbody2D rb;
        private bool moveLeft = false;
        private bool moveRight = false;
        private bool isJumping = false;
        private Animator animator;
        float moveDir;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        public void SetupControlButtons()
        {
            DisableButton(GameManager.Instance.btnDown);
            DisableButton(GameManager.Instance.btnUp);

            SetupButton(GameManager.Instance.btnLeft, "left");
            SetupButton(GameManager.Instance.btnRight, "right");
            SetupButton(GameManager.Instance.btnPlace, "jump");
        }
        private void SetupButton(Button button, string direction)
        {
            var holdButton = button.GetComponent<HoldButton>();
            holdButton.SetDirection(direction);
            holdButton.SetPlayerMovement(this);
            holdButton.SetPieceMovement(null); 
        }

        private void DisableButton(Button button)
        {
            button.GetComponent<HoldButton>().enabled = false;
            button.GetComponent<Button>().interactable = false;
        }

        private void Update()
        {
            if(GameManager.Instance.IsGameOver() || !GameManager.Instance.IsPlacedAllTetromino())
            {
                rb.linearVelocity = new Vector2(0,rb.linearVelocity.y); 
                return;
            }
            moveDir = 0f;
            if (moveLeft) moveDir = -1f;
            else if (moveRight) moveDir = 1f;
            else HandleKeyboardInput();

            CheckJump();
            UpdateAnimation();

            if (moveDir < 0)
                transform.localScale = new Vector3(-0.1f, 0.1f, 0.1f);
            else if (moveDir > 0)
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);

            if (isJumping && IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                isJumping = false;
            }
        }

        private void CheckJump()
        {
            if (IsJumpTile())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 1.75f);
            }
        }

        private void UpdateAnimation()
        {
            bool isMoving = moveDir != 0;
            animator.SetBool("isWalking", isMoving);
            animator.SetBool("isJumping", !IsGrounded());
        }

        private void HandleKeyboardInput()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveDir = -1f;
            }

            else if (Input.GetKey(KeyCode.RightArrow))
            {
                moveDir = 1f;
            }
            else
            {
                moveDir = 0f;
            }


            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                isJumping = true;
            }
        }
        public void OnMoveHold(string dir)
        {
            if (dir == "left") moveLeft = true;
            else if (dir == "right") moveRight = true;
            else if (dir == "jump") isJumping = true;
        }

        public void OnMoveRelease(string dir)
        {
            if (dir == "left") moveLeft = false;
            else if (dir == "right") moveRight = false;
            else if (dir == "jump") isJumping = false;

        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapBox(groundCheck.position, new Vector2(0.1f, 0.1f), 0f, groundLayer);
        }

        private bool IsJumpTile()
        {
            return Physics2D.OverlapBox(groundCheck.position, new Vector2(0.2f, 0.1f), 0f, jumpLayer);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                StartCoroutine(GameManager.Instance.LoseGame());
                animator.SetTrigger("isLose");
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Finish"))
            {
                StartCoroutine(GameManager.Instance.WinGame());
                animator.SetTrigger("isWin");
            }
        }
    }

}
