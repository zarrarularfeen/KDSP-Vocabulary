// using TMPro;
// using Unity.Properties;
// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine.Rendering;
// using System;

// [System.Serializable]
// public struct SentencesBookInformation
// {
//     public Books book;
//     public bool enabled;
//     public List<ContentPictureAudioTrio> sightWordList;
//     public string sentenceContext;
//     public List<ContentPictureAudioTrio> contentList;

// }

// public class SentencesManager : MonoBehaviour
// {
//     public static SentencesManager Instance { get; private set; }
//     [SerializeField] private List<SentencesBookInformation> booksList = new List<SentencesBookInformation>();
//     private List<ContentPictureAudioTrio> sightWords = new List<ContentPictureAudioTrio>();
//     private List<ContextListEntry> contextList = new() { new ContextListEntry { context = "", list = new List<ContentPictureAudioTrio>() } };

//     void Awake()
//     {
//         // Ensure only one instance exists
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject); // Destroy duplicate instances
//         }
//         Instance = this;
//     }

//     void Start()
//     {

//     }

//     void Update()
//     {

//     }

//     public List<ContextListEntry> GetCurrentEnabledDictionaryPhrases()
//     {
//         contextList.Clear();

//         foreach (PhrasesBookInformation book in booksList)
//         {
//             if (book.enabled)
//             {
//                 foreach (PBI b in book.PBIList)
//                 {
//                     ContextListEntry entry = new ContextListEntry
//                     {
//                         context = b.phraseContext,
//                         list = new List<ContentPictureAudioTrio>(b.contentList)
//                     };
//                     contextList.Add(entry);
//                 }
//             }
//         }

//         return contextList;
//     }

//     public List<ContentPictureAudioTrio> GetCurrentEnabledDictionarySightWords()
//     {
//         sightWords.Clear();

//         foreach (PhrasesBookInformation book in booksList)
//         {
//             if (book.enabled)
//             {
//                 sightWords.AddRange(book.sightWordList);
//             }
//         }

//         return sightWords;
//     }

//     public List<ContentPictureAudioTrio> GetRequestedBookPhrases(Books requestedBook, string requestedContext)
//     {
//         List<ContentPictureAudioTrio> displayBook = new List<ContentPictureAudioTrio>();
//         int check = 0;

//         foreach (PhrasesBookInformation book in booksList)
//         {
//             if (book.book == requestedBook)
//             {
//                 foreach (PBI b in book.PBIList)
//                 {
//                     if (b.phraseContext == requestedContext)
//                     {
//                         displayBook.AddRange(b.contentList);
//                         check = 1;
//                     }
//                 }
//                 if (check == 1)
//                 {
//                     return displayBook;
//                 }
//             }
//         }

//         return null;
//     }

//     public List<ContentPictureAudioTrio> GetRequestedBookSightWords(Books requestedBook)
//     {
//         List<ContentPictureAudioTrio> displayBook = new List<ContentPictureAudioTrio>();

//         foreach (PhrasesBookInformation book in booksList)
//         {
//             if (book.book == requestedBook)
//             {
//                 return book.sightWordList;
//             }
//         }

//         return null;
//     }

//     public void SetBookEnabled(Books requestedBook, bool enabled)
//     {
//         for (int i = 0; i < booksList.Count; i++)
//         {
//             if (booksList[i].book == requestedBook)
//             {
//                 PhrasesBookInformation temp = booksList[i];
//                 temp.enabled = enabled;
//                 booksList[i] = temp;
//                 break;
//             }
//         }
//     }
// }