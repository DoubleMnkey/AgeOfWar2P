using UnityEngine;

public class RocketExplosionEffect : MonoBehaviour
{
    [Header("자동 파괴 시간 (0이면 첫 번째 애니메이션 길이에 자동 맞춤)")]
    public float autoDestroyTime = 0f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Start()
    {
        float destroyDelay = autoDestroyTime;

        if (destroyDelay <= 0f)
        {
            Animator animator = GetComponent<Animator>();
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
                if (clips.Length > 0)
                {
                    destroyDelay = clips[0].length;
                }
            }
        }

        if (destroyDelay <= 0f) destroyDelay = 0.5f;

        Destroy(gameObject, destroyDelay);
    }

    public void SetSortingOrder(string layerName, int order)
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = layerName;
            spriteRenderer.sortingOrder = order;
        }
    }
}