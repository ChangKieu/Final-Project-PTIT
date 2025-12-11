using UnityEngine;
using UnityEngine.Tilemaps;

namespace Minigame1
{

    [DefaultExecutionOrder(-1)]
    public class Board : MonoBehaviour
    {
        public Tilemap tilemap { get; private set; }
        public Tilemap jumpMap { get; private set; }
        public Tilemap walkThrough { get; private set; }
        public Tilemap hintMap { get; private set; }


        public Piece activePiece { get; private set; }

        [SerializeField] private Vector2Int boardSize = new Vector2Int(10, 20);
        [SerializeField] private Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

        private TetrominoData[] listTetrominoData;


        public RectInt Bounds
        {
            get
            {
                Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
                return new RectInt(position, boardSize);
            }
        }

        private void Start()
        {
            SetUpTileMapAndTetromino();
            SpawnPiece();


        }
        private void SpawnHint()
        {
            hintMap.ClearAllTiles();
            int currentIndex = GameManager.Instance.GetCurrentIndex();
            int nextIndex = Mathf.Min(currentIndex+3, listTetrominoData.Length);

            for(int i = currentIndex; i<nextIndex; i++)
            {
                TetrominoData obj = listTetrominoData[i];
                Vector3Int hintPosition = new Vector3Int(-1 - (i-currentIndex) * 5, 9, 0);
                if (obj.tetromino == Tetromino.Jump || obj.tetromino == Tetromino.WalkThrough)
                {
                    foreach (Vector3Int cell in obj.cells)
                    {
                        Vector3Int position = hintPosition + cell;
                        hintMap.SetTile(position, obj.ruleTile);
                    }
                }
                else
                {
                    foreach (Vector3Int cell in obj.cells)
                    {
                        Vector3Int position = hintPosition + cell;
                        hintMap.SetTile(position, obj.tile);
                    }
                }
            }
  
        }
        private void SetUpTileMapAndTetromino()
        {
            tilemap = transform.GetChild(0).GetComponent<Tilemap>();
            jumpMap = transform.GetChild(1).GetComponent<Tilemap>();
            walkThrough = transform.GetChild(2).GetComponent<Tilemap>();
            hintMap = transform.GetChild(3).GetComponent<Tilemap>();

            activePiece = GetComponentInChildren<Piece>();

            listTetrominoData = GameManager.Instance.GetTetrominoData();
            for (int i = 0; i < listTetrominoData.Length; i++)
            {
                listTetrominoData[i].Initialize();
            }
        }
        public void SpawnPiece()
        {
            if(GameManager.Instance.IsGameOver())
            {
                return;
            }
            SpawnHint();
            TetrominoData data = listTetrominoData[GameManager.Instance.GetCurrentIndex()];

            activePiece.Initialize(this, spawnPosition, data);

            if (IsValidPosition(activePiece, spawnPosition))
            {
                Set(activePiece);
            }
            else
            {
                StartCoroutine(GameManager.Instance.LoseGame());
            }
            GameManager.Instance.IncreaseCurrentIndex();
        }


        public void Set(Piece piece)
        {
            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int tilePosition = piece.cells[i] + piece.position;
                if(piece.data.tetromino == Tetromino.Jump)
                {
                    jumpMap.SetTile(tilePosition, piece.data.ruleTile);
                }
                else if (piece.data.tetromino == Tetromino.WalkThrough)
                {
                    walkThrough.SetTile(tilePosition, piece.data.ruleTile);
                }
                else
                {
                    tilemap.SetTile(tilePosition, piece.data.tile);
                }
            }
        }

        public void Clear(Piece piece)
        {
            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int tilePosition = piece.cells[i] + piece.position;
                tilemap.SetTile(tilePosition, null);
                jumpMap.SetTile(tilePosition, null);
                walkThrough.SetTile(tilePosition, null);
            }


        }

        public bool IsValidPosition(Piece piece, Vector3Int position)
        {
            RectInt bounds = Bounds;

            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int tilePosition = piece.cells[i] + position;

                if (!bounds.Contains((Vector2Int)tilePosition))
                {
                    return false;
                }

                if (tilemap.HasTile(tilePosition) || jumpMap.HasTile(tilePosition) || walkThrough.HasTile(tilePosition))
                {
                    return false;
                }
                Vector3 worldPos = tilemap.CellToWorld(tilePosition);

                Collider2D[] hitCol = Physics2D.OverlapPointAll(worldPos);
            
                foreach(Collider2D hit in hitCol)
                {
                    if (hit != null && hit.CompareTag("Ground"))
                    {
                        return false;
                    }
                }

            }

            return true;
        }

   
    }
}
