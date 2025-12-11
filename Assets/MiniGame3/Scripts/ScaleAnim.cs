using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Minigame3
{
    public class ScaleAnim : MonoBehaviour
    {
        [SerializeField] Vector3 ScaleTo = Vector3.one;
        [SerializeField] float AnimateTime = 0.3f;
        [SerializeField] float Delay = 0f;
        public Ease EaseType = Ease.OutBack;
        public bool isDisplay = false;
        private Tween scaleTween;
        [SerializeField] private bool isScales = true;

        private void OnEnable()
        {
            if (isScales)
            {
                transform.localScale = Vector3.zero;
                scaleTween = transform.DOScale(ScaleTo, AnimateTime)
                    .SetDelay(Delay)
                    .SetEase(EaseType);
                return;
            }
            Vector3 scale = transform.localScale;
            scale.y = 0;
            transform.localScale = scale;
            scaleTween = transform.DOScaleY(ScaleTo.y, AnimateTime)
                .SetDelay(Delay)
                .SetEase(EaseType);
        }

    }

}