using UnityEngine;
using UnityEngine.UI;

namespace Minigame1
{
    public class Piece : MonoBehaviour
    {
        public Board board { get; private set; }
        public TetrominoData data { get; private set; }
        public Vector3Int[] cells { get; private set; }
        public Vector3Int position { get; private set; }
        public int rotationIndex { get; private set; }

        [SerializeField] private float stepDelay = 0.5f;
        [SerializeField] private float moveDelay = 0.1f;
        [SerializeField] private float lockDelay = 0.5f;

        private GameObject highlightLine;

        private float stepTime;
        private float moveTime;
        private float lockTime;

        [Header("UI")]
        private bool moveLeft, moveRight, moveDown, rotate, place;

        private void Start()
        {
            SetupControlButtons();
        }

        private void SetupButton(Button button, string direction)
        {
            highlightLine = GameManager.Instance.GetHighLightLine();
            var holdButton = button.GetComponent<HoldButton>();
            holdButton.SetDirection(direction);
            holdButton.SetPieceMovement(this);
        }

        private void SetupControlButtons()
        {
            SetupButton(GameManager.Instance.btnLeft, "left");
            SetupButton(GameManager.Instance.btnRight, "right");
            SetupButton(GameManager.Instance.btnDown, "down");
            SetupButton(GameManager.Instance.btnUp, "rotate");
            SetupButton(GameManager.Instance.btnPlace, "place");
        }


        public void Initialize(Board board, Vector3Int position, TetrominoData data)
        {
            this.data = data;
            this.board = board;
            this.position = position;

            rotationIndex = 0;
            stepTime = Time.time + stepDelay;
            moveTime = Time.time + moveDelay;
            lockTime = 0f;

            if (cells == null || cells.Length != data.cells.Length)
            {
                cells = new Vector3Int[data.cells.Length];
            }

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = (Vector3Int)data.cells[i];
            }
        }

        private void Update()
        {
            if(GameManager.Instance.IsGameOver())
            {
                return;
            }
            board.Clear(this);

            lockTime += Time.deltaTime;

            UpdateHighlightLine();


            if (Input.GetKeyDown(KeyCode.UpArrow) || rotate)
            {
                rotate = false;
                Rotate(1);
            }

            if (Input.GetKeyDown(KeyCode.Space) || place)
            {
                GameManager.Instance.NextSubject();
                place = false;
                HardDrop();
            }

            if (Time.time > moveTime)
            {
                HandleMoveInputs();
            }

            if (Time.time > stepTime)
            {
                Step();
            }

            board.Set(this);
        }

        private void UpdateHighlightLine()
        {
            if (highlightLine == null || cells == null || cells.Length == 0)
                return;
            highlightLine.SetActive(true);
            int minX = cells[0].x + position.x;
            int maxX = minX;

            for (int i = 1; i < cells.Length; i++)
            {
                int worldX = cells[i].x + position.x;
                minX = Mathf.Min(minX, worldX);
                maxX = Mathf.Max(maxX, worldX);
            }

            Vector3 minPos = board.tilemap.CellToWorld(new Vector3Int(minX, position.y, 0));
            Vector3 maxPos = board.tilemap.CellToWorld(new Vector3Int(maxX, position.y, 0));

            int widthInTiles = maxX - minX + 1;
            float scaleX = widthInTiles * 0.35f / 2;

            float centerX = (minPos.x + maxPos.x) / 2f + 0.4125f /2 ;

            highlightLine.transform.localScale = new Vector3(scaleX, 0.35f, 0.35f);
            highlightLine.transform.position = new Vector3(centerX, 0f, 0f);

        }

        private void HandleMoveInputs()
        {
            if (Input.GetKey(KeyCode.DownArrow) || moveDown)
            {
                if (Move(Vector2Int.down))
                {
                    stepTime = Time.time + stepDelay;
                }
            }

            if (Input.GetKey(KeyCode.LeftArrow) || moveLeft)
            {
                Move(Vector2Int.left);
            }
            else if (Input.GetKey(KeyCode.RightArrow) || moveRight)
            {
                Move(Vector2Int.right);
            }
        }

        private void Step()
        {
            stepTime = Time.time + stepDelay;

            Move(Vector2Int.down);

            if (lockTime >= lockDelay)
            {
                Lock();
            }
        }

        private void HardDrop()
        {
            while (Move(Vector2Int.down))
            {
                continue;
            }

            Lock();
        }

        private void Lock()
        {
            board.Set(this);
            highlightLine.SetActive(false);
            if (!GameManager.Instance.CheckOutTetrominoData())
            {
                board.SpawnPiece();
            }
            else
            {
                GameManager.Instance.SetPlacedAllTetromino(true);
                GameManager.Instance.SetPlayerMovement();
                board.hintMap.ClearAllTiles();
                board.hintMap.gameObject.SetActive(false);
                Destroy(board);
                Destroy(this);
            }
        }

        private bool Move(Vector2Int translation)
        {
            Vector3Int newPosition = position;
            newPosition.x += translation.x;
            newPosition.y += translation.y;

            bool valid = board.IsValidPosition(this, newPosition);

            if (valid)
            {
                position = newPosition;
                moveTime = Time.time + moveDelay;
                lockTime = 0f; 
            }

            return valid;
        }

        private void Rotate(int direction)
        {
            int originalRotation = rotationIndex;

            rotationIndex = Wrap(rotationIndex + direction, 0, 4);
            ApplyRotationMatrix(direction);

            if (!TestWallKicks(rotationIndex, direction))
            {
                rotationIndex = originalRotation;
                ApplyRotationMatrix(-direction);
            }

        }

        private void ApplyRotationMatrix(int direction)
        {
            float[] matrix = Data.RotationMatrix;

            for (int i = 0; i < cells.Length; i++)
            {
                Vector3 cell = cells[i];

                int x, y;

                switch (data.tetromino)
                {
                    case Tetromino.I:
                    case Tetromino.O:
                        cell.x -= 0.5f;
                        cell.y -= 0.5f;
                        x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                        y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                        break;

                    default:
                        x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                        y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                        break;
                }

                cells[i] = new Vector3Int(x, y, 0);
            }

        }

        private bool TestWallKicks(int rotationIndex, int rotationDirection)
        {
            int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

            for (int i = 0; i < data.wallKicks.GetLength(1); i++)
            {
                Vector2Int translation = data.wallKicks[wallKickIndex, i];

                if (Move(translation))
                {
                    return true;
                }
            }

            return false;
        }

        private int GetWallKickIndex(int rotationIndex, int rotationDirection)
        {
            int wallKickIndex = rotationIndex * 2;

            if (rotationDirection < 0)
            {
                wallKickIndex--;
            }

            return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
        }

        private int Wrap(int input, int min, int max)
        {
            if (input < min)
            {
                return max - (min - input) % (max - min);
            }
            else
            {
                return min + (input - min) % (max - min);
            }
        }

        public void MoveDown()
        {
            if (Move(Vector2Int.down))
            {
                stepTime = Time.time + stepDelay;
            }
        }

        public void OnMoveHold(string dir)
        {
            switch (dir)
            {
                case "left":
                    moveLeft = true;
                    break;
                case "right":
                    moveRight = true;
                    break;
                case "down":
                    moveDown = true;
                    break;
                case "rotate":
                    rotate = true;
                    break;
                case "place":
                    place = true;
                    break;
            }
        }

        public void OnMoveRelease(string dir)
        {
            switch (dir)
            {
                case "left":
                    moveLeft = false;
                    break;
                case "right":
                    moveRight = false;
                    break;
                case "down":
                    moveDown = false;
                    break;
                case "rotate":
                    rotate = false;
                    break;
                case "place":
                    place = false;
                    break;
            }
        }


    }

}