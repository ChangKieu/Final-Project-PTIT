using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MiniGame7
{
    public class AnswerDrag : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public int correctIndex;
        public int currentIndex;

        [SerializeField] private Text textAnswer;

        [HideInInspector] public RectTransform rect;

        private Canvas canvas;
        private Camera uiCam;

        private Vector2 startPos;
        private Vector2 offset;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();

            uiCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;
        }

        public void SetupAnswer(string answer, int index)
        {
            textAnswer.text = answer;
            correctIndex = index;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            startPos = rect.anchoredPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect.parent as RectTransform,
                eventData.position,
                uiCam,
                out Vector2 localMousePos
            );

            offset = startPos - localMousePos;
            transform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect.parent as RectTransform,
                eventData.position,
                uiCam,
                out Vector2 localMousePos
            );

            rect.anchoredPosition = localMousePos + offset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            AnswerDrag target = GetNearestAnswer();

            if (target != null)
            {
                SwapWith(target);
                AudioManager.Instance.PlayPlace();
            }
            else
            {
                rect.anchoredPosition = startPos;
            }
        }

        private void SwapWith(AnswerDrag target)
        {
            Vector2 targetPos = target.rect.anchoredPosition;
            int targetIndex = target.currentIndex;

            target.rect.anchoredPosition = startPos;
            target.currentIndex = currentIndex;

            rect.anchoredPosition = targetPos;
            currentIndex = targetIndex;
        }

        private AnswerDrag GetNearestAnswer()
        {
            float minDist = float.MaxValue;
            AnswerDrag nearest = null;

            foreach (var a in GameManager.Instance.listAnswerDrag)
            {
                if (a == this) continue;

                float dist = Vector2.Distance(rect.anchoredPosition, a.rect.anchoredPosition);
                if (dist < 90f && dist < minDist)
                {
                    minDist = dist;
                    nearest = a;
                }
            }

            return nearest;
        }
    }
}
