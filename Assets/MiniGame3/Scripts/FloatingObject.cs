using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Minigame3
{
    public class FloatingObject : MonoBehaviour
    {
        [SerializeField] private float floatAmount = 10f;
        [SerializeField] private float duration = 1f;
        [SerializeField] private float delay = 1f;

        private RectTransform rectTransform;
        private float startY;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            startY = rectTransform.anchoredPosition.y;

            Invoke("StartFloating", delay);
        }

        void StartFloating()
        {
            rectTransform.DOAnchorPosY(startY + floatAmount, duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

}
