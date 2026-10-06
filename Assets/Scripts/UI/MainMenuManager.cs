using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [Header("메뉴 버튼 컨테이너 (버튼들을 담고 있는 부모 오브젝트)")]
    [SerializeField] private GameObject buttonContainer;

    [Header("버튼 리스트 (위에서 아래 순서)")]
    [SerializeField] private RectTransform[] menuButtons;

    [Header("선택 표시용 칼 이미지")]
    [SerializeField] private RectTransform swordPointer;
    [SerializeField] private float pointerOffsetX = -30f;

    [Header("이동 씬 이름 설정")]
    [SerializeField] private string playSceneName = "GameScene";

    [Header("UI 패널 설정")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject howToPlayPanel;

    [Header("기타 설정")]
    [SerializeField] private bool hideMouseCursor = true;

    private int selectedIndex = 0;

    private void Start()
    {
        if (hideMouseCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        StartCoroutine(InitPointerPositionNextFrame());
    }

    private IEnumerator InitPointerPositionNextFrame()
    {
        yield return new WaitForEndOfFrame();
        UpdatePointerPosition();
    }

    private void Update()
    {
        if (IsAnyPanelActive())
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseAllPanels();
            }
            return;
        }

        HandleNavigation();
        HandleConfirmation();
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

        if (swordPointer != null && swordPointer.gameObject.activeInHierarchy)
        {
            Vector3[] buttonCorners = new Vector3[4];
            currentButton.GetWorldCorners(buttonCorners);

            float leftX = buttonCorners[0].x;
            float centerY = (buttonCorners[0].y + buttonCorners[1].y) * 0.5f;

            Canvas canvas = swordPointer.GetComponentInParent<Canvas>();
            float scaleFactor = (canvas != null) ? canvas.scaleFactor : 1f;

            swordPointer.position = new Vector3(leftX + (pointerOffsetX * scaleFactor), centerY, currentButton.position.z);
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(currentButton.gameObject);
        }
    }

    private void ExecuteMenu(int index)
    {
        switch (index)
        {
            case 0:
                SceneManager.LoadScene(playSceneName);
                break;

            case 1:
                if (howToPlayPanel != null)
                {
                    OpenPanel(howToPlayPanel);
                }
                break;

            case 2:
                if (settingsPanel != null)
                {
                    OpenPanel(settingsPanel);
                }
                break;

            case 3:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }
    }

    private void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);

        if (buttonContainer != null)
        {
            buttonContainer.SetActive(false);
        }

        if (swordPointer != null)
        {
            swordPointer.gameObject.SetActive(false);
        }
    }

    private void CloseAllPanels()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);

        if (buttonContainer != null)
        {
            buttonContainer.SetActive(true);
        }

        if (swordPointer != null)
        {
            swordPointer.gameObject.SetActive(true);
        }

        StartCoroutine(InitPointerPositionNextFrame());
    }

    private bool IsAnyPanelActive()
    {
        return (settingsPanel != null && settingsPanel.activeSelf) ||
               (howToPlayPanel != null && howToPlayPanel.activeSelf);
    }
}