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
            // content = PhrasesManager.Instance.GetCurrentEnabledDictionaryPhrases();
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
        foreach (ContentPictureAudioTrio pair in content)
        {
            Button newButton = Instantiate(wordButton, wordsGrid.transform);
            newButton.gameObject.SetActive(true);
            wordText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            wordText.text = pair.content;
            newButton.onClick.AddListener(() => OnWordButtonClicked(pair));
            Debug.Log("button added to grid: " + pair.content);
        }
    }

    void OnWordButtonClicked(ContentPictureAudioTrio pair)
    {
        Debug.Log("Word button clicked: " + pair.content);
        // Add your logic here for what happens when a word button is clicked

        if (currentGameMode == GameMode.Vocabulary)
        {
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
        else if (currentGameMode == GameMode.Phrases)
        {
            if (!PhrasesLevelManager.selectedContent.Contains(pair))
            {
                PhrasesLevelManager.selectedContent.Add(pair);
                Debug.Log("Added to selectedContent: " + pair.content);
            }
            else
            {
                Debug.Log("Already in selectedContent: " + pair.content);
            }
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
