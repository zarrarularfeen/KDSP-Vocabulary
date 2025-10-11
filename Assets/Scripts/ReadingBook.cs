using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public enum Books
{
    Vocabulary1,
    Vocabulary2,
    Vocabulary3,

    Phrases1,
    Phrases2,
    Phrases3,

    Sentences1,
    Sentences2,
    Sentences3
}

[System.Serializable]
public struct ContentPicturePair
{
    public string content;
    public Image image;
}

[System.Serializable]
public struct BookInformation
{
    public Books book;
    public bool enabled;
    public List<ContentPicturePair> contentList;
}

public class ReadingBook : MonoBehaviour
{
    public static ReadingBook Instance { get; private set; }
    [SerializeField] private List<BookInformation> booksList = new List<BookInformation>();
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Image displayImage;

    private List<ContentPicturePair> content = new List<ContentPicturePair>();

    void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        Instance = this;
    }

    void Start()
    {
        GetCurrentEnabledDictionary();
    }

    void GetCurrentEnabledDictionary()
    {
        foreach (BookInformation book in booksList)
        {
            if (book.enabled)
            {
                content.AddRange(book.contentList);
            }
        }
    }

    void DisplayContent()
    {
        int currentidx = 0;
        ContentPicturePair currentContent = content[currentidx];

        displayText.text = currentContent.content;
        displayImage = currentContent.image;
        displayImage.SetNativeSize();
    }
}