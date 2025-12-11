using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MiniGame5
{
    public class LetterDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public char letter;

        public Vector2 originalPositionInSpawn;

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Canvas rootCanvas;

        [HideInInspector] public Transform originalParent;
        private Vector2 originalPosition;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            rootCanvas = GameManager.Instance.GetComponentInParent<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetParent(GameManager.Instance.transform, false); 
            originalParent = transform.parent;
            originalPosition = rectTransform.anchoredPosition;


            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rootCanvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            rectTransform.anchoredPosition = localPoint;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;

            if (transform.parent == GameManager.Instance.transform)
            {
                transform.SetParent(GameManager.Instance.LetterSpawnPointTransform());
                rectTransform.anchoredPosition = originalPositionInSpawn;
            }
        }

        public void SetLetter(char c)
        {
            letter = c;
            GetComponent<Text>().text = c.ToString();
        }
    }
}
