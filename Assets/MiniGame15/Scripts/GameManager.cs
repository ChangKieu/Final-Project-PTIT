using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiniGame15
{
    [System.Serializable]
    public class LevelData
    {
        public Sprite[] listMaKhoaSprites;
        public string[] listChuoiMa;
        public Sprite[] listChuoiMaSprites;
        public string answer;
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Level Data")]
        [SerializeField] private LevelData[] levels;
        private LevelData curData;

        [Header("UI Containers")]
        [SerializeField] private Transform maKhoaContainer;
        [SerializeField] private Transform answerMaKhoa;     // Slot Step 1
        [SerializeField] private Transform chuoiMaContainer;
        [SerializeField] private Transform answerChuoiMa;    // Step 2

        [SerializeField] private InputField answer;

        [Header("Steps")]
        [SerializeField] private GameObject step1, step2, step3;
        [SerializeField] private Button btnCheck1, btnCheck2, btnCheck3;

        private int currentLevel = 0;

        private List<Vector2> originPos_maKhoa = new List<Vector2>();
        private List<Vector2> originPos_chuoiMa = new List<Vector2>();
        private List<Vector2> originPos_answerChuoiMa = new List<Vector2>();

        private int[] shuffleIndex;
        [SerializeField] private GameObject homePanel, winEffect;
        private string sceneName;
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
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

            if (currentLevel >= levels.Length)
            {
                ProgressManager.SetProgress(sceneName, 0);
                currentLevel = 0;
            }
            btnCheck1.onClick.AddListener(CheckStep1);
            btnCheck2.onClick.AddListener(CheckStep2);
            btnCheck3.onClick.AddListener(CheckStep3);

            LoadLevel(currentLevel);
        }
        private void LoadLevel(int id)
        {
            curData = levels[id];

            step1.SetActive(true);
            step2.SetActive(false);
            step3.SetActive(false);

            InitUI();
            ShufflePositions();
            SetupDragStep1();
            SetupDragStep2();
        }

        private void InitUI()
        {
            originPos_maKhoa.Clear();
            originPos_chuoiMa.Clear();
            originPos_answerChuoiMa.Clear();

            for (int i = 0; i < curData.listChuoiMaSprites.Length; i++)
            {
                maKhoaContainer.GetChild(i).gameObject.SetActive(true);
                answerMaKhoa.GetChild(i).gameObject.SetActive(true);
                chuoiMaContainer.GetChild(i).gameObject.SetActive(true);
                answerChuoiMa.GetChild(i).gameObject.SetActive(true);

                maKhoaContainer.GetChild(i).GetComponent<Image>().sprite = curData.listMaKhoaSprites[i];
                chuoiMaContainer.GetChild(i).GetComponentInChildren<Text>().text = curData.listChuoiMa[i];
                answerChuoiMa.GetChild(i).GetComponent<Image>().sprite = curData.listChuoiMaSprites[i];

                originPos_maKhoa.Add(maKhoaContainer.GetChild(i).GetComponent<RectTransform>().anchoredPosition);
                originPos_chuoiMa.Add(chuoiMaContainer.GetChild(i).GetComponent<RectTransform>().anchoredPosition);
                originPos_answerChuoiMa.Add(answerChuoiMa.GetChild(i).GetComponent<RectTransform>().anchoredPosition);
            }
        }

        private void ShufflePositions()
        {
            int count = curData.listChuoiMaSprites.Length;
            shuffleIndex = GenerateShuffle(count);

            for (int i = 0; i < count; i++)
            {
                maKhoaContainer.GetChild(i).GetComponent<RectTransform>().anchoredPosition =
                    originPos_maKhoa[shuffleIndex[i]];

                chuoiMaContainer.GetChild(i).GetComponent<RectTransform>().anchoredPosition =
                    originPos_chuoiMa[shuffleIndex[i]];

                answerChuoiMa.GetChild(i).GetComponent<RectTransform>().anchoredPosition =
                    originPos_answerChuoiMa[shuffleIndex[i]];
            }
        }

        private int[] GenerateShuffle(int n)
        {
            int[] arr = new int[n];
            for (int i = 0; i < n; i++) arr[i] = i;

            for (int i = 0; i < n; i++)
            {
                int r = Random.Range(i, n);
                (arr[i], arr[r]) = (arr[r], arr[i]);
            }
            return arr;
        }

        // ======================== STEP 1 =============================
        private void SetupDragStep1()
        {
            for (int i = 0; i < curData.listChuoiMaSprites.Length; i++)
            {
                var drag = maKhoaContainer.GetChild(i).GetComponent<ItemDragStep1>();
                if (drag == null) drag = maKhoaContainer.GetChild(i).gameObject.AddComponent<ItemDragStep1>();
                drag.index = i;
            }
        }

        public Transform GetSlotAtStep1(Vector2 screenPos)
        {
            PointerEventData ped = new PointerEventData(EventSystem.current);
            ped.position = screenPos;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ped, results);

            foreach (var r in results)
            {
                foreach (Transform slot in answerMaKhoa)
                {
                    if (r.gameObject.transform == slot || r.gameObject.transform.IsChildOf(slot))
                    {
                        return slot;
                    }
                }
            }

            return null;
        }




        public void CheckStep1()
        {
            int correct = 0;

            for (int i = 0; i < curData.listChuoiMaSprites.Length; i++)
            {
                ItemDragStep1 item = maKhoaContainer.GetChild(i).GetComponent<ItemDragStep1>();

                if (item.currentSlot != null)
                {
                    if (item.currentSlot.GetSiblingIndex() == item.index)
                    {
                        correct++;
                    }
                }
            }

            if (correct == curData.listChuoiMaSprites.Length)
            {
                btnCheck1.gameObject.SetActive(false);
                step2.SetActive(true);
                for (int i = 0; i < curData.listChuoiMaSprites.Length; i++)
                {
                    chuoiMaContainer.GetChild(i)
                        .GetComponent<RectTransform>().anchoredPosition = originPos_chuoiMa[i];
                }
            }
            else
            {
                ResetStep1Items();
            }
        }

        private void ResetStep1Items()
{
    for (int i = 0; i < curData.listChuoiMaSprites.Length; i++)
    {
        var rect = maKhoaContainer.GetChild(i).GetComponent<RectTransform>();
        rect.anchoredPosition = originPos_maKhoa[shuffleIndex[i]];

        maKhoaContainer.GetChild(i).GetComponent<ItemDragStep1>().currentSlot = null;
    }
}


        private void SetupDragStep2()
        {
            for (int i = 0; i < curData.listChuoiMaSprites.Length; i++)
            {
                var drag = answerChuoiMa.GetChild(i).GetComponent<ItemSwapStep2>();
                if (drag == null) drag = answerChuoiMa.GetChild(i).gameObject.AddComponent<ItemSwapStep2>();
                drag.index = i;
            }
        }

        public Transform GetSwapTargetStep2(Vector2 screenPos)
        {
            PointerEventData ped = new PointerEventData(EventSystem.current);
            ped.position = screenPos;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ped, results);

            foreach (var r in results)
            {
                foreach (Transform slot in answerChuoiMa)
                {
                    if (r.gameObject.transform == slot || r.gameObject.transform.IsChildOf(slot))
                    {
                        return slot;
                    }
                }
            }

            return null;
        }

        public void CheckStep2()
        {
            for (int i = 0; i < curData.listChuoiMaSprites.Length; i++)
            {
                Vector2 pos = answerChuoiMa.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
                if (pos != originPos_answerChuoiMa[i])
                {
                    return;
                }
            }

            btnCheck2.gameObject.SetActive(false);
            step3.SetActive(true);
        }

        public void CheckStep3()
        {
            string textAnswer = Normalize(answer.text);
            string curDataAnswer = Normalize(curData.answer);


            if (textAnswer == curDataAnswer)
            {
                winEffect.SetActive(true);

                currentLevel++;
                ProgressManager.SetProgress(sceneName, currentLevel);
                if (currentLevel >= levels.Length)
                {
                    ProgressManager.SetDone(sceneName);
                    LoadSceneManager.Instance.ShowPanelDone();
                    return;
                }
                NextLevel();
            }
            else
            {
                answer.text = "";
            }
        }

        private string Normalize(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";

            s = s.ToLowerInvariant();

            string normalizedFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (char c in normalizedFormD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            string withoutDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

            string cleaned = Regex.Replace(withoutDiacritics, @"[^a-z0-9]+", "");

            return cleaned;
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
