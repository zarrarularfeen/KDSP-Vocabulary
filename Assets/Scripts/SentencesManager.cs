using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System;

[System.Serializable]
public struct SentencesBookInformation
{
    public Books book;
    public bool enabled;
    public string sentenceContext;
    public List<SBEntry> contentList;
}

[System.Serializable]
public struct BlankPositionGroup
{
    public List<int> positions;
}

[System.Serializable]
public struct SBEntry
{
    public string sightWord;
    public AudioClip sightWordAudio;
    public ContentPictureAudioTrio CPAT;
    public List<BlankPositionGroup> blankPositions;
}

public class SentencesManager : MonoBehaviour
{
    public static SentencesManager Instance { get; private set; }
    [SerializeField] private List<SentencesBookInformation> booksList = new List<SentencesBookInformation>();
    private List<ContentPictureAudioTrio> sightWords = new List<ContentPictureAudioTrio>();
    private List<SBEntry> sbList = new List<SBEntry>();
    private List<ContentPictureAudioTrio> displayBook = new List<ContentPictureAudioTrio>();

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

    public List<SBEntry> GetCurrentEnabledDictionarySentences()
    {
        sbList.Clear();

        foreach (SentencesBookInformation book in booksList)
        {
            if (book.enabled)
            {
                sbList.AddRange(book.contentList);
                // foreach (SBEntry b in book.contentList)
                // {
                //     sbList.Add(b);
                // }
            }
        }

        return sbList;
    }

    public List<ContentPictureAudioTrio> GetCurrentEnabledDictionarySentencesForReadingBook()
    {
        sightWords.Clear();

        foreach (SentencesBookInformation book in booksList)
        {
            if (book.enabled)
            {
                foreach (SBEntry b in book.contentList)
                {
                    ContentPictureAudioTrio entry = new ContentPictureAudioTrio
                    {
                        content = b.CPAT.content + "*" + book.sentenceContext,
                        image = b.CPAT.image,
                        audio = b.CPAT.audio
                    };
                    sightWords.Add(entry);
                }
            }
        }

        return sightWords;
    }

    public List<ContentPictureAudioTrio> GetCurrentEnabledDictionarySightWords()
    {
        sightWords.Clear();

        foreach (SentencesBookInformation book in booksList)
        {
            if (book.enabled)
            {
                foreach (SBEntry b in book.contentList)
                {
                    ContentPictureAudioTrio entry = new ContentPictureAudioTrio
                    {
                        content = b.sightWord,
                        image = b.CPAT.image,
                        audio = b.sightWordAudio
                    };
                    sightWords.Add(entry);
                }
            }
        }

        return sightWords;
    }

    public List<ContentPictureAudioTrio> GetRequestedBookSentences(Books requestedBook)
    {
        displayBook.Clear();

        foreach (SentencesBookInformation book in booksList)
        {
            if (book.book == requestedBook)
            {
                foreach (SBEntry b in book.contentList)
                {
                    displayBook.Add(b.CPAT);
                }
                return displayBook;
            }
        }

        return null;
    }

    public List<ContentPictureAudioTrio> GetRequestedBookSightWords(Books requestedBook)
    {
        displayBook.Clear();

        foreach (SentencesBookInformation book in booksList)
        {
            if (book.book == requestedBook)
            {
                foreach (SBEntry b in book.contentList)
                {
                    ContentPictureAudioTrio entry = new ContentPictureAudioTrio
                    {
                        content = b.sightWord,
                        image = b.CPAT.image,
                        audio = b.sightWordAudio
                    };
                    displayBook.Add(entry);
                }
                return displayBook;
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
                SentencesBookInformation temp = booksList[i];
                temp.enabled = enabled;
                booksList[i] = temp;
                break;
            }
        }
    }

    public List<List<string>> GenerateFillInTheBlankList(SBEntry entry)
    {
        List<List<string>> output = new List<List<string>>();
        string[] contentList = entry.CPAT.content.Split();

        foreach (BlankPositionGroup lst in entry.blankPositions)
        {
            List<string> fitb = new List<string>();
            for (int i = 0; i < contentList.Length; i++)
            {
                if (lst.positions[i] == i)
                {
                    fitb.Add("_");
                }
                else
                {
                    fitb.Add(contentList[i]);
                }
            }
            output.Add(fitb);
        }

        return output;
    }
}