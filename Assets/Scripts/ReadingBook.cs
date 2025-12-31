using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

[System.Serializable]
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
public struct ContentPictureAudioTrio
{
    public string content;
    public Sprite image;
    public AudioClip audio;
}

[System.Serializable]
public struct BookInformation
{
    public Books book;
    public bool enabled;
    public List<ContentPictureAudioTrio> contentList;
}

public class ReadingBook : MonoBehaviour
{
    public static ReadingBook Instance { get; private set; }
    [SerializeField] private List<BookInformation> booksList = new List<BookInformation>();

    private List<ContentPictureAudioTrio> content = new List<ContentPictureAudioTrio>();

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

    }

    void Update()
    {

    }

    public List<ContentPictureAudioTrio> GetCurrentEnabledDictionary()
    {
        content.Clear();

        foreach (BookInformation book in booksList)
        {
            if (book.enabled)
            {
                content.AddRange(book.contentList);
            }
        }

        return content;
    }

    public List<ContentPictureAudioTrio> GetRequestedBook(Books requestedBook)
    {
        List<ContentPictureAudioTrio> displayBook = new List<ContentPictureAudioTrio>();

        foreach (BookInformation book in booksList)
        {
            if (book.book == requestedBook)
            {
                return book.contentList;
            }
        }

        return null;
    }

    public void SetBookEnabled(Books requestedBook, bool enabled)
    {
        for (int i = 0; i < booksList.Count; i++)
        {
            if (booksList[i].book == requestedBook)
            {
                BookInformation temp = booksList[i];
                temp.enabled = enabled;
                booksList[i] = temp;
                break;
            }
        }
    }
}