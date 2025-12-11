using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace MiniGame9
{

    public class CardController : MonoBehaviour, IPointerClickHandler
    {
        public int id;
        public Image img;
        public Sprite frontSprite; 
        public Sprite backSprite;  

        private bool isFlipped = false;
        private bool isMatched = false;

        private GameManager gm;

        private void Start()
        {
            gm = FindAnyObjectByType<GameManager>();
            img.sprite = backSprite;     
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isFlipped || isMatched) return;
            gm.OnCardClicked(this);
        }

        public void FlipOpen()
        {
            isFlipped = true;

            // hiệu ứng flip
            transform.DOScaleX(0, 0.2f).OnComplete(() =>
            {
                img.sprite = frontSprite;
                transform.DOScaleX(1, 0.2f);
            });
        }

        public void FlipClose()
        {
            isFlipped = false;

            transform.DOScaleX(0, 0.2f).OnComplete(() =>
            {
                img.sprite = backSprite;
                transform.DOScaleX(1, 0.2f);
            });
        }

        public void HideCard()
        {
            isMatched = true;
            this.transform.DOScale(0, 0.3f).SetEase(Ease.InBack);
        }
    }
}
