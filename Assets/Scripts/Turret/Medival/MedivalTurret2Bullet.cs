using UnityEngine;

public class MedivalTurret2Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float splashRadius = 2.5f;

    [Header("ÅºÈ¯/ÆÄÆí ÀÌÆåÆ® ¼³Á¤")]
    public GameObject bulletPiecePrefab;
    public int minPieceCount = 4;
    public int maxPieceCount = 7;
    public float scatterRadius = 1.8f;

    private Transform target;
    private Vector3 lastTargetPos;
    private float damage;
    private LayerMask enemyLayer;

    public void Init(Transform _target, float _damage, LayerMask _enemyLayer)
    {
        target = _target;
        damage = _damage;
        enemyLayer = _enemyLayer;

        if (target != null)
        {
            lastTargetPos = GetTargetCenter(target);
        }
    }

    private void Update()
    {
        if (target != null)
        {
            lastTargetPos = GetTargetCenter(target);
        }

        Vector3 dir = (lastTargetPos - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (Vector3.Distance(transform.position, lastTargetPos) < 0.3f)
        {
            Explode();
        }
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, splashRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            UnitControl unit = hit.GetComponent<UnitControl>();
            if (unit != null && !unit.IsDead)
            {
                unit.TakeDamage(damage);
            }
        }

        SpawnDebrisPieces(transform.position);

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