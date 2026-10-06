using System;
using UnityEngine;
using UnityEngine.UI;

public class GaugeBar : MonoBehaviour
{
    [Header("UI 연결")]
    public Image redGaugeImage;

    private float currentProgress = 0f;
    private float targetProductionTime = 1f;
    private bool isProducing = false;

    private Action onCompleteCallback;

    void Start()
    {
        if (redGaugeImage == null)
            redGaugeImage = GetComponent<Image>();

        if (redGaugeImage != null)
            redGaugeImage.fillAmount = 0f;
    }

    void Update()
    {
        if (isProducing)
        {
            UpdateProductionProgress();
        }
    }

    public void StartProduction(float buildTime, Action onComplete)
    {
        targetProductionTime = Mathf.Max(0.1f, buildTime);
        currentProgress = 0f;
        onCompleteCallback = onComplete;
        isProducing = true;

        if (redGaugeImage != null)
            redGaugeImage.fillAmount = 0f;
    }

    private void UpdateProductionProgress()
    {
        currentProgress += Time.deltaTime;
        float fillRatio = currentProgress / targetProductionTime;

        if (redGaugeImage != null)
            redGaugeImage.fillAmount = Mathf.Clamp01(fillRatio);

        if (currentProgress >= targetProductionTime)
        {
            CompleteProduction();
        }
    }

    private void CompleteProduction()
    {
        isProducing = false;

        if (redGaugeImage != null)
            redGaugeImage.fillAmount = 0f;

        onCompleteCallback?.Invoke();
        onCompleteCallback = null;
    }

    public bool IsProducing => isProducing;
}