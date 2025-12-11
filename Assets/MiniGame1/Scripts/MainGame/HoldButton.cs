using UnityEngine;
using UnityEngine.EventSystems;

namespace Minigame1
{
    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private string direction;
        private PlayerController playerMovement;
        private Piece pieceMovement;
        private void Start()
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(playerMovement != null) 
                playerMovement.OnMoveHold(direction);
            if (pieceMovement != null)
                pieceMovement.OnMoveHold(direction);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (pieceMovement != null)
                pieceMovement.OnMoveRelease(direction);
            if (playerMovement != null)
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
        public void SetPieceMovement(Piece movement)
        {
            pieceMovement = movement;
        }
    }

}
