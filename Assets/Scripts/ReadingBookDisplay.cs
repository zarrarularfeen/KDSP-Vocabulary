using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System;

public enum ReadingBookMode
{
    Vocabulary,
    Phrases,
    Sentences
}

public enum ReadingBookForm
{
    SightWords,
    PhrasesOrSentences
}

public class ReadingBookDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Image displayImage;
    [SerializeField] private Button displayButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    [SerializeField] private GameObject VocabularyGrid;
    [SerializeField] private GameObject PhrasesGrid;
    [SerializeField] private GameObject SentencesGrid;

    [SerializeField] private GameObject VocabularyGridMSB;
    [SerializeField] private GameObject PhrasesGridMSB;
    [SerializeField] private GameObject SentencesGridMSB;

    [SerializeField] private GameObject NextSceneButton;
    [SerializeField] private GameObject SightWordsButton;
    [SerializeField] private GameObject PhrasesButton;
    [SerializeField] private GameObject SentencesButton;

    private List<ContentPictureAudioTrio> content = new List<ContentPictureAudioTrio>();
    private int currentidx = 0;
    private static Books requestedBook;
    private static ReadingBookMode currentBookMode;
    private static ReadingBookForm currentBookForm;

    void Awake()
    {

    }

    void Start()
    {
        if (SceneController.currentScene == Scenes.ReadingBookDisplayBookSelection)
        {
            if (currentBookMode == ReadingBookMode.Vocabulary)
            {
                VocabularyGrid.SetActive(true);
                VocabularyGridMSB.SetActive(true);
                NextSceneButton.SetActive(true);
            }
            else if (currentBookMode == ReadingBookMode.Phrases)
            {
                NextSceneButton.SetActive(false);
                SightWordsButton.SetActive(true);
                PhrasesButton.SetActive(true);

                SightWordsButton.GetComponent<Button>().onClick.AddListener(() => { OpenGrids(ReadingBookForm.SightWords); });
                PhrasesButton.GetComponent<Button>().onClick.AddListener(() => { OpenGrids(ReadingBookForm.PhrasesOrSentences); });
            }
            else if (currentBookMode == ReadingBookMode.Sentences)
            {
                NextSceneButton.SetActive(false);
                SightWordsButton.SetActive(true);
                SentencesButton.SetActive(true);

                SightWordsButton.GetComponent<Button>().onClick.AddListener(() => { OpenGrids(ReadingBookForm.SightWords); });
                SentencesButton.GetComponent<Button>().onClick.AddListener(() => { OpenGrids(ReadingBookForm.PhrasesOrSentences); });
            }
        }
        else
        {
            GetContentList();
            nextButton.onClick.AddListener(Next);
            previousButton.onClick.AddListener(Previous);
            nextButton.gameObject.SetActive(false);
            previousButton.gameObject.SetActive(false);
            DisplayContent();
        }
    }

    void Update()
    {

    }

    public void OpenGrids(ReadingBookForm form)
    {
        SightWordsButton.SetActive(false);
        PhrasesButton.SetActive(false);
        SentencesButton.SetActive(false);
        NextSceneButton.SetActive(true);
        if (currentBookMode == ReadingBookMode.Phrases)
        {
            PhrasesGrid.SetActive(true);
            PhrasesGridMSB.SetActive(true);
        }
        else if (currentBookMode == ReadingBookMode.Sentences)
        {
            SentencesGrid.SetActive(true);
            SentencesGridMSB.SetActive(true);
        }
        currentBookForm = form;
    }

    public void GetContentList()
    {
        switch (currentBookMode)
        {
            case ReadingBookMode.Vocabulary:
                content = ReadingBook.Instance.GetRequestedBook(requestedBook);
                break;

            case ReadingBookMode.Phrases:
                if (currentBookForm == ReadingBookForm.SightWords)
                {
                    content = PhrasesManager.Instance.GetCurrentEnabledDictionarySightWords();
                }
                else
                {
                    content = PhrasesManager.Instance.GetCurrentEnabledDictionaryPhrasesForReadingBook();
                }
                break;

            case ReadingBookMode.Sentences:
                if (currentBookForm == ReadingBookForm.SightWords)
                {
                    content = SentencesManager.Instance.GetCurrentEnabledDictionarySightWords();
                }
                else
                {
                    content = SentencesManager.Instance.GetCurrentEnabledDictionarySentencesForReadingBook();
                }
                break;
        }
    }

    public static void SetRequestedBook(Books book)
    {
        requestedBook = book;
    }

    public static void SetReadingBookMode(ReadingBookMode mode)
    {
        currentBookMode = mode;
    }

    void Next()
    {
        currentidx++;
        DisplayContent();
    }

    void Previous()
    {
        currentidx--;
        DisplayContent();
    }

    void DisplayContent()
    {
        if (currentidx == 0 && content.Count > 1)
        {
            nextButton.gameObject.SetActive(true);
            previousButton.gameObject.SetActive(false);
        }
        else if (currentidx == content.Count - 1)
        {
            nextButton.gameObject.SetActive(false);
            previousButton.gameObject.SetActive(true);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
            previousButton.gameObject.SetActive(true);
        }

        ContentPictureAudioTrio currentContent = content[currentidx];
        displayImage.sprite = currentContent.image;
        displayText.text = currentContent.content;
        displayImage.SetNativeSize();

        if (currentBookMode == ReadingBookMode.Phrases && currentBookForm == ReadingBookForm.PhrasesOrSentences)
        {
            displayButton.onClick.AddListener(() => { AudioManager.Instance.PhrasesAudioFunction(currentContent.content); });
        }
        else if (currentBookMode == ReadingBookMode.Sentences && currentBookForm == ReadingBookForm.PhrasesOrSentences)
        {
            string[] temp;
            temp = currentContent.content.Split("*");
            displayText.text = temp[0];
            Debug.Log(temp[0] + " * " + temp[1]);
            if (temp[0].Length > 30)
            {
                displayText.fontSize = 100;
            }
            if (temp[0].Length > 40)
            {
                displayText.fontSize = 80;
            }
            if (temp[0].Length > 50)
            {
                displayText.fontSize = 70;
            }
            displayButton.onClick.AddListener(() => { AudioManager.Instance.SentencesAudioFunction(temp[1], temp[0]); });
        }
        else
        {
            displayButton.onClick.AddListener(() => { AudioManager.Instance.WordAudioFunction(currentContent.content); });
        }


    }
}