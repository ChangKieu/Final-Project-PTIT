using MiniGame5;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGame5
{
    public class AnswerSlot : MonoBehaviour, IDropHandler
    {
        public char correctChar;
        private LetterDrag placedLetter;

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;

            LetterDrag drag = eventData.pointerDrag.GetComponent<LetterDrag>();

            if (placedLetter != null)
            {
                placedLetter.transform.SetParent(GameManager.Instance.LetterSpawnPointTransform());
                placedLetter.GetComponent<RectTransform>().anchoredPosition =
                    placedLetter.originalPositionInSpawn;
            }

            drag.transform.SetParent(transform);
            drag.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            placedLetter = drag;

            GameManager.Instance.CheckWinCondition();
        }

        public bool IsCorrect()
        {
            return placedLetter != null && placedLetter.letter == correctChar;
        }
    }

}
