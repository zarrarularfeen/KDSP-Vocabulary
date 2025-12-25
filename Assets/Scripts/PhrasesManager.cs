using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;


[System.Serializable]
public struct PBI
{
    public string phraseContext;
    public int index;
    public List<ContentPicturePair> contentList;
}


[System.Serializable]
public struct PhrasesBookInformation
{
    public Books book;
    public bool enabled;
    public List<ContentPicturePair> sightWordList;
    public List<PBI> PBIList;

}

public class PhrasesManager : MonoBehaviour
{
    public static PhrasesManager Instance { get; private set; }
    [SerializeField] private List<PhrasesBookInformation> booksList = new List<PhrasesBookInformation>();

    private List<ContentPicturePair> content = new List<ContentPicturePair>();

    private List<ContentPicturePair> sightWords = new List<ContentPicturePair>();

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

    public List<ContentPicturePair> GetCurrentEnabledDictionaryPhrases()
    {
        content.Clear();

        foreach (PhrasesBookInformation book in booksList)
        {
            if (book.enabled)
            {
                foreach (PBI b in book.PBIList)
                {
                    content.AddRange(b.contentList);
                }
            }
        }

        return content;
    }

    public List<ContentPicturePair> GetCurrentEnabledDictionarySightWords()
    {
        sightWords.Clear();

        foreach (PhrasesBookInformation book in booksList)
        {
            if (book.enabled)
            {
                sightWords.AddRange(book.sightWordList);
            }
        }

        return sightWords;
    }

    public List<ContentPicturePair> GetRequestedBookPhrases(Books requestedBook, string requestedContext)
    {
        List<ContentPicturePair> displayBook = new List<ContentPicturePair>();
        int check = 0;

        foreach (PhrasesBookInformation book in booksList)
        {
            if (book.book == requestedBook)
            {
                foreach (PBI b in book.PBIList)
                {
                    if (b.phraseContext == requestedContext)
                    {
                        displayBook.AddRange(b.contentList);
                        check = 1;
                    }
                }
                if (check == 1)
                {
                    return displayBook;
                }
            }
        }

        return null;
    }

    public List<ContentPicturePair> GetRequestedBookSightWords(Books requestedBook)
    {
        List<ContentPicturePair> displayBook = new List<ContentPicturePair>();

        foreach (PhrasesBookInformation book in booksList)
        {
            if (book.book == requestedBook)
            {
                return book.sightWordList;
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
                PhrasesBookInformation temp = booksList[i];
                temp.enabled = enabled;
                booksList[i] = temp;
                break;
            }
        }
    }
}