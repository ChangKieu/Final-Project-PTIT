using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MiniGame7
{
    public class AnswerDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public int correctIndex;
        public int currentIndex;

        [SerializeField] private Text textAnswer;

        private Canvas canvas;
        private Transform parent;

        private Vector3 startLocalPos;

        private void Awake()
        {
            canvas = FindFirstObjectByType<Canvas>();
            parent = transform.parent;
        }

        public void SetupAnswer(string answer, int index)
        {
            textAnswer.text = answer;
            correctIndex = index;
            currentIndex = index;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            startLocalPos = transform.localPosition;
            transform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out Vector2 localPoint);

            transform.localPosition = localPoint;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            AnswerDrag target = GetTargetDrag(eventData);

            if (target != null && target != this)
            {
                int indexA = transform.GetSiblingIndex();
                int indexB = target.transform.GetSiblingIndex();

                transform.SetSiblingIndex(indexB);
                target.transform.SetSiblingIndex(indexA);

                GameManager.Instance.UpdateIndexOrder();
            }
            else
            {
                transform.localPosition = startLocalPos;
            }
        }

        private AnswerDrag GetTargetDrag(PointerEventData data)
        {
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);

            foreach (var r in results)
            {
                AnswerDrag drag = r.gameObject.GetComponent<AnswerDrag>();
                if (drag != null && drag != this)
                    return drag;
            }

            return null;
        }
    }
}
