using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [Header("BGM")]
    [SerializeField] private AudioClip buildBGM;
    [SerializeField] private AudioClip attackBGM;
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip gameclearBGM;
    [SerializeField] private AudioClip gameoverBGM;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    public void PlayBuildBGM()
    {
        PlayBGM(buildBGM);
    }

    public void PlayAttackBGM()
    {
        PlayBGM(attackBGM);
    }

    public void PlayTitleBGM()
    {
        PlayBGM(titleBGM);
    }

    public void PlayGameoverBGM()
    {
        PlayBGM(gameoverBGM);
    }

    public void PlayGameclearBGM()
    {
        PlayBGM(gameclearBGM);
    }

    private void PlayBGM(AudioClip clip)
    {
        if (clip == null)
            return;

        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        audioSource.clip = clip;
        audioSource.Play();
    }
}