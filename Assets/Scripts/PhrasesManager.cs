using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

[System.Serializable]
public struct ContentPictureTrio
{
    public string content;
    public Sprite phraseImage;
    public Sprite sightWordImage;
}

[System.Serializable]
public struct PBI
{
    public string phraseContext;
    public int index;
    public List<ContentPictureTrio> contentList;
}


[System.Serializable]
public struct PhrasesBookInformation
{
    public Books book;
    public bool enabled;
    public List<PBI> PBIList;

}

public class PhrasesManager : MonoBehaviour
{
    public static PhrasesManager Instance { get; private set; }
    [SerializeField] private List<PhrasesBookInformation> booksList = new List<PhrasesBookInformation>();

    private List<ContentPictureTrio> content = new List<ContentPictureTrio>();

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

    public List<ContentPictureTrio> GetCurrentEnabledDictionary()
    {
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

    public List<ContentPictureTrio> GetRequestedBook(Books requestedBook, string requestedContext)
    {
        List<ContentPictureTrio> displayBook = new List<ContentPictureTrio>();
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

    public List<string> ExtractSightWords(List<PBI> list)
    {
        List<string> sightWords = new List<string>();

        foreach (PBI b in list)
        {
            foreach (ContentPictureTrio c in b.contentList)
            {
                sightWords.Add(c.content.Split()[b.index]);
            }
        }

        return sightWords;
    }
}