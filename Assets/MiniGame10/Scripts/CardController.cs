using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace MiniGame10
{
    public class CardController : MonoBehaviour, IPointerClickHandler
    {
        public int number;
        public Sprite frontSprite;
        public Sprite backSprite;

        private bool isFlipped = false;
        private bool isLocked = false;

        private Image img;
        private GameManager gm;
        public bool IsLocked => isLocked;
        public bool IsFlipped => isFlipped;

        void Awake()
        {
            img = GetComponent<Image>();
        }

        void Start()
        {
            gm = FindAnyObjectByType<GameManager>();
            img.sprite = backSprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isLocked || isFlipped) return;
            gm.OnCardClicked(this);
        }

        public void FlipOpen()
        {
            isFlipped = true;
            transform.DOScaleX(0f, 0.2f).OnComplete(() =>
            {
                img.sprite = frontSprite;
                transform.DOScaleX(1f, 0.2f);
            });
        }

        public void FlipClose()
        {
            isFlipped = false;
            isLocked = false;
            transform.DOScaleX(0f, 0.2f).OnComplete(() =>
            {
                img.sprite = backSprite;
                transform.DOScaleX(1f, 0.2f);
            }).SetDelay(0.2f);
        }

        public void LockCard()
        {
            isLocked = true;
        }
    }
}
