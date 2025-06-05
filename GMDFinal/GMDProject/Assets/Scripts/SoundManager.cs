using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource ambientSource;

    [Header("Sound Clips")]
    public AudioClip hitEnemyClip;
    public AudioClip hitPlayerClip;
    public AudioClip levelUpClip;
    public AudioClip ambientMusic;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayAmbient();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayAmbient()
    {
        if (ambientSource != null && ambientMusic != null)
        {
            ambientSource.clip = ambientMusic;
            ambientSource.loop = true;
            ambientSource.Play();
        }
    }
}
