using UnityEngine;

public class HealEffectFollower : MonoBehaviour
{
    private Transform targetUnit;
    private SpriteRenderer targetSpriteRenderer;
    private SpriteRenderer mySpriteRenderer;

    [Header("크기 조절 설정")]
    [Tooltip("유닛 폭 대비 이펙트 가로 비율 (예: 1.0 = 정확히 맞춤)")]
    [SerializeField] private float widthMultiplier = 1.0f;

    [Tooltip("유닛 높이 대비 이펙트 세로 비율")]
    [SerializeField] private float heightMultiplier = 1.0f;

    [Header("위치 미세 조절")]
    [Tooltip("발밑 위치 기준 Y축 보정값")]
    [SerializeField] private float yOffset = 0.0f;

    public void Setup(Transform unitTransform)
    {
        targetUnit = unitTransform;

        targetSpriteRenderer = unitTransform.GetComponent<SpriteRenderer>();
        if (targetSpriteRenderer == null)
        {
            targetSpriteRenderer = unitTransform.GetComponentInChildren<SpriteRenderer>();
        }

        mySpriteRenderer = GetComponent<SpriteRenderer>();

        SyncSortingOrder();
        AdjustSizeAndPosition();
    }

    void Update()
    {
        if (targetUnit == null)
        {
            Destroy(gameObject);
            return;
        }

        SyncSortingOrder();
        AdjustSizeAndPosition();
    }

    private void SyncSortingOrder()
    {
        if (targetSpriteRenderer != null && mySpriteRenderer != null)
        {
            mySpriteRenderer.sortingLayerID = targetSpriteRenderer.sortingLayerID;
            mySpriteRenderer.sortingOrder = targetSpriteRenderer.sortingOrder + 1;
        }
    }

    private void AdjustSizeAndPosition()
    {
        if (targetSpriteRenderer == null || mySpriteRenderer == null || mySpriteRenderer.sprite == null)
        {
            if (targetUnit != null)
                transform.position = targetUnit.position + new Vector3(0f, 0f, -0.1f);
            return;
        }

        Bounds combinedBounds = GetTargetBounds(targetUnit);

        Vector2 mySpriteSize = mySpriteRenderer.sprite.bounds.size;
        if (mySpriteSize.x <= 0f || mySpriteSize.y <= 0f) return;

        float targetWidth = combinedBounds.size.x * widthMultiplier;
        float targetHeight = combinedBounds.size.y * heightMultiplier;

        float scaleX = targetWidth / mySpriteSize.x;
        float scaleY = targetHeight / mySpriteSize.y;

        transform.localScale = new Vector3(scaleX, scaleY, 1f);

        float normalizedPivotY = mySpriteRenderer.sprite.pivot.y / mySpriteRenderer.sprite.rect.height;

        float bottomY = combinedBounds.min.y + yOffset;
        float centerX = combinedBounds.center.x; 

        float effectY = bottomY + (targetHeight * normalizedPivotY);

        transform.position = new Vector3(centerX, effectY, combinedBounds.center.z - 0.1f);
    }

    private Bounds GetTargetBounds(Transform target)
    {
        SpriteRenderer[] renderers = target.GetComponentsInChildren<SpriteRenderer>();
        if (renderers.Length == 0)
        {
            return new Bounds(target.position, Vector3.one);
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            if (renderers[i] == mySpriteRenderer) continue;
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }
}