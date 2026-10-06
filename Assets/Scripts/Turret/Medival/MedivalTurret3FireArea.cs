using UnityEngine;

public class MedivalTurret3FireArea : MonoBehaviour
{
    public float duration = 3.0f;

    [Header("사운드 설정")]
    [SerializeField] private AudioClip burningLoopSFX;

    private AudioSource audioSource;

    public void Init()
    {
        if (burningLoopSFX != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = burningLoopSFX;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; 
            audioSource.Play();
        }

        Destroy(gameObject, duration); 
    }
}