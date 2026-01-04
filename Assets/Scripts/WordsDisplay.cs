using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using NUnit.Framework;
using Unity.VisualScripting;

public enum WordsDisplayMode
{
    Vocabulary,
    SightWords,
    Phrases
}

public enum GameMode
{
    Vocabulary,
    Phrases,
    Sentences
}
public class WordsDisplay : MonoBehaviour
{

    private List<ContentPictureAudioTrio> content = new List<ContentPictureAudioTrio>();
    private List<ContextListEntry> contextList = new List<ContextListEntry>();
    private TextMeshProUGUI wordText;
    [SerializeField] private Button wordButton;
    [SerializeField] private Button NextButton;
    [SerializeField] private Button BackButton;
    [SerializeField] private GridLayoutGroup wordsGrid;
    public static WordsDisplayMode currentMode;
    public static GameMode currentGameMode;
    public static WordsDisplay Instance { get; private set; }


    void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (currentMode == WordsDisplayMode.Vocabulary)
        {
            content = ReadingBook.Instance.GetCurrentEnabledDictionary();
        }
        else if (currentMode == WordsDisplayMode.Phrases)
        {
            contextList = PhrasesManager.Instance.GetCurrentEnabledDictionaryPhrases();
        }
        else if (currentMode == WordsDisplayMode.SightWords)
        {
            content = PhrasesManager.Instance.GetCurrentEnabledDictionarySightWords();
        }

        foreach (ContentPictureAudioTrio pair in content)
        {
            Debug.Log(pair.content);
        }

        DisplayWords();
        OnNextButtonClicked(NextButton);
        OnBackButtonClicked(BackButton);
    }
    // Update is called once per frame
    public static void SetWordsDisplayMode(WordsDisplayMode mode)
    {
        currentMode = mode;
    }

    public static void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
    }

    void DisplayWords()
    {
        if (currentMode == WordsDisplayMode.SightWords || currentMode == WordsDisplayMode.Vocabulary)
            foreach (ContentPictureAudioTrio pair in content)
            {
                Button newButton = Instantiate(wordButton, wordsGrid.transform);
                newButton.gameObject.SetActive(true);
                wordText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                wordText.text = pair.content;
                newButton.onClick.AddListener(() => OnWordButtonClicked(pair));
                Debug.Log("button added to grid: " + pair.content);
            }
        else if (currentMode == WordsDisplayMode.Phrases)
        {
            foreach (ContextListEntry entry in contextList)
            {
                Button newButton = Instantiate(wordButton, wordsGrid.transform);
                newButton.gameObject.SetActive(true);
                wordText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                wordText.text = entry.context;
                newButton.onClick.AddListener(() => OnWordButtonClickedPhrases(entry));
                Debug.Log("button added to grid phrases: " + entry.context);
            }
        }
    }

    void OnWordButtonClicked(ContentPictureAudioTrio pair)
    {
        Debug.Log("Word button clicked: " + pair.content);
        // Add your logic here for what happens when a word button is clicked
        if (!VocabularyMatching.selectedContent.Contains(pair))
        {
            VocabularyMatching.selectedContent.Add(pair);
            Debug.Log("Added to selectedContent: " + pair.content);
        }
        else
        {
            Debug.Log("Already in selectedContent: " + pair.content);
        }
    }

    void OnWordButtonClickedPhrases(ContextListEntry entry)
    {
        Debug.Log("Phrase context button clicked: " + entry.context);
        // Add your logic here for what happens when a phrase context button is clicked
        if (!PhrasesLevelManager.selectedContextList.Contains(entry))
        {
            PhrasesLevelManager.selectedContextList.Add(entry);
            Debug.Log("Added to selectedContextList: " + entry.context);
        }
        else
        {
            Debug.Log("Already in selectedContextList: " + entry.context);
        }
    }

    void OnNextButtonClicked(Button nextButton)
    {
        if (currentGameMode == GameMode.Vocabulary)
            nextButton.onClick.AddListener(() => SceneController.Instance.OpenLevelSelect("VocabularyMatching"));
        else if (currentGameMode == GameMode.Phrases)
            nextButton.onClick.AddListener(() => SceneController.Instance.OpenLevelSelect("PhrasesLevel"));
    }

    void OnBackButtonClicked(Button backButton)
    {
        if (currentGameMode == GameMode.Vocabulary)
            backButton.onClick.AddListener(() => SceneController.Instance.OpenLevelSelect("EnableBooksVocabulary"));
        else if (currentGameMode == GameMode.Phrases)
            backButton.onClick.AddListener(() => SceneController.Instance.OpenLevelSelect("EnableBooksPhrases"));
    }

}
