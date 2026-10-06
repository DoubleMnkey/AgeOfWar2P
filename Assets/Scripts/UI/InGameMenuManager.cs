using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class InGameMenuManager : MonoBehaviour
{
    [Header("인게임 메뉴 전체 루트 (ESC로 켜고 끄는 최상위 Canvas/Panel)")]
    [SerializeField] private GameObject pauseMenuRoot;

    [Header("메뉴 버튼 컨테이너 (Resume, HowToPlay, Settings, Quit 버튼을 담은 부모)")]
    [SerializeField] private GameObject buttonContainer;

    [Header("버튼 리스트 (0: Resume, 1: How to Play, 2: Setting, 3: Quit)")]
    [SerializeField] private RectTransform[] menuButtons;

    [Header("선택 표시용 칼 이미지")]
    [SerializeField] private RectTransform swordPointer;
    [SerializeField] private float pointerOffsetX = -30f;

    [Header("이동할 타이틀 씬 이름")]
    [SerializeField] private string titleSceneName = "TitleScene"; 

    [Header("UI 서브 패널 설정")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject howToPlayPanel;

    [Header("마우스 커서 설정")]
    [SerializeField] private bool hideMouseCursor = true;

    private int selectedIndex = 0;
    private bool isPaused = false;

    private void Start()
    {
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(false);
        CloseAllSubPanels();

        if (hideMouseCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void OnOpenMenu()
    {
        isPaused = true;
        selectedIndex = 0;

        CloseAllSubPanels();

        StopAllCoroutines();
        StartCoroutine(InitPointerPositionNextFrame());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsAnySubPanelActive())
            {
                CloseAllSubPanels();
            }
            return;
        }

        if (!isPaused || IsAnySubPanelActive()) return;

        HandleNavigation();
        HandleConfirmation();
    }

    private IEnumerator InitPointerPositionNextFrame()
    {
        yield return new WaitForEndOfFrame();
        UpdatePointerPosition();
    }

    private void HandleNavigation()
    {
        if (menuButtons == null || menuButtons.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            selectedIndex = (selectedIndex - 1 + menuButtons.Length) % menuButtons.Length;
            UpdatePointerPosition();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            selectedIndex = (selectedIndex + 1) % menuButtons.Length;
            UpdatePointerPosition();
        }
    }

    private void HandleConfirmation()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteMenu(selectedIndex);
        }
    }

    private void UpdatePointerPosition()
    {
        if (menuButtons == null || menuButtons.Length == 0 || selectedIndex >= menuButtons.Length) return;

        RectTransform currentButton = menuButtons[selectedIndex];

        if (swordPointer != null && currentButton != null)
        {
            if (!swordPointer.gameObject.activeSelf)
                swordPointer.gameObject.SetActive(true);

            Canvas parentCanvas = swordPointer.GetComponentInParent<Canvas>();
            float scaleFactor = (parentCanvas != null) ? parentCanvas.scaleFactor : 1f;

            float buttonLeftX = currentButton.position.x - (currentButton.rect.width * currentButton.lossyScale.x * currentButton.pivot.x);

            swordPointer.position = new Vector3(buttonLeftX + (pointerOffsetX * scaleFactor), currentButton.position.y, currentButton.position.z);
        }

        if (EventSystem.current != null && currentButton != null)
        {
            EventSystem.current.SetSelectedGameObject(currentButton.gameObject);
        }
    }

    private void ExecuteMenu(int index)
    {
        switch (index)
        {
            case 0: 
                MenuManager menuManager = GetComponentInParent<MenuManager>();
                if (menuManager == null)
                {
                    menuManager = FindObjectOfType<MenuManager>();
                }

                if (menuManager != null)
                {
                    menuManager.ToggleSettings();
                }
                else
                {
                    isPaused = false;
                    Time.timeScale = 1f;
                    if (pauseMenuRoot != null) pauseMenuRoot.SetActive(false);
                }
                break;

            case 1: 
                if (howToPlayPanel != null) OpenSubPanel(howToPlayPanel);
                break;

            case 2: 
                if (settingsPanel != null) OpenSubPanel(settingsPanel);
                break;

            case 3: 
                Time.timeScale = 1f; 
                SceneManager.LoadScene(titleSceneName);
                break;
        }
    }

    private void OpenSubPanel(GameObject panel)
    {
        panel.SetActive(true);

        if (buttonContainer != null) buttonContainer.SetActive(false);
        if (swordPointer != null) swordPointer.gameObject.SetActive(false);
    }
    public void CloseAllSubPanels()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);

        if (buttonContainer != null) buttonContainer.SetActive(true);
        if (swordPointer != null) swordPointer.gameObject.SetActive(true);

        if (isPaused)
        {
            StopAllCoroutines();
            StartCoroutine(InitPointerPositionNextFrame());
        }
    }

    public bool IsAnySubPanelActive()
    {
        return (settingsPanel != null && settingsPanel.activeInHierarchy) ||
               (howToPlayPanel != null && howToPlayPanel.activeInHierarchy);
    }

}