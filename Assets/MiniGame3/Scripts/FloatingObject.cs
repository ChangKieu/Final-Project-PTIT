using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Minigame3
{
    public class FloatingObject : MonoBehaviour
    {
        [SerializeField] private bool isSprite = false;

        [SerializeField] private float floatAmount = 10f;
        [SerializeField] private float duration = 1f;
        [SerializeField] private float delay = 1f;

        private RectTransform rectTransform;
        private SpriteRenderer spriteRenderer;

        private float startY_UI;
        private float startY_World;

        void Start()
        {
            if (isSprite)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                startY_World = transform.position.y;
            }
            else
            {
                rectTransform = GetComponent<RectTransform>();
                startY_UI = rectTransform.anchoredPosition.y;
            }

            Invoke(nameof(StartFloating), delay);
        }

        void StartFloating()
        {
            if (isSprite)
            {
                transform.DOMoveY(startY_World + floatAmount, duration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                rectTransform.DOAnchorPosY(startY_UI + floatAmount, duration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }
    }
}
