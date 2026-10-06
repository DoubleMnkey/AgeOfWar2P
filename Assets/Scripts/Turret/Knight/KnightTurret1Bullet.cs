using UnityEngine;

public class KnightTurret1Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float splashRadius = 1.8f;

    [Header("돌 파편 이펙트 설정")]
    public GameObject stonePiecePrefab;
    public int minPieceCount = 3;
    public int maxPieceCount = 6;
    public float scatterRadius = 1.5f;

    private Vector3 targetPosition;
    private float damage;
    private LayerMask enemyLayer;

    public void Init(Transform target, float _damage, LayerMask _enemyLayer)
    {
        targetPosition = target != null ? GetTargetCenter(target) : transform.position;
        damage = _damage;
        enemyLayer = _enemyLayer;
    }

    private void Update()
    {
        if (targetPosition == Vector3.zero)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (targetPosition - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
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
        if (stonePiecePrefab == null) return;

        centerPos.z = 0f;
        int count = Random.Range(minPieceCount, maxPieceCount + 1);

        for (int i = 0; i < count; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * scatterRadius;
            Vector3 dropTarget = centerPos + new Vector3(randomCircle.x, Mathf.Abs(randomCircle.y) * -0.5f - 0.2f, 0f);

            GameObject pieceObj = Instantiate(stonePiecePrefab, centerPos, Quaternion.identity);
            StoneDebrisPiece piece = pieceObj.GetComponent<StoneDebrisPiece>();

            if (piece != null)
            {
                piece.SetSortingOrder("Default", 999);
                piece.Launch(dropTarget);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}