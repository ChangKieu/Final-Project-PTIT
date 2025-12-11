using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGame15
{
    public class ItemDragStep1 : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public int index;

        private RectTransform rect;
        private Vector2 startPos;
        private Canvas canvas;
        private Vector2 offset;

        public Transform currentSlot = null;   // slot hiện tại đang đứng

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            canvas = FindFirstObjectByType<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            startPos = rect.anchoredPosition;

            // Tính offset để giữ vị trí chuẩn
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 mousePos
            );

            offset = rect.anchoredPosition - mousePos;

            // Khi bắt đầu kéo, bỏ liên kết slot cũ
            currentSlot = null;
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
            Transform newSlot = GameManager.Instance.GetSlotAtStep1(eventData.position);

            if (newSlot != null)
            {
                Debug.Log("Snap vào slot");

                ItemDragStep1 otherItem = FindItemInSlot(newSlot);

                if (otherItem != null && otherItem != this)
                {
                    otherItem.rect.anchoredPosition = otherItem.startPos;
                    otherItem.currentSlot = null;
                }

                currentSlot = newSlot;

                rect.position = newSlot.GetComponent<RectTransform>().position;
            }
            else
            {
                rect.anchoredPosition = startPos;
            }
        }

        private ItemDragStep1 FindItemInSlot(Transform slot)
        {
            foreach (ItemDragStep1 item in FindObjectsByType<ItemDragStep1>(FindObjectsSortMode.None))
            {
                if (item != this && item.currentSlot == slot)
                    return item;
            }
            return null;
        }
    }
}
