using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGame6
{
    public class TimelineController : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        public RectTransform timelineHandle;
        public RectTransform playHandle;
        public float minX = -2115f;
        public float maxX = 2115f;

        private Vector2 offset;

        public float NormalizedTime
            => Mathf.InverseLerp(minX, maxX, timelineHandle.anchoredPosition.x);

        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)timelineHandle.parent,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            offset = timelineHandle.anchoredPosition - localPoint;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)timelineHandle.parent,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            Vector2 newPos = localPoint + offset;
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);

            timelineHandle.anchoredPosition = new Vector2(newPos.x, timelineHandle.anchoredPosition.y);
        }
    }

}
