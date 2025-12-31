using UnityEngine;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private GameObject audioManagerPrefab;
    [SerializeField] private GameObject readingBookPrefab;

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
            DontDestroyOnLoad(readingBookPrefab); // Ensure it persists across scenes
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
