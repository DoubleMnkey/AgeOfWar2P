using System.Collections;
using UnityEngine;

public class FutureLaserBeam : MonoBehaviour
{
    public LayerMask targetLayer;
    public GameObject impactEffect;

    public float damage = 500f;
    public float lifeTime = 0.25f; 

    [Header("강렬한 연출 설정")]
    public float initialScaleX = 2.5f; 

    [Header("화면 흔들림 강도")]
    public float shakeMagnitude = 0.35f; 

    [Header("사운드 설정")]
    [SerializeField] private AudioClip laserSFX;  
    [SerializeField] private AudioClip impactSFX; 

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (SoundManager.Instance != null)
        {
            if (laserSFX != null) SoundManager.Instance.PlaySFX(laserSFX);
            if (impactSFX != null) SoundManager.Instance.PlaySFX(impactSFX);
        }

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeForDuration(lifeTime, shakeMagnitude);
        }

        transform.localScale = new Vector3(initialScaleX, transform.localScale.y, transform.localScale.z);

        if (impactEffect != null)
        {
            Instantiate(
                impactEffect,
                new Vector3(transform.position.x, -7.77f, 0f),
                Quaternion.identity);
        }

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position,
            new Vector2(1.5f, 25f),
            0,
            targetLayer);

        foreach (Collider2D hit in hits)
        {
            UnitControl unit = hit.GetComponent<UnitControl>();
            if (unit != null && !unit.IsDead)
            {
                unit.TakeDamage(damage);
            }
        }

        StartCoroutine(LaserFadeRoutine());

        Destroy(gameObject, lifeTime);
    }

    private IEnumerator LaserFadeRoutine()
    {
        float timer = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = new Vector3(0.1f, transform.localScale.y, transform.localScale.z);

        while (timer < lifeTime)
        {
            timer += Time.deltaTime;
            float progress = timer / lifeTime;

            transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(1f, 0f, progress);
                spriteRenderer.color = color;
            }

            yield return null;
        }
    }
}