using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
namespace MiniGame11
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Transform movePoint;
        [SerializeField] private Animator animator;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float distanceMultiplier = 1.1f;
        [SerializeField] private GameObject itemPick;
        [SerializeField] private Transform itemPickTransform, throwPosition;

        [Header("Collision")]
        [SerializeField] private LayerMask walkableTile;
        [SerializeField] private LayerMask bucket, home;

        [Header("UI Buttons")]
        [SerializeField] private Button btnUp, btnDown, btnLeft, btnRight, btnPick;

        private bool moveUp, moveDown, moveLeft, moveRight;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            transform.position = GameManager.Instance.GetPlayerPosition();
            movePoint.parent = null;
            btnUp.GetComponent<HoldButton>().SetDirection("up");
            btnUp.GetComponent<HoldButton>().SetPlayerMovement(this);

            btnDown.GetComponent<HoldButton>().SetDirection("down");
            btnDown.GetComponent<HoldButton>().SetPlayerMovement(this);

            btnLeft.GetComponent<HoldButton>().SetDirection("left");
            btnLeft.GetComponent<HoldButton>().SetPlayerMovement(this);

            btnRight.GetComponent<HoldButton>().SetDirection("right");
            btnRight.GetComponent<HoldButton>().SetPlayerMovement(this);

            btnPick.onClick.AddListener(PickUpItem);
        }


        void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

            bool isWalking = Vector3.Distance(transform.position, movePoint.position) > 0.05f;
            //animator.SetBool("isWalking", isWalking);
            if(GameManager.Instance.IsGameOver())
            {
                return;
            }
            if (!isWalking)
            {
                Vector3 dir = Vector3.zero;

                if (moveRight)
                {
                    dir = Vector3.right;

                }
                else if (moveLeft)
                {
                    dir = Vector3.left;

                }
                else if (moveUp)
                {
                    dir = Vector3.up;
                }
                else if (moveDown)
                {
                    dir = Vector3.down;
                }
                else
                {
                    float moveX = Input.GetAxisRaw("Horizontal");
                    float moveY = Input.GetAxisRaw("Vertical");
                    if (Mathf.Abs(moveX) > 0.1f)
                        dir = moveX > 0 ? Vector3.right : Vector3.left;
                    else if (Mathf.Abs(moveY) > 0.1f)
                        dir = moveY > 0 ? Vector3.up : Vector3.down;
                }

                if (dir != Vector3.zero)
                {
                    if (dir.x > 0)
                        transform.localScale = new Vector3(0.075f, 0.075f, 0.075f);
                    else if (dir.x < 0)
                        transform.localScale = new Vector3(-0.075f, 0.075f, 0.075f);
                    Vector3 targetPos = movePoint.position + dir * distanceMultiplier;
                    bool canWalk = Physics2D.OverlapCircle(targetPos, 0.2f, walkableTile);
                    bool isDoneTag = HasDoneTagAtPosition(targetPos, 0.2f);
                    if (canWalk)
                    {
                        movePoint.position = targetPos;
                        if (isDoneTag)
                        {
                            GameManager.Instance.CheckNumberHome();
                        }
                        GameManager.Instance.CheckFuel();
                    }
                }

            }
        }
        bool HasDoneTagAtPosition(Vector3 position, float radius = 0.2f)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius);
            foreach (var col in colliders)
            {
                if (col.CompareTag("Done"))
                    return true;
            }
            return false;
        }
        void PickUpItem()
        {
            Collider2D itemCol = Physics2D.OverlapCircle(transform.position + Vector3.down * distanceMultiplier / 2, 0.2f, bucket);
            Collider2D homeCol = Physics2D.OverlapCircle(transform.position + Vector3.up * distanceMultiplier, 0.2f, home
                );
            if(homeCol != null && itemPick!=null)
            {
                ItemController homeControl = homeCol.GetComponent<ItemController>();
                ItemController itemControl = itemPick.GetComponent<ItemController>();
                if(homeControl != null && itemControl != null && homeControl.GetID() == itemControl.GetID())
                {
                    itemPick.transform.SetParent(null);
                    itemPick.transform.DOMove(homeCol.transform.position, 0.2f).SetEase(Ease.OutBack);
                    itemPick.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        homeControl.SetDoneSprite();
                    }); ;
                    itemPick = null;
                    GameManager.Instance.AddNumberHome();
                    return;
                }
            }
            if (itemCol != null)
            {
                if(itemPick == null)
                {
                    itemPick = itemCol.gameObject;
                    ItemController itemController = itemPick.GetComponent<ItemController>();
                    if (itemController != null)
                    {
                        itemPick.transform.SetParent(itemPickTransform);
                        itemPick.transform.localPosition = Vector3.zero;
                        itemPick.GetComponent<Collider2D>().enabled = false;
                    }

                }
                else
                {
                    Vector3 colOriginalPos = itemCol.transform.position;
                    Vector3 pickOriginalPos = itemPick.transform.position;

                    itemPick.transform.SetParent(null);
                    itemPick.transform.position = colOriginalPos;
                    itemPick.GetComponent<Collider2D>().enabled = true;

                    itemCol.transform.SetParent(itemPickTransform);
                    itemCol.transform.localPosition = Vector3.zero;
                    itemCol.GetComponent<Collider2D>().enabled = false;

                    itemPick = itemCol.gameObject;
                }
            }
            else
            {
                if (itemPick != null)
                {
                    itemPick.transform.SetParent(null);
                    itemPick.transform.DOMove(throwPosition.position, 0.2f).SetEase(Ease.OutBack);
                    itemPick.GetComponent<Collider2D>().enabled = true;
                    itemPick = null;
                }
            }
        }

        public void OnMoveHold(string direction)
        {
            switch (direction)
            {
                case "up": moveUp = true; break;
                case "down": moveDown = true; break;
                case "left": moveLeft = true; break;
                case "right": moveRight = true; break;
            }
        }

        public void OnMoveRelease(string direction)
        {
            switch (direction)
            {
                case "up": moveUp = false; break;
                case "down": moveDown = false; break;
                case "left": moveLeft = false; break;
                case "right": moveRight = false; break;
            }
        }
    }

}
