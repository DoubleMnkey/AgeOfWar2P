using UnityEngine;

public class Meteor : MonoBehaviour
{
    [Header("운석 능력치")]
    public float speed = 15f;
    public float damage = 50f;

    [Header("타겟 설정")]
    public LayerMask targetLayer;

    [Header("화면 흔들림 강도")]
    public float shakeMagnitude = 0.25f; 

    private Vector3 direction = Vector3.down;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, -90f);

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeForDuration(1.2f, shakeMagnitude);
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
        if (collision.GetComponent<Meteor>() != null) return;

        if (((1 << collision.gameObject.layer) & targetLayer.value) == 0) return;

        IDamageable target = collision.GetComponentInParent<IDamageable>();
        if (target == null) target = collision.GetComponent<IDamageable>();

        if (target != null)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}