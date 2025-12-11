using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Minigame3
{
    [System.Serializable]
    public struct BrushData
    {
        public Sprite brushSprite;
        public TileBase brushTile;
    }

    [System.Serializable]
    public struct LevelData
    {
        public int[] color;
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Level Settings")]
        [SerializeField] private Tilemap board;
        [SerializeField] private Transform resultMap;
        private int currentLevel;

        [Header("UI")]
        [SerializeField] private GameObject winPanel, losePanel, winEffect;
        [SerializeField] private Text txtLevel;
        [SerializeField] private Transform brushContainer;
        [SerializeField] private BrushData[] brushDatas;
        [SerializeField] private LevelData[] levelDatas;

        [Header("Timer")]
        [SerializeField] private Image timerImage;

        private bool isGameOver = false;
        private bool isDrawing = false;

        // Lưu màu của từng tile
        private Dictionary<Vector3Int, BrushColor> tileColors = new Dictionary<Vector3Int, BrushColor>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            SetUpLevel();
        }

        private void SetUpLevel()
        {
            currentLevel = PlayerPrefs.GetInt("Level", 0);

            if (currentLevel >= resultMap.childCount)
            {
                currentLevel = 0;
                PlayerPrefs.SetInt("Level", 0);
            }

            txtLevel.text = "Level " + (currentLevel + 1);

            resultMap.GetChild(currentLevel).gameObject.SetActive(true);

            SetUpBrush();

            StartTimer(15f);
        }

        private void SetUpBrush()
        {
            LevelData current = levelDatas[currentLevel];

            for (int i = 0; i < brushContainer.childCount; i++)
            {
                int colorIndex = current.color[i];

                BrushController brush = brushContainer.GetChild(i)
                    .GetComponent<BrushController>();

                brush.SetUpBrush(colorIndex, brushDatas[colorIndex].brushTile);
            }
        }

        // =========================================
        // TIMER
        // =========================================

        private void StartTimer(float duration)
        {
            if (timerImage != null)
                timerImage.fillAmount = 1f;

            StartCoroutine(TimerCoroutine(duration));
        }

        private IEnumerator TimerCoroutine(float duration)
        {
            float time = duration;

            while (time > 0f)
            {
                timerImage.fillAmount = time / duration;

                if (isGameOver)
                    yield break;

                yield return null;
                time -= Time.deltaTime;
            }

            if (!isGameOver)
                StartCoroutine(LoseGame());
        }

        // =========================================
        // TILE COLOR SYSTEM
        // =========================================

        public BrushColor? GetTileColor(Vector3Int pos)
        {
            if (tileColors.ContainsKey(pos))
                return tileColors[pos];

            return null;
        }

        public void SetTileColor(Vector3Int pos, BrushColor color)
        {
            tileColors[pos] = color;
        }

        public TileBase GetTileByColor(BrushColor color)
        {
            return brushDatas[(int)color].brushTile;
        }

        // Pha màu theo yêu cầu
        public BrushColor MixColor(BrushColor? current, BrushColor brush)
        {
            if (current == null)
                return brush;

            BrushColor c = current.Value;

            if (c == BrushColor.Black)
                return BrushColor.Black;

            if (c == BrushColor.Green || c == BrushColor.Orange || c == BrushColor.Purple)
            {
                if (brush == BrushColor.Blue || brush == BrushColor.Red || brush == BrushColor.Yellow)
                    return BrushColor.Black;

                return BrushColor.Black;
            }

            if (c == BrushColor.Blue)
            {
                if (brush == BrushColor.Yellow) return BrushColor.Green;
                if (brush == BrushColor.Red) return BrushColor.Purple;
            }

            if (c == BrushColor.Yellow)
            {
                if (brush == BrushColor.Blue) return BrushColor.Green;
                if (brush == BrushColor.Red) return BrushColor.Orange;
            }

            if (c == BrushColor.Red)
            {
                if (brush == BrushColor.Blue) return BrushColor.Purple;
                if (brush == BrushColor.Yellow) return BrushColor.Orange;
            }

            return c;
        }


        public bool CheckWinCondition()
        {
            Tilemap goal = resultMap.GetChild(currentLevel).GetComponent<Tilemap>();

            foreach (var pos in board.cellBounds.allPositionsWithin)
            {
                TileBase a = board.GetTile(pos);
                TileBase b = goal.GetTile(pos);

                if (b != null)
                {
                    if (a == null || a != b)
                        return false;
                }
            }

            return true;
        }

        public IEnumerator WinGame()
        {
            isGameOver = true;

            int levelIndex = PlayerPrefs.GetInt("Level", 0) + 1;

            if (PlayerPrefs.GetInt("LevelUnlocked", 0) < levelIndex)
                PlayerPrefs.SetInt("LevelUnlocked", levelIndex);

            PlayerPrefs.SetInt("Level", levelIndex);
            PlayerPrefs.Save();

            winEffect.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            winPanel.SetActive(true);
        }

        public IEnumerator LoseGame()
        {
            isGameOver = true;
            yield return new WaitForSeconds(0.7f);
            losePanel.SetActive(true);
        }

        // =========================================
        // GETTERS
        // =========================================

        public Tilemap GetBoard() => board;
        public bool IsDrawing() => isDrawing;
        public bool IsGameOver() => isGameOver;

        public void SetIsDrawing(bool val) => isDrawing = val;
        public Sprite GetBrushSprite(int index)
        {
            return brushDatas[index].brushSprite;
        }
        public void ReloadLevel()
        {
            PlayerPrefs.SetInt("Level", currentLevel);
            PlayerPrefs.Save();
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }

        public void NextLevel()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }

        public void LoadHome()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
            PlayerPrefs.SetInt("Menu", 0);
        }

        public void LoadMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
            PlayerPrefs.SetInt("Menu", 1);
        }
    }
}
