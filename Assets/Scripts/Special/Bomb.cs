using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bomb : MonoBehaviour
{
    [Header("Settings")]
    public float damage = 150f;
    public float explosionRadius = 1.5f;
    public GameObject explosionEffectPrefab;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip explosionSFX; 

    [Header("Ground Setting")]
    public float groundY = -3.2f;

    [Header("Curved Drop Settings")]
    public float initialHorizontalSpeed = 10f;
    public float horizontalDamping = 10f;

    [HideInInspector] public LayerMask targetLayer;
    [HideInInspector] public bool isLeftUser;

    private Rigidbody2D rb;
    private bool hasExploded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        float direction = isLeftUser ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * initialHorizontalSpeed, 0f);

        float initialZAngle = isLeftUser ? -30f : 30f;
        transform.rotation = Quaternion.Euler(0f, 0f, initialZAngle);
    }

    private void Update()
    {
        if (hasExploded) return;

        if (transform.position.y <= groundY)
        {
            Explode();
        }
    }

    private void FixedUpdate()
    {
        if (hasExploded) return;

        float newVX = Mathf.MoveTowards(rb.linearVelocity.x, 0f, horizontalDamping * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newVX, rb.linearVelocity.y);

        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasExploded && ((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (explosionSFX != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(explosionSFX);
        }

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, targetLayer);
        foreach (Collider2D hit in hitColliders)
        {
            UnitControl unit = hit.GetComponent<UnitControl>();
            if (unit != null && !unit.IsDead)
            {
                unit.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}