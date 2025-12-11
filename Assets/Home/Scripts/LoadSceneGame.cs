using DG.Tweening;
using UnityEngine;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager Instance;

    [SerializeField] private GameObject panelTransition;
    [SerializeField] private Material instanceMaterial;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        FadeIn();
    }

    public void LoadScene(string sceneName)
    {
        FadeOut(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        });
    }

    public void FadeIn()
    {
        panelTransition.SetActive(true);
        instanceMaterial.SetFloat("_Radius", 0f);

        DOTween.To(
            () => instanceMaterial.GetFloat("_Radius"),
            x => instanceMaterial.SetFloat("_Radius", x),
            0.3f, 1f
        ).OnComplete(() =>
        {
            panelTransition.SetActive(false);
        });
    }

    private void FadeOut(System.Action onDone)
    {
        panelTransition.SetActive(true);
        instanceMaterial.SetFloat("_Radius", 0.3f);

        DOTween.To(
            () => instanceMaterial.GetFloat("_Radius"),
            x => instanceMaterial.SetFloat("_Radius", x),
            0f, 1f
        ).OnComplete(() => onDone?.Invoke());
    }
}
