using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    [Header("타이틀 씬 이름")]
    [SerializeField] private string titleSceneName = "TitleMenu";

    [Header("입력 대기 시간 (초)")]
    [SerializeField] private float inputDelay = 0.5f; 

    private bool canInput = false;

    private void OnEnable()
    {
        canInput = false;
        Invoke(nameof(EnableInput), inputDelay);
    }

    private void EnableInput()
    {
        canInput = true;
    }

    private void Update()
    {
        if (!canInput) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            ReturnToTitle();
        }
    }

    private void ReturnToTitle()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(titleSceneName);
    }
}