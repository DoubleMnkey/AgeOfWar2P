using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [Header("Settings")]
    public float destroyDelay = 1.0f; 

    [Header("Sound Settings")]
    [SerializeField] private AudioClip fireSFX; 

    private void Start()
    {
        if (fireSFX != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(fireSFX);
        }

        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            float totalDuration = ps.main.duration + ps.main.startLifetime.constantMax;
            Destroy(gameObject, totalDuration);
            return;
        }

        Destroy(gameObject, destroyDelay);
    }
}