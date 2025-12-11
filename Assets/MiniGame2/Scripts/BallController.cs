using DG.Tweening;
using UnityEngine;

namespace Minigame2
{
    public class BallController : MonoBehaviour
    {
        private Rigidbody2D rb;
        private Vector2 startTouchPos;
        private bool isThrown = false;
        [SerializeField] private float throwPower = 5f;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        void Update()
        {
            if (transform.position.y < -7f)
            {
                Destroy(gameObject);
            }
        }

        void OnMouseDown()
        {
            if (!isThrown)
                startTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        void OnMouseUp()
        {
            if (!isThrown)
            {
                GameManager.Instance.SpawnBall();

                rb.bodyType = RigidbodyType2D.Dynamic;
                Vector2 endTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = (startTouchPos - endTouchPos);
                rb.AddForce(-direction * throwPower, ForceMode2D.Impulse);
                isThrown = true;

                //transform.DORotate(new Vector3(0, 0, 360f), 0.3f, RotateMode.FastBeyond360)
                //         .SetLoops(-1)
                //         .SetEase(Ease.Linear);
            }
        }

    }
}
