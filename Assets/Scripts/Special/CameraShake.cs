using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("기본 흔들림 설정")]
    [Tooltip("인스펙터에서 기본 강도를 조절할 수 있습니다 (수치가 클수록 폭이 큼)")]
    [Range(0.01f, 2.0f)]
    public float defaultMagnitude = 0.2f;

    [Tooltip("흔들리는 속도/주기 (수치가 클수록 덜덜거리는 주기가 빠름)")]
    [SerializeField] private float shakeFrequency = 35f;

    private Vector3 originalPos;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        originalPos = transform.localPosition;
    }
    public void ShakeForDuration(float duration, float magnitude = -1f)
    {
        if (magnitude < 0f) magnitude = defaultMagnitude;

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) * 2f - 1f) * magnitude;
            float offsetY = (Mathf.PerlinNoise(0f, Time.time * shakeFrequency) * 2f - 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + offsetX, originalPos.y + offsetY, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        shakeCoroutine = null;
    }
}