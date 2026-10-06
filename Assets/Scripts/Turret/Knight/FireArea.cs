using UnityEngine;

public class FireArea : MonoBehaviour
{
    public float duration = 3f; 

    [Header("사운드 설정")]
    [SerializeField] private AudioClip burningLoopSFX;

    private float timer = 0f;
    private AudioSource audioSource;

    private void Start()
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
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}