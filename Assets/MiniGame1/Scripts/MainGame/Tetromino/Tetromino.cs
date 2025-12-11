using UnityEngine;
using UnityEngine.Tilemaps;

namespace Minigame1
{
    public enum Tetromino
    {
        I, J, L, O, S, T, Z, Jump, WalkThrough
    }

    [System.Serializable]
    public struct TetrominoData
    {
        public Tile tile { get; private set; }
        public RuleTile ruleTile { get; private set; }

        public Tetromino tetromino; 

        public Vector2Int[] cells { get; private set; }
        public Vector2Int[,] wallKicks { get; private set; }

        public void Initialize()
        {
            if(tetromino == Tetromino.Jump)
            {
                ruleTile = GameManager.Instance.GetJumpTile(); 
            }
            else if(tetromino == Tetromino.WalkThrough)
            {
                ruleTile = GameManager.Instance.GetWalkThroughTile();
            }
            else
            {
                Tile[] tiles = GameManager.Instance.GetColor();
                int randomColor = Random.Range(0, tiles.Length);
                tile = tiles[randomColor];

            }

            cells = Data.Cells[tetromino];
            wallKicks = Data.WallKicks[tetromino];
        }

    }

}
