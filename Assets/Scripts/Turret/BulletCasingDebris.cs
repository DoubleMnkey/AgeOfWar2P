using UnityEngine;

public class BulletCasingDebris : MonoBehaviour
{
    [Header("탄환 파편 이동 옵션")]
    public float baseSpeed = 3f;       
    public float baseArcHeight = 1.5f; 
    public float lifeTime = 10f;    

    private Vector3 startPos;
    private Vector3 targetPos;
    private float progress = 0f;
    private bool isLaunched = false;

    private float speed;
    private float arcHeight;
    private float rotationSpeed;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        Destroy(gameObject, lifeTime);
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

    public void Launch(Vector3 target)
    {
        startPos = transform.position;
        targetPos = target;

        startPos.z = 0f;
        targetPos.z = 0f;

        speed = baseSpeed * Random.Range(0.8f, 1.3f);
        arcHeight = baseArcHeight * Random.Range(0.8f, 1.4f);
        rotationSpeed = Random.Range(-900f, 900f);

        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        progress = 0f;
        isLaunched = true;
    }

    private void Update()
    {
        if (!isLaunched) return;

        progress += Time.deltaTime * speed;
        float p = Mathf.Min(progress, 1.0f);

        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, p);
        currentPos.y += Mathf.Sin(p * Mathf.PI) * arcHeight;
        currentPos.z = 0f;

        transform.position = currentPos;
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (p >= 1.0f)
        {
            isLaunched = false;
            Destroy(gameObject);
        }
    }
}