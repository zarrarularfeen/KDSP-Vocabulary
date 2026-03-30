using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] public AudioSource audioSource;
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip winningSound;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip wrongSound;
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip withSound;
    [SerializeField] private AudioClip showMeSound;
    [SerializeField] private AudioClip[] positiveReinforcementSound;
    private List<AudioClip> seqPlayAudioClips = new List<AudioClip>();
    private float seqDelay = 0.1f;

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
        if (!backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.loop = true; // Loop the background music
            backgroundMusicSource.clip = backgroundMusic;
            backgroundMusicSource.Play();
        }
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

    public void ShowMeFunction(AudioClip audio)
    {
        seqPlayAudioClips.Clear();
        if (audio != null)
        {
            seqPlayAudioClips.Add(showMeSound);
            seqPlayAudioClips.Add(audio);
            StartCoroutine(PlayAudioSequentially());
        }
    }

    public void ShowMeFunction(string content)
    {
        if (content != null)
        {
            string modifiedContent = content.Replace("/", " or ");
            AudioClip audio = Resources.Load<AudioClip>($"Updated-Audios/show_me/{modifiedContent}");
            audioSource.clip = audio;
            audioSource.Play();
        }
    }

    public void MatchWithFunction(AudioClip audio)
    {
        seqPlayAudioClips.Clear();
        if (audio != null)
        {
            seqPlayAudioClips.Add(matchSound);
            seqPlayAudioClips.Add(audio);
            seqPlayAudioClips.Add(withSound);
            seqPlayAudioClips.Add(audio);
            StartCoroutine(PlayAudioSequentially());
        }
    }

    public void MatchWithFunction(string content)
    {
        Debug.Log("content = " + content);
        if (content != null)
        {
            string modifiedContent = content.Replace("/", " or ");
            AudioClip audio = Resources.Load<AudioClip>($"Updated-Audios/match_words/{modifiedContent}");
            audioSource.clip = audio;
            audioSource.Play();
        }
    }

    public void WordAudioFunction(string content)
    {
        if (content != null)
        {
            string modifiedContent = content.Replace("/", " or ");
            Debug.Log(modifiedContent);
            AudioClip audio = Resources.Load<AudioClip>($"Updated-Audios/words/{modifiedContent}");
            if (audio == null)
            {
                Debug.Log("Audio Not Found!");
            }
            audioSource.clip = audio;
            audioSource.Play();
        }
    }

    public void PhrasesAudioFunction(string content)
    {
        if (content != null)
        {
            string modifiedContent = content.Replace("/", " or ");
            modifiedContent = content.Trim();
            Debug.Log(modifiedContent);
            AudioClip audio = Resources.Load<AudioClip>($"Updated-Audios/phrases/{modifiedContent}");
            if (audio == null)
            {
                Debug.Log("Audio Not Found!");
            }
            audioSource.clip = audio;
            audioSource.Play();
        }
    }

    public void SentencesAudioFunction(string context, string content)
    {
        if (content != null)
        {
            string modifiedContent = char.ToUpper(content[0]) + content.Substring(1);
            if (modifiedContent[modifiedContent.Length - 1] != '.')
            {
                modifiedContent = modifiedContent + '.';
            }
            if (context[context.Length - 1] == '?')
            {
                context = context.Remove(context.Length - 1);
            }
            Debug.Log(context);
            Debug.Log(modifiedContent);
            AudioClip audio = Resources.Load<AudioClip>($"Updated-Audios/sentences/{context}/{modifiedContent}");
            if (audio == null)
            {
                Debug.Log("Audio Not Found!");
            }
            audioSource.clip = audio;
            audioSource.Play();
        }
    }

    public void StopAllSFX()
    {
        audioSource.Stop();
    }

    IEnumerator PlayAudioSequentially()
    {
        for (int i = 0; i < seqPlayAudioClips.Count; i++)
        {
            audioSource.clip = seqPlayAudioClips[i];
            audioSource.Play();

            while (audioSource.isPlaying)
            {
                yield return new WaitForSeconds(seqDelay);
                // yield return null;
            }
        }
    }

    public void WaitForCurrentAudio()
    {
        StartCoroutine(WaitForCurrentAudioCoroutine());
    }


    IEnumerator WaitForCurrentAudioCoroutine()
    {
        if (audioSource.isPlaying)
        {
            yield return new WaitForSeconds(audioSource.clip.length);
        }
    }
}