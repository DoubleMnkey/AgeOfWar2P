using UnityEngine;

public class MiltaryTurret2Bullet : MonoBehaviour
{
    [Header("로켓 이동 및 가속 설정")]
    public float initialSpeed = 2f;
    public float acceleration = 40f;
    public float maxSpeed = 50f;

    [Header("폭발 및 이펙트 설정")]
    public float splashRadius = 2.5f;
    public GameObject explosionPrefab; 

    [Header("탄환/파편 이펙트 설정")]
    public GameObject bulletPiecePrefab; 
    public int minPieceCount = 4;
    public int maxPieceCount = 8;
    public float scatterRadius = 2.0f;

    [Header("사운드 설정")]
    [SerializeField] private AudioClip hitSFX;

    private Transform target;
    private Vector3 lastTargetPos;
    private float damage;
    private LayerMask enemyLayer;

    private float currentSpeed;

    public void SetTarget(Transform _target, float _damage, LayerMask _enemyLayer)
    {
        target = _target;
        damage = _damage;
        enemyLayer = _enemyLayer;

        currentSpeed = initialSpeed;

        if (target != null)
        {
            lastTargetPos = GetTargetCenter(target);
        }
    }

    private void Update()
    {
        currentSpeed += acceleration * Time.deltaTime;
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

        if (target != null)
        {
            lastTargetPos = GetTargetCenter(target);
        }

        Vector3 destination = lastTargetPos;
        Vector3 currentPos = transform.position;
        Vector3 dir = (destination - currentPos).normalized;

        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        float moveDistance = currentSpeed * Time.deltaTime;
        float distanceToTarget = Vector3.Distance(currentPos, destination);

        if (distanceToTarget <= 0.4f || moveDistance >= distanceToTarget)
        {
            transform.position = destination;
            Explode();
            return;
        }

        transform.Translate(dir * moveDistance, Space.World);
    }

    private Vector3 GetTargetCenter(Transform targetTransform)
    {
        if (targetTransform == null) return Vector3.zero;

        Collider2D col = targetTransform.GetComponent<Collider2D>();
        if (col != null)
        {
            return col.bounds.center;
        }

        return targetTransform.position + Vector3.up * 1.0f;
    }

    private void Explode()
    {
        if (hitSFX != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(hitSFX);
        }

        if (explosionPrefab != null)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.z = 0f;

            GameObject expObj = Instantiate(explosionPrefab, spawnPos, Quaternion.identity);
            RocketExplosionEffect expEffect = expObj.GetComponent<RocketExplosionEffect>();

            if (expEffect != null && target != null)
            {
                SpriteRenderer targetSR = target.GetComponent<SpriteRenderer>();
                if (targetSR == null)
                    targetSR = target.GetComponentInChildren<SpriteRenderer>();

                if (targetSR != null)
                {
                    expEffect.SetSortingOrder(targetSR.sortingLayerName, targetSR.sortingOrder + 1);
                }
            }
        }

        SpawnDebrisPieces(transform.position);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, splashRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            UnitControl unit = hit.GetComponent<UnitControl>();
            if (unit != null && !unit.IsDead)
            {
                unit.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    private void SpawnDebrisPieces(Vector3 centerPos)
    {
        if (bulletPiecePrefab == null) return;

        centerPos.z = 0f;
        int count = Random.Range(minPieceCount, maxPieceCount + 1);

        for (int i = 0; i < count; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * scatterRadius;
            Vector3 dropTarget = centerPos + new Vector3(randomCircle.x, Mathf.Abs(randomCircle.y) * -0.5f - 0.2f, 0f);

            GameObject pieceObj = Instantiate(bulletPiecePrefab, centerPos, Quaternion.identity);
            BulletCasingDebris piece = pieceObj.GetComponent<BulletCasingDebris>();

            if (piece != null)
            {
                piece.SetSortingOrder("Default", 999);
                piece.Launch(dropTarget);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}