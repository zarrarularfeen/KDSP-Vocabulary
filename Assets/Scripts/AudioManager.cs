using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip winningSound;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip wrongSound;
    [SerializeField] private AudioClip[] positiveReinforcementSound;

    void Awake()
    {
        // Ensure only one instance of AudioManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if (!backgroundMusicSource.isPlaying)
        // {
        //     backgroundMusicSource.loop = true; // Loop the background music
        //     backgroundMusicSource.clip = backgroundMusic;
        //     backgroundMusicSource.Play();
        // }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayWinningSoundDelayed()
    {
        if (winningSound != null)
        {
            audioSource.clip = winningSound;
            audioSource.PlayDelayed(2f); // Play winning sound with a slight delay
        }
    }

    public void PlayWinningSoundNonDelayed()
    {
        if (winningSound != null)
        {
            audioSource.PlayOneShot(winningSound); // Play winning sound
        }
    }

    public void PlayCorrectSound()
    {
        if (correctSound != null)
        {
            audioSource.PlayOneShot(correctSound);
        }
    }

    public void PlayWrongSound()
    {
        if (wrongSound != null)
        {
            audioSource.PlayOneShot(wrongSound);
        }
    }

    public void PlayPositiveReinforcementSound()
    {
        int randomIndex = UnityEngine.Random.Range(0, positiveReinforcementSound.Length);
        if (positiveReinforcementSound[randomIndex] != null)
        {
            audioSource.PlayOneShot(positiveReinforcementSound[randomIndex]);
        }
    }

    public void PlayGivenAudioNonDelayed(AudioClip audio)
    {
        if (audio != null)
        {
            audioSource.PlayOneShot(audio);
        }
    }

    public void PlayGivenAudioDelayed(AudioClip audio, float delay)
    {
        if (audio != null)
        {
            audioSource.clip = audio;
            audioSource.PlayDelayed(delay);
        }
    }

    public void StopAllSFX()
    {
        audioSource.Stop();
    }
}