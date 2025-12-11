using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ScaleAnim : MonoBehaviour
{
    [SerializeField] Vector3 ScaleTo = Vector3.one;
    [SerializeField] float AnimateTime = 0.3f;
    [SerializeField] float Delay = 0f;
    public Ease EaseType = Ease.OutBack;
    public bool isDisplay = false;
    private Tween scaleTween;

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        scaleTween = transform.DOScale(ScaleTo, AnimateTime)
            .SetDelay(Delay)
            .SetEase(EaseType);
    }

    private void OnDisable()
    {
        scaleTween = transform.DOScale(Vector3.zero, AnimateTime)
            .SetDelay(Delay)
            .SetEase(EaseType);
    }
}
