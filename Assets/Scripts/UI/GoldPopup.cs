using UnityEngine;
using TMPro; 

public class GoldPopup : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TMP_Text goldText;      
    [SerializeField] private CanvasGroup canvasGroup; 
    [SerializeField] private Transform coinTransform; 

    [Header("위치 및 연출 설정")]
    [SerializeField] private float moveDistance = 1.2f; 
    [SerializeField] private float duration = 0.8f; 
    [SerializeField] private float fadeStartRatio = 0.5f; 

    [Header("동전 3D 회전 연출 설정")]
    [Tooltip("Y축을 회전시켜 동전이 3D처럼 팽팽 뒤집히게 만듭니다.")]
    [SerializeField] private float flipRotateSpeed = 720f; 

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float timer = 0f;

    public void Setup(int goldAmount)
    {
        if (goldText != null)
        {
            goldText.text = $"+{goldAmount}";
        }

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        startPosition = transform.position;
        targetPosition = startPosition + Vector3.up * moveDistance;

        Destroy(gameObject, duration);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / duration);

        float easeOutProgress = 1f - Mathf.Pow(1f - progress, 3); 
        transform.position = Vector3.Lerp(startPosition, targetPosition, easeOutProgress);

        if (coinTransform != null)
        {
            coinTransform.Rotate(0f, flipRotateSpeed * Time.deltaTime, 0f);
        }

        if (canvasGroup != null && progress >= fadeStartRatio)
        {
            float fadeProgress = (progress - fadeStartRatio) / (1f - fadeStartRatio);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeProgress);
        }
    }
}