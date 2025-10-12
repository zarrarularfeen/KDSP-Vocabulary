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
public struct ContentPicturePair
{
    public string content;
    public Sprite image;
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

    }

    void Update()
    {

    }

    public List<ContentPicturePair> GetCurrentEnabledDictionary()
    {
        foreach (BookInformation book in booksList)
        {
            if (book.enabled)
            {
                content.AddRange(book.contentList);
            }
        }

        return content;
    }

    public List<ContentPicturePair> GetRequestedBook(Books requestedBook)
    {
        List<ContentPicturePair> displayBook = new List<ContentPicturePair>();

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