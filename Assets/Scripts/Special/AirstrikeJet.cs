using System.Collections;
using UnityEngine;

public class AirstrikeJet : MonoBehaviour
{
    [Header("Flight Settings")]
    public float flySpeed = 8f;
    public float dropInterval = 0.6f;
    public GameObject bombPrefab;

    [Header("Drop Area Settings")]
    public float minDropX = -16f;
    public float maxDropX = 16f;

    [Header("화면 흔들림 강도")]
    public float shakeMagnitude = 0.15f; 

    [Header("Sound Settings")]
    [SerializeField] private AudioClip flySFX;

    [HideInInspector] public LayerMask targetLayer;
    [HideInInspector] public bool isLeftUser;

    private float targetX;
    private bool isMoving = false;

    public void Initialize(bool isLeft, LayerMask layer, float startX, float endX, float flyHeight)
    {
        isLeftUser = isLeft;
        targetLayer = layer;
        targetX = endX;

        transform.position = new Vector3(startX, flyHeight, 0f);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = !isLeftUser;
        }

        if (flySFX != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(flySFX);
        }

        isMoving = true;

        float flyDistance = Mathf.Abs(endX - startX);
        float flyDuration = flyDistance / flySpeed;

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeForDuration(flyDuration, shakeMagnitude);
        }

        StartCoroutine(DropBombsRoutine());
    }

    private void Update()
    {
        if (!isMoving) return;

        float moveDirection = isLeftUser ? 1f : -1f;
        transform.Translate(Vector3.right * moveDirection * flySpeed * Time.deltaTime, Space.World);

        if ((isLeftUser && transform.position.x >= targetX) || (!isLeftUser && transform.position.x <= targetX))
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DropBombsRoutine()
    {
        while (isMoving)
        {
            float currentX = transform.position.x;

            bool canDrop = isLeftUser
                ? (currentX >= minDropX && currentX <= maxDropX)
                : (currentX <= maxDropX && currentX >= minDropX);

            if (canDrop)
            {
                if (bombPrefab != null)
                {
                    GameObject bombObj = Instantiate(bombPrefab, transform.position, Quaternion.identity, null);

                    Bomb bomb = bombObj.GetComponent<Bomb>();
                    if (bomb != null)
                    {
                        bomb.targetLayer = targetLayer;
                        bomb.isLeftUser = isLeftUser;
                    }
                }
            }

            yield return new WaitForSeconds(dropInterval);
        }
    }
}