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
public struct SBEntry
{
    public string sightWord;
    public AudioClip sightWordAudio;
    public ContentPictureAudioTrio CPAT;
}

public class SentencesManager : MonoBehaviour
{
    public static SentencesManager Instance { get; private set; }
    [SerializeField] private List<SentencesBookInformation> booksList = new List<SentencesBookInformation>();
    private List<ContentPictureAudioTrio> sightWords = new List<ContentPictureAudioTrio>();
    private List<ContentPictureAudioTrio> sbList = new List<ContentPictureAudioTrio>();
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

    public List<ContentPictureAudioTrio> GetCurrentEnabledDictionarySentences()
    {
        sbList.Clear();

        foreach (SentencesBookInformation book in booksList)
        {
            if (book.enabled)
            {
                foreach (SBEntry b in book.contentList)
                {
                    sbList.Add(b.CPAT);
                }
            }
        }

        return sbList;
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
}