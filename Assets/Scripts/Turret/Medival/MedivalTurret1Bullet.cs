using UnityEngine;

public class MedivalTurret1Bullet : MonoBehaviour
{
    public float speed = 15f;

    [Header("ÅºÈ¯ ÆÄÆí ÀÌÆåÆ® ¼³Á¤")]
    public GameObject bulletPiecePrefab;
    public int minPieceCount = 2;
    public int maxPieceCount = 4;
    public float scatterRadius = 1.2f;

    private Transform target;
    private float damage;

    public void SetTarget(Transform _target, float _damage)
    {
        target = _target;
        damage = _damage;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 destination = GetTargetCenter(target);

        Vector3 dir = (destination - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (Vector3.Distance(transform.position, destination) < 0.3f)
        {
            HitTarget();
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

    private void HitTarget()
    {
        UnitControl unit = target.GetComponent<UnitControl>();
        if (unit != null && !unit.IsDead)
        {
            unit.TakeDamage(damage);
        }

        SpawnDebrisPieces(GetTargetCenter(target));

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
}