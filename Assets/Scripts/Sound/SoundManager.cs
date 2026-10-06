using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("오디오 소스 연결")]
    [SerializeField] private AudioSource bgmSource; 
    [SerializeField] private AudioSource sfxSource; 

    [Header("BGM 음원 목록")]
    [SerializeField] private AudioClip bgmClip; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 0.8f));
        SetBGMVolume(PlayerPrefs.GetFloat("BGMVolume", 0.8f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 0.8f));

        PlayBGM(bgmClip);
    }

    private void InitAudioSources()
    {
        if (bgmSource != null)
        {
            bgmSource.loop = true;
            bgmSource.spatialBlend = 0f; 
        }

        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.spatialBlend = 0f; 
        }
    }
    public void SetMasterVolume(float value)
    {
        AudioListener.volume = Mathf.Clamp01(value); 
    }

    public void SetBGMVolume(float value)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = Mathf.Clamp01(value); 
        }
    }

    public void SetSFXVolume(float value)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = Mathf.Clamp01(value);
        }
    }
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.Play();
    }
    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.PlayOneShot(clip, volumeScale);
    }
}