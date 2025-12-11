using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniGame11
{
    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private string direction;
        private PlayerController playerMovement;

        private void Start()
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            playerMovement.OnMoveHold(direction);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            playerMovement.OnMoveRelease(direction);
        }
        public void SetDirection(string dir)
        {
            direction = dir;
        }
        public void SetPlayerMovement(PlayerController movement)
        {
            playerMovement = movement;
        }
    }

}
