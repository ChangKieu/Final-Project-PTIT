using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiniGame11
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Level Settings")]
        [SerializeField] private GameObject map;
        [SerializeField] private int[] numberHome;
        [SerializeField] private int countHome;
        [SerializeField] private int[] numberFuel;
        [SerializeField] private int countFuel;
        private int currentLevel;

        [Header("UI")]
        [SerializeField] private GameObject winPanel, losePanel;
        [SerializeField] private Text txtFuel;

        private bool isGameOver = false;

        [SerializeField] private GameObject homePanel;
        private string sceneName;

        void Awake()
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
                SetUp();
                LoadSceneManager.Instance.FadeInImage();
            }
        }

        private void SetUp()
        {
            currentLevel = ProgressManager.GetProgress(sceneName);

            if (currentLevel >= map.transform.childCount)
            {
                ProgressManager.SetProgress(sceneName, 0);
                currentLevel = 0;
            }

            map.transform.GetChild(currentLevel).gameObject.SetActive(true);
            txtFuel.text = "ENERRGY: " + numberFuel[currentLevel].ToString("D2");
        }

        public Vector3 GetPlayerPosition()
        {
            return map.transform.GetChild(currentLevel).GetChild(0).position;
        }
        public int GetCurrentLevel()
        {
            return currentLevel;
        }
        public void AddNumberHome()
        {
            countHome++;
        
        }
        public void CheckNumberHome()
        {
            if (countHome >= numberHome[currentLevel])
            {
                StartCoroutine(WinGame());
            }
        }
        public void CheckFuel()
        {
            countFuel++;
            txtFuel.text = "ENERRGY: " + (numberFuel[currentLevel] - countFuel).ToString("D2");
            if (countFuel >= numberFuel[currentLevel] && !isGameOver)
            {
                StartCoroutine(LoseGame());
            }

        }
        public IEnumerator WinGame()
        {
            isGameOver = true;
            yield return new WaitForSeconds(0.4f);
            winPanel.SetActive(true);

            currentLevel++;
            ProgressManager.SetProgress(sceneName, currentLevel);
            if (currentLevel >= map.transform.childCount)
            {
                ProgressManager.SetDone(sceneName);
                LoadSceneManager.Instance.ShowPanelDone();
                yield break;
            }
            NextLevel();
        }
        public IEnumerator LoseGame()
        {
            isGameOver = true;
            yield return new WaitForSeconds(0.4f);
            losePanel.SetActive(true);
            yield return new WaitForSeconds(1f);

            NextLevel();
        }

        public bool IsGameOver()
        {
            return isGameOver;
        }
        public void CheckGameWin()
        {
            isGameOver = true;
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

