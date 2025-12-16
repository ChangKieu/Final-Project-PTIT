using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayClickSound : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        AudioManager.Instance.PlayClick();
    }
}
