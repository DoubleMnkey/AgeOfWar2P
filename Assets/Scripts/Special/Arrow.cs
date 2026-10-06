using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("화살 능력치")]
    public float speed = 12f;
    public float damage = 25f;

    [Header("타겟 설정")]
    public LayerMask targetLayer;

    [Header("화면 흔들림 강도")]
    public float shakeMagnitude = 0.08f;

    private Vector3 direction = Vector3.down;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, -90f);

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeForDuration(1.5f, shakeMagnitude);
        }
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (transform.position.y < -8f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Arrow>() != null) return;

        if (((1 << collision.gameObject.layer) & targetLayer.value) == 0) return;

        IDamageable target = collision.GetComponentInParent<IDamageable>();
        if (target == null) target = collision.GetComponent<IDamageable>();

        if (target != null && !target.IsDead)
        {
            target.TakeDamage(damage);
            Destroy(gameObject); 
        }
    }
}