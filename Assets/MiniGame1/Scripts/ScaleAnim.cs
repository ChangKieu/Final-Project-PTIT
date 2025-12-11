using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Minigame1
{
    public class ScaleAnim : MonoBehaviour
    {
        [SerializeField] Vector3 ScaleTo = Vector3.one;
        [SerializeField] float AnimateTime = 0.3f;
        [SerializeField] float Delay = 0f;
        [SerializeField] bool isFade = false;
        public Ease EaseType = Ease.OutBack;
        public bool isDisplay = false;
        private Tween scaleTween;

        private void OnEnable()
        {
            if (isFade)
            {
                Image spriteRenderer = GetComponent<Image>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.DOFade(1f, AnimateTime).SetDelay(Delay).SetEase(EaseType);
                }
            
            }
            else
            {
                transform.localScale = Vector3.zero;
                scaleTween = transform.DOScale(ScaleTo, AnimateTime)
                    .SetDelay(Delay)
                    .SetEase(EaseType);

            }
        }

    }

}