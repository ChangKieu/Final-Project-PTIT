using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            currentLevel = PlayerPrefs.GetInt("Level", 0);
            if (currentLevel >= map.transform.childCount)
            {
                currentLevel = 0;
                PlayerPrefs.SetInt("Level", 0);
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
            int levelIndex = PlayerPrefs.GetInt("Level", 0) + 1;
            if (PlayerPrefs.GetInt("LevelUnlocked", 0) < levelIndex)
            {
                PlayerPrefs.SetInt("LevelUnlocked", levelIndex);
            }
            PlayerPrefs.SetInt("Level", levelIndex);
            PlayerPrefs.Save();
            yield return new WaitForSeconds(1f);

            NextLevel();
        }
        public IEnumerator LoseGame()
        {
            isGameOver = true;
            yield return new WaitForSeconds(0.4f);
            losePanel.SetActive(true);
            yield return new WaitForSeconds(1f);

            ReloadLevel();
        }

        public bool IsGameOver()
        {
            return isGameOver;
        }
        public void CheckGameWin()
        {
            isGameOver = true;
        }
        public void ReloadLevel()
        {
            PlayerPrefs.SetInt("Level", currentLevel);
            PlayerPrefs.Save();
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        public void NextLevel()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

    }

}

