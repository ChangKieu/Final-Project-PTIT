using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

namespace Minigame3
{
    public enum BrushColor
    {
        Blue,
        Green,
        Purple,
        Red,
        Yellow,
        Orange,
        Black
    }

    public class BrushController : MonoBehaviour
    {
        private BrushColor brushColor;
        private TileBase brushTile;
        private Vector3 child1InitialPos;

        private void Awake()
        {
            child1InitialPos = transform.GetChild(1).position;
        }

        private void OnMouseDown()
        {
            if (GameManager.Instance.IsGameOver() || GameManager.Instance.IsDrawing()) return;
            Draw();
        }

        public void SetUpBrush(int color, TileBase tile)
        {
            brushColor = (BrushColor)color;
            brushTile = tile;

            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite =
                GameManager.Instance.GetBrushSprite(color);

            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite =
                GameManager.Instance.GetBrushSprite(color);
        }

        public void Draw()
        {
            GameManager.Instance.SetIsDrawing(true);

            var child1 = transform.GetChild(1);
            child1.gameObject.SetActive(true);

            float zRotation = transform.GetChild(0).eulerAngles.z;
            Vector3 direction = Mathf.Approximately(zRotation, 0f) ? Vector3.left : Vector3.up;

            float stepSize = 0.7f;
            int steps = 4;

            Tilemap board = GameManager.Instance.GetBoard();

            Sequence seq = DOTween.Sequence();
            Vector3 currentPos = child1.position;

            for (int i = 1; i <= steps; i++)
            {
                Vector3 newPos = currentPos + direction * stepSize;

                seq.Append(child1.DOMove(newPos, 0.15f).OnStepComplete(() =>
                {
                    Vector3Int cellPos = board.WorldToCell(child1.position);

                    if (board.HasTile(cellPos))
                    {
                        BrushColor? currentColor = GameManager.Instance.GetTileColor(cellPos);

                        BrushColor finalColor = GameManager.Instance.MixColor(currentColor, brushColor);

                        TileBase tile = GameManager.Instance.GetTileByColor(finalColor);
                        board.SetTile(cellPos, tile);

                        GameManager.Instance.SetTileColor(cellPos, finalColor);
                    }
                }));

                currentPos = newPos;
            }

            seq.OnComplete(() =>
            {
                GameManager.Instance.SetIsDrawing(false);

                child1.position = child1InitialPos;
                child1.gameObject.SetActive(false);

                if (GameManager.Instance.CheckWinCondition())
                {
                    StartCoroutine(GameManager.Instance.WinGame());
                }
            });

            seq.Play();
        }
    }
}
