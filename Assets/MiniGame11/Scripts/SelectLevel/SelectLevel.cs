using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MiniGame11
{
    public class SelectLevel : MonoBehaviour
    {
        [SerializeField] private GameObject[] levels;
        [SerializeField] private Sprite pickSprite, doneSprite;

        private void Awake()
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

                if (i < PlayerPrefs.GetInt("LevelUnlocked", 0))
                {
                    SetLevelUnlock(i);
                }
                else if (i == PlayerPrefs.GetInt("LevelUnlocked", 0))
                {
                    SetLevelPick(i);
                }
                else
                {
                    SetLevelLock(i);
                }
            }
        }
        private void SetLevelUnlock(int levelIndex)
        {
            levels[levelIndex].GetComponent<Image>().sprite = doneSprite;

            levels[levelIndex].GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectLevelGame(levelIndex);
            });

        }
        private void SetLevelPick(int levelIndex)
        {
            levels[levelIndex].GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectLevelGame(levelIndex);
            });
            levels[levelIndex].transform.GetChild(1).gameObject.SetActive(true);


        }
        private void SetLevelLock(int levelIndex)
        {
            levels[levelIndex].transform.GetComponent<Button>().interactable = false;
        }
        private void SelectLevelGame(int levelIndex)
        {
            PlayerPrefs.SetInt("Level", levelIndex);
            PlayerPrefs.Save();
            SceneManager.LoadScene("MainGame");
        }
    }

}
