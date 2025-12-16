using UnityEngine;
using UnityEngine.UI;

public class UIButtonSelect : MonoBehaviour
{
    private Button[] listButton;
    [SerializeField] private Sprite onButton, offButton;
    [SerializeField] private Slider bgmSlider, sfxSlider;

    private void Awake()
    {
        listButton = new Button[2];
        listButton = GetComponentsInChildren<Button>();
        SetButtonActive(0);
        LoadSceneManager.Instance.FadeIn();
    }
    private void Start()
    {
        AudioManager.Instance.SetUpAudio(bgmSlider, sfxSlider);
    }
    public void SetButtonActive(int index)
    {
        for (int i = 0; i < listButton.Length; i++)
        {
            if (i == index)
            {
                listButton[i].image.sprite = onButton;
                listButton[i].GetComponentInChildren<Text>().color = Color.white;
            }
            else
            {
                listButton[i].image.sprite = offButton;
                listButton[i].GetComponentInChildren<Text>().color = Color.black;
            }
        }
    }
}
