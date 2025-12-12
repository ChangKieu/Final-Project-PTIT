using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager Instance;

    [SerializeField] private GameObject panelTransition;
    [SerializeField] private Material instanceMaterial;
    [SerializeField] private GameObject panelDone;
    private Image imgTransition;

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

        imgTransition = panelTransition.GetComponent<Image>();
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
    public void LoadSceneImg(string sceneName)
    {
        FadeOutImage(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        });
    }

    public void FadeIn()
    {
        panelTransition.SetActive(true);
        UseRadiusMaterial();
        instanceMaterial.SetFloat("_Radius", 0f);

        Color c = imgTransition.color;
        c.a = 1f;
        imgTransition.color = c;

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
        UseRadiusMaterial();
        instanceMaterial.SetFloat("_Radius", 0.3f);

        Color c = imgTransition.color;
        c.a = 1f;
        imgTransition.color = c;

        DOTween.To(
            () => instanceMaterial.GetFloat("_Radius"),
            x => instanceMaterial.SetFloat("_Radius", x),
            0f, 1f
        ).OnComplete(() => onDone?.Invoke());
    }
    public void FadeInImage()
    {
        float duration = 1f;
        panelTransition.SetActive(true);
        UseDefaultMaterial();
        Color c = imgTransition.color;
        c.a = 1f;
        imgTransition.color = c;

        imgTransition.DOFade(0f, duration).OnComplete(() =>
        {
            panelTransition.SetActive(false);
        });
    }

    public void FadeOutImage(System.Action onDone)
    {
        float duration = 1f;
        panelTransition.SetActive(true);
        UseDefaultMaterial();

        Color c = imgTransition.color;
        c.a = 0f;
        imgTransition.color = c;

        imgTransition.DOFade(1f, duration).OnComplete(() =>
        {
            onDone?.Invoke();
        });
    }
    private void UseRadiusMaterial() => imgTransition.material = instanceMaterial;
    private void UseDefaultMaterial() => imgTransition.material = null;


    public void ShowPanelDone()
    {
        panelDone.SetActive(true);
    }
    public void LoadHome()
    {
        panelDone.SetActive(false);

        LoadScene("Home");
    }
}
