using UnityEngine;

public class CaveTurret1Bullet : MonoBehaviour
{
    public float speed = 12f;

    [Header("돌 파편 이펙트 설정")]
    public GameObject stonePiecePrefab;
    public int minPieceCount = 3;
    public int maxPieceCount = 6;
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

        SpawnDebrisPieces();

        Destroy(gameObject);
    }

    private void SpawnDebrisPieces()
    {
        if (stonePiecePrefab == null || target == null) return;

        Vector3 spawnPos = GetTargetCenter(target);
        spawnPos.z = 0f;

        int count = Random.Range(minPieceCount, maxPieceCount + 1);

        for (int i = 0; i < count; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * scatterRadius;
            Vector3 dropTarget = spawnPos + new Vector3(randomCircle.x, Mathf.Abs(randomCircle.y) * -0.5f - 0.2f, 0f);

            GameObject pieceObj = Instantiate(stonePiecePrefab, spawnPos, Quaternion.identity);
            StoneDebrisPiece piece = pieceObj.GetComponent<StoneDebrisPiece>();

            if (piece != null)
            {
                piece.SetSortingOrder("Default", 999);
                piece.Launch(dropTarget);
            }
        }
    }
}