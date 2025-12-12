using MiniGame4;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
namespace Minigame1
{
    [System.Serializable]
    public struct NumberLevel
    {
        public TetrominoData[] listTetromino;
        public string[] listSubject;
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game setting")]
        [SerializeField] private NumberLevel[] listLevel;
        [SerializeField] private GameObject map;
        private int currentLevel;
        private int currIndex = 0;

        [Header("Player")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private GameObject highLightLine;
        [SerializeField] private GameObject frame;

        public bool isPlacedAllTetromino { get; private set; }


        [Header("TIle")]
        [SerializeField] private Tile[] colorTile;
        [SerializeField] private RuleTile jumpTile;
        [SerializeField] private RuleTile walkThroughTile;


        [Header("UI")]
        public Button btnLeft, btnRight, btnUp, btnDown, btnPlace;
        [SerializeField] private GameObject winEffect;
        [SerializeField] private Text txtSubject, txtLevel;
        private int subjectIndex = 0;

        private bool isGameOver;

        [SerializeField] private GameObject homePanel;
        private string sceneName;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            sceneName = SceneManager.GetActiveScene().name;
            if (PlayerPrefs.GetInt("Menu" + sceneName, 0) == 0)
            {
                homePanel.SetActive(true);
                LoadSceneManager.Instance.FadeIn();
            }
            else
            {
                PlayerPrefs.SetInt("Menu" + sceneName, 0);
                homePanel.SetActive(false);
                SetUpLevel();
                LoadSceneManager.Instance.FadeInImage();
            }
        }

        private void SetUpLevel()
        {
            currentLevel = ProgressManager.GetProgress(sceneName);

            if (currentLevel >= listLevel.Length)
            {
                ProgressManager.SetProgress(sceneName, 0);
                currentLevel = 0;
            }
            map.transform.GetChild(currentLevel).gameObject.SetActive(true);
            txtSubject.text = listLevel[currentLevel].listSubject[subjectIndex];
            txtLevel.text = "Học kỳ " + (currentLevel + 1);
        }

        public void NextSubject()
        {
            subjectIndex++;
            if (subjectIndex >= listLevel[currentLevel].listSubject.Length)
            {
                txtSubject.transform.parent.gameObject.SetActive(false);
                return;
            }
            txtSubject.text = listLevel[currentLevel].listSubject[subjectIndex];
        }

        public GameObject GetHighLightLine()
        {
            return highLightLine;
        }
        public RuleTile GetJumpTile()
        {
            return jumpTile;
        }

        public RuleTile GetWalkThroughTile()
        {
            return walkThroughTile;
        }

        public Tile[] GetColor()
        {
            return colorTile;
        }

        public TetrominoData[] GetTetrominoData()
        {
            return listLevel[currentLevel].listTetromino;
        }

        public int GetCurrentIndex()
        {
            return currIndex;
        }

        public void IncreaseCurrentIndex()
        {
            currIndex++;
        }

        public bool CheckOutTetrominoData()
        {
            return currIndex >= listLevel[currentLevel].listTetromino.Length;
        }

        public bool IsPlacedAllTetromino()
        {
            return isPlacedAllTetromino;
        }

        public void SetPlacedAllTetromino(bool value)
        {
            isPlacedAllTetromino = value;
        }

        public void SetPlayerMovement()
        {
            frame.SetActive(false);
            playerController.SetupControlButtons();
        }

        public bool IsGameOver()
        {
            return isGameOver;
        }

        public IEnumerator WinGame()
        {
            Debug.Log("Win Game");
            winEffect.SetActive(true);
            isGameOver = true;

            currentLevel++;
            ProgressManager.SetProgress(sceneName, currentLevel);
            if (currentLevel >= listLevel.Length)
            {
                ProgressManager.SetDone(sceneName);
                LoadSceneManager.Instance.ShowPanelDone();
                yield break;
            }
            yield return new WaitForSeconds(3f);
            winEffect.SetActive(false);
            NextLevel();
        }

        public IEnumerator LoseGame()
        {
            Debug.Log("Lose Game");
            isGameOver = true;
            yield return new WaitForSeconds(1f);
            NextLevel();
        }

        public void NextLevel()
        {
            PlayerPrefs.SetInt("Menu" + sceneName, 1);

            LoadSceneManager.Instance.LoadSceneImg(sceneName);
        }
        public void LoadExit()
        {
            PlayerPrefs.SetInt("Menu" + sceneName, 0);

            LoadSceneManager.Instance.LoadScene(sceneName);
        }
        public void LoadHome()
        {
            LoadSceneManager.Instance.LoadScene("Home");
        }
    }

}

