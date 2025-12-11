using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGame15
{
    public class ItemSwapStep2 : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public int index;

        private RectTransform rect;
        private Vector2 startPos;
        private Canvas canvas;
        private Vector2 offset;  // quan trọng để item không lệch chuột

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            canvas = FindFirstObjectByType<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            startPos = rect.anchoredPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 mousePos
            );

            offset = rect.anchoredPosition - mousePos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 mousePos
            );

            rect.anchoredPosition = mousePos + offset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Transform target = GameManager.Instance.GetSwapTargetStep2(eventData.position);

            if (target != null && target != transform)
            {
                RectTransform rectTarget = target.GetComponent<RectTransform>();

                Vector2 targetPos = rectTarget.anchoredPosition;

                rectTarget.anchoredPosition = startPos;
                rect.anchoredPosition = targetPos;
                Debug.Log("Swapped items");
            }
            else
            {
                rect.anchoredPosition = startPos;
            }
        }
    }
}
