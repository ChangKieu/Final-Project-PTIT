using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Minigame1 
{ 
    public class SelectLevel : MonoBehaviour
    {
        [SerializeField] private GameObject[] levels;
        [SerializeField] private Sprite lockSprite;

        private void Awake()
        {
            SetUpLevel();
        }

        private void SetUpLevel()
        {
            levels = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                levels[i] = transform.GetChild(i).gameObject;
            }
            for (int i = 0; i < levels.Length; i++)
            {
                int indexLevel = i;

                levels[i].transform.GetChild(0).GetComponent<Text>().text = (i + 1).ToString();

                if (i <= PlayerPrefs.GetInt("LevelUnlocked", 0))
                {
                    SetLevelUnlock(i);
                }

                else
                {
                    SetLevelLock(i);
                }
            }
        }

        private void SetLevelUnlock(int levelIndex)
        {
            levels[levelIndex].GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectLevelGame(levelIndex);
            });

        }
        private void SetLevelLock(int levelIndex)
        {
            levels[levelIndex].GetComponent<Image>().sprite = lockSprite;
            levels[levelIndex].transform.GetChild(1).gameObject.SetActive(true);
            levels[levelIndex].transform.GetChild(0).gameObject.SetActive(false);

            levels[levelIndex].transform.GetComponent<Button>().enabled = false;
        }
        private void SelectLevelGame(int levelIndex)
        {
            PlayerPrefs.SetInt("Level", levelIndex);
            PlayerPrefs.Save();
            SceneManager.LoadScene("MainGame");
        }
    }

}