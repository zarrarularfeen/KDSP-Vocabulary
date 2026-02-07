using UnityEngine;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private GameObject audioManagerPrefab;
    [SerializeField] private GameObject readingBookPrefab;
    [SerializeField] private GameObject phrasesManagerPrefab;
    [SerializeField] private GameObject sentencesManagerPrefab;

    void Awake()
    {
        if (AudioManager.Instance == null)
        {
            GameObject audioManagerInstance = Instantiate(audioManagerPrefab);
            DontDestroyOnLoad(audioManagerInstance); // Ensure it persists across scenes
        }

        if (ReadingBook.Instance == null)
        {
            GameObject readingBookInstance = Instantiate(readingBookPrefab);
            DontDestroyOnLoad(readingBookInstance); // Ensure it persists across scenes
        }

        if (PhrasesManager.Instance == null)
        {
            GameObject phrasesManagerInstance = Instantiate(phrasesManagerPrefab);
            DontDestroyOnLoad(phrasesManagerInstance); // Ensure it persists across scenes
        }

        if (SentencesManager.Instance == null)
        {
            GameObject sentencesManagerInstance = Instantiate(sentencesManagerPrefab);
            DontDestroyOnLoad(sentencesManagerInstance); // Ensure it persists across scenes
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
