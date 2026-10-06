using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class SettingsManager : MonoBehaviour
{
    [Header("오디오 믹서 (선택 사항)")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("UI 컨트롤러 요소 연결 (순서대로)")]
    [SerializeField] private Selectable[] settingItems;

    [Header("선택 영역 하이라이트 박스 (Image)")]
    [SerializeField] private RectTransform highlightBox;
    [SerializeField] private Vector2 boxPadding = new Vector2(20f, 10f); 

    [Header("특정 UI 슬라이더 / 토글 직접 연결")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullscreenToggle;

    private const string MASTER_KEY = "MasterVolume";
    private const string BGM_KEY = "BGMVolume";
    private const string SFX_KEY = "SFXVolume";
    private const string FULLSCREEN_KEY = "IsFullscreen";

    private int selectedIndex = 0;

    private void OnEnable()
    {
        selectedIndex = 0;
        StartCoroutine(InitHighlightNextFrame());
    }

    private IEnumerator InitHighlightNextFrame()
    {
        yield return new WaitForEndOfFrame();
        UpdateHighlightPosition();
    }

    private void Start()
    {
        float master = PlayerPrefs.GetFloat(MASTER_KEY, 0.8f);
        float bgm = PlayerPrefs.GetFloat(BGM_KEY, 0.8f);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, 0.8f);
        bool isFullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;

        if (masterSlider != null) masterSlider.value = master;
        if (bgmSlider != null) bgmSlider.value = bgm;
        if (sfxSlider != null) sfxSlider.value = sfx;
        if (fullscreenToggle != null) fullscreenToggle.isOn = isFullscreen;

        if (masterSlider != null) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (bgmSlider != null) bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        if (fullscreenToggle != null) fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        SetFullscreen(isFullscreen);
        SetMasterVolume(master);
        SetBGMVolume(bgm);
        SetSFXVolume(sfx);
    }

    private void Update()
    {
        HandleNavigation();
        HandleValueChange();
    }
    private void HandleNavigation()
    {
        if (settingItems == null || settingItems.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            selectedIndex = (selectedIndex - 1 + settingItems.Length) % settingItems.Length;
            UpdateHighlightPosition();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            selectedIndex = (selectedIndex + 1) % settingItems.Length;
            UpdateHighlightPosition();
        }
    }

    private void HandleValueChange()
    {
        if (settingItems == null || settingItems.Length == 0) return;

        Selectable currentItem = settingItems[selectedIndex];

        if (currentItem is Slider slider)
        {
            float step = (slider.maxValue - slider.minValue) * 0.05f;

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                slider.value = Mathf.Clamp(slider.value - step, slider.minValue, slider.maxValue);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                slider.value = Mathf.Clamp(slider.value + step, slider.minValue, slider.maxValue);
            }
        }
        else if (currentItem is Toggle toggle)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                toggle.isOn = !toggle.isOn;
            }
        }
    }

    private void UpdateHighlightPosition()
    {
        if (settingItems == null || settingItems.Length == 0 || selectedIndex >= settingItems.Length) return;

        Selectable currentItem = settingItems[selectedIndex];

        if (highlightBox != null && currentItem != null)
        {
            RectTransform rowTarget = currentItem.transform.parent as RectTransform;

            if (rowTarget != null)
            {
                highlightBox.position = rowTarget.position;
            }
            else
            {
                RectTransform currentTarget = currentItem.GetComponent<RectTransform>();
                highlightBox.position = currentTarget.position;
                highlightBox.sizeDelta = currentTarget.rect.size + boxPadding;
            }
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(currentItem.gameObject);
        }
    }

    public void SetMasterVolume(float value)
    {
        PlayerPrefs.SetFloat(MASTER_KEY, value);
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMasterVolume(value);
        }
        else
        {
            AudioListener.volume = value;
        }
    }

    public void SetBGMVolume(float value)
    {
        PlayerPrefs.SetFloat(BGM_KEY, value);
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGMVolume(value);
        }
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat(SFX_KEY, value);
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetSFXVolume(value);
        }
    }

    public void SetFullscreen(bool isFullscreen)
{
    PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullscreen ? 1 : 0);

    if (isFullscreen)
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
    }
    else
    {
        Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
    }
}
}