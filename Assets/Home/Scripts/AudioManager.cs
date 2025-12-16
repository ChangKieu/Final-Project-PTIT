using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip sfxClick, sfxDone, sfxPlace, sfxMove, sfxMagic, sfxCollision;
    [SerializeField] private AudioClip sfxJump;
    [SerializeField] private AudioClip sfxWin, sfxLose;


    [SerializeField] private Slider sliderBGM;
    [SerializeField] private Slider sliderSFX;

    private const string PP_BGM = "BGM_VOL";
    private const string PP_SFX = "SFX_VOL";

    private float bgmVol = 0.5f;
    private float sfxVol = 1.0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);

        bgmVol = PlayerPrefs.GetFloat(PP_BGM, 0.5f);
        sfxVol = PlayerPrefs.GetFloat(PP_SFX, 1.0f);
        ApplyVolumes();
    }

    public void SetUpAudio(Slider bgm, Slider sfx)
    {
        sliderBGM = bgm;
        sliderSFX = sfx;
        sliderBGM.minValue = 0f;
        sliderBGM.maxValue = 1f;
        sliderBGM.SetValueWithoutNotify(GetBGMVolume());
        sliderBGM.onValueChanged.AddListener(OnBGMChanged);

        sliderSFX.minValue = 0f; sliderSFX.maxValue = 1f;
        sliderSFX.SetValueWithoutNotify(GetSFXVolume());
        sliderSFX.onValueChanged.AddListener(OnSFXChanged);
    }

    private void OnDisable()
    {
        if (sliderBGM != null) sliderBGM.onValueChanged.RemoveListener(OnBGMChanged);
        if (sliderSFX != null) sliderSFX.onValueChanged.RemoveListener(OnSFXChanged);
    }

    private void OnBGMChanged(float v)
    {
        SetBGMVolume(v);
    }

    private void OnSFXChanged(float v)
    {
        SetSFXVolume(v);
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        bgmSource.loop = loop;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVol);
    }

    public void SetBGMVolume(float v)
    {
        bgmVol = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(PP_BGM, bgmVol);
        PlayerPrefs.Save();
        ApplyVolumes();
    }

    public void SetSFXVolume(float v)
    {
        sfxVol = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(PP_SFX, sfxVol);
        PlayerPrefs.Save();
        ApplyVolumes();
    }

    public float GetBGMVolume() => bgmVol;
    public float GetSFXVolume() => sfxVol;

    private void ApplyVolumes()
    {
        if (bgmSource) bgmSource.volume = bgmVol;
        if (sfxSource) sfxSource.volume = sfxVol;
    }

    public void PlayClick() { PlaySFX(sfxClick); }
    public void PlayCollision() { PlaySFX(sfxCollision); }
    public void PlayDone() { PlaySFX(sfxDone); }
    public void PlayMove() { PlaySFX(sfxMove); }
    public void PlayPlace() { PlaySFX(sfxPlace); }
    public void PlayJump() { PlaySFX(sfxJump); }
    public void PlayMagic() { PlaySFX(sfxMagic); }
    public void PlayLose() { PlaySFX(sfxLose); }

    public void PlayWin() { PlaySFX(sfxWin); }
}