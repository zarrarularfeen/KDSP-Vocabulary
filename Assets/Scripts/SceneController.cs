using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum Scenes
{
    Home,
    GameSelection,
    ReadingBookDisplay,
    ReadingBookDisplaySelection,
    EnableBooksVocabulary,
    EnableBooksPhrases,
    EnableBooksSentences,
    Vocabulary,
    Phrases,
    Sentences,
    VocabularyMatching,
    PhrasesLevel,
    WordsDisplay,
    SentencesLevel,
    BatchSizeSet

}

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }
    public static int numberSelected;
    public static int rangeselected;
    public static Scenes currentScene;

    // // Add this flag to track first app launch
    // public static bool isFirstLaunch = true;
    // private float transitionTime = 1f;
    // [SerializeField] private Animator transitionAnimator;

    void Awake()
    {
        // Ensure only one instance of SceneController exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        Instance = this;
    }

    public void OpenReadingBookDisplay(string bookName)
    {
        currentScene = Scenes.ReadingBookDisplay;
        ReadingBookDisplay.SetRequestedBook(Books.Vocabulary1);
        if (System.Enum.TryParse(bookName, out Books book))
        {
            ReadingBookDisplay.SetRequestedBook(book);
        }
        SceneManager.LoadScene(currentScene.ToString());
    }

    public void EnableBooksVocabulary(string bookName)
    {
        if (System.Enum.TryParse(bookName, out Books book))
        {
            ReadingBook.Instance.SetBookEnabled(book, true);
        }
    }

    public void EnableBooksPhrases(string bookName)
    {
        if (System.Enum.TryParse(bookName, out Books book))
        {
            PhrasesManager.Instance.SetBookEnabled(book, true);
        }
    }

    public void OpenLevelSelect(string gameName)
    {
        currentScene = (Scenes)System.Enum.Parse(typeof(Scenes), gameName);
        // isFirstLaunch = false;
        // StartCoroutine(LoadlevelTransition());
        SceneManager.LoadScene(currentScene.ToString());
    }

    public void OpenLevelSelect(Scenes sceneName)
    {
        currentScene = sceneName;
        // isFirstLaunch = false;
        // StartCoroutine(LoadlevelTransition());
        SceneManager.LoadScene(currentScene.ToString());
    }

    public void OpenGameSelection()
    {
        currentScene = Scenes.GameSelection;
        // isFirstLaunch = false;
        // StartCoroutine(LoadlevelTransition());
        SceneManager.LoadScene(currentScene.ToString());
    }

    public void OpenHomeScreen()
    {
        currentScene = Scenes.Home;
        // Don't set isFirstLaunch = false here, because we're returning to home
        // StartCoroutine(LoadlevelTransition());
        SceneManager.LoadScene(currentScene.ToString());
    }

    public void ReloadScene()
    {
        // StartCoroutine(LoadlevelTransition());
        SceneManager.LoadScene(currentScene.ToString());
    }

    // IEnumerator LoadlevelTransition()
    // {
    //     transitionAnimator.SetTrigger("NextLevel");

    //     yield return new WaitForSeconds(transitionTime);

    //     if (AudioManager.Instance != null)
    //     {
    //         AudioManager.Instance.StopAllSFX();
    //     }

    //     SceneManager.LoadScene(currentScene.ToString());
    // }

    //Vocabulary Mode selection
    public void SetVocabularyMode(VocabularyMode mode)
    {
        VocabularyMatching.SetVocabularyMode(mode);
    }

    public void SetVocabularyModeMatch()
    {
        SetVocabularyMode(VocabularyMode.Match);
    }

    public void SetVocabularyModeSelect()
    {
        SetVocabularyMode(VocabularyMode.Select);
    }

    public void SetVocabularyModeName()
    {
        SetVocabularyMode(VocabularyMode.Name);
    }
    //Words Display Mode selection
    public void SetWordsDisplayMode(WordsDisplayMode mode)
    {
        WordsDisplay.SetWordsDisplayMode(mode);
    }

    public void SetWordsDisplayModeVocabulary()
    {
        SetWordsDisplayMode(WordsDisplayMode.Vocabulary);
    }
    public void SetWordsDisplayModeSightWords()
    {
        SetWordsDisplayMode(WordsDisplayMode.SightWords);
    }
    public void SetWordsDisplayModePhrases()
    {
        SetWordsDisplayMode(WordsDisplayMode.Phrases);
    }
    //Words Display Game Mode selection
    public void SetWordsDisplayGameMode(GameMode mode)
    {
        WordsDisplay.SetGameMode(mode);
    }
    public void SetWordsDisplayGameModeVocabulary()
    {
        SetWordsDisplayGameMode(GameMode.Vocabulary);
    }
    public void SetWordsDisplayGameModePhrases()
    {
        SetWordsDisplayGameMode(GameMode.Phrases);
    }
    public void SetWordsDisplayGameModeSentences()
    {
        SetWordsDisplayGameMode(GameMode.Sentences);
    }

    //Phrases Level Mode selection
    public void SetPhrasesMode(PhrasesLevelMode mode)
    {
        PhrasesLevelManager.SetPhrasesLevelMode(mode);
    }
    public void SetPhrasesModeMatch()
    {
        SetPhrasesMode(PhrasesLevelMode.MatchSghtWord);
    }
    public void SetPhrasesModeSelect()
    {
        SetPhrasesMode(PhrasesLevelMode.SelectSightWord);
    }
    public void SetPhrasesModeRead()
    {
        SetPhrasesMode(PhrasesLevelMode.ReadSightWord);
    }
    public void SetPhrasesModeUnderstandSightWord()
    {
        SetPhrasesMode(PhrasesLevelMode.UnderstandSightWord);
    }
    public void SetPhrasesModeUnderstandPhrase()
    {
        SetPhrasesMode(PhrasesLevelMode.UnderstandPhrase);
    }
    //Sentences Level Mode selection
    public void SetSentencesMode(SentencesLevelMode mode)
    {
        SentencesLevelManager.SetSentencesLevelMode(mode);
    }
    public void SetSentencesModeBuild()
    {
        SetSentencesMode(SentencesLevelMode.BuildSentences);
    }
    public void SetSentencesModeMatchPicture()
    {
        SetSentencesMode(SentencesLevelMode.MatchPicture);
    }
    public void SetSentencesModeSelectPicture()
    {
        SetSentencesMode(SentencesLevelMode.SelectPicture);
    }
    public void SetSentencesModeNamePicture()
    {
        SetSentencesMode(SentencesLevelMode.NamePicture);
    }
    public void SetSentencesModeMatchSightWord()
    {
        SetSentencesMode(SentencesLevelMode.MatchSightWord);
    }
    public void SetSentencesModeSelectSightWord()
    {
        SetSentencesMode(SentencesLevelMode.SelectSightWord);
    }
    public void SetSentencesModeReadSightWord()
    {
        SetSentencesMode(SentencesLevelMode.ReadSightWord);
    }
    public void SetSentencesModeMatchSightWordPicture()
    {
        SetSentencesMode(SentencesLevelMode.MatchSightWordPicture);
    }
    public void SetSentencesModeMatchSentencesPicture()
    {
        SetSentencesMode(SentencesLevelMode.MatchSentencesPicture);
    }
    public void SetSentencesModeReadSentences()
    {
        SetSentencesMode(SentencesLevelMode.ReadSentences);
    }

    public void SetNextSceneBatchSizeSettingVocabulary()
    {
        BatchSizeSetting.nextScene = Scenes.VocabularyMatching;
    }
    public void SetNextSceneBatchSizeSettingPhrases()
    {
        BatchSizeSetting.nextScene = Scenes.PhrasesLevel;
    }
    public void SetNextSceneBatchSizeSettingSentences()
    {
        BatchSizeSetting.nextScene = Scenes.SentencesLevel;
    }
    public void OpenBatchSizeSetting(Scenes sceneName)
    {
        BatchSizeSetting.nextScene = sceneName;
        currentScene = Scenes.BatchSizeSet;
        SceneManager.LoadScene(currentScene.ToString());
    }

    public void ExitApplication()
    {
        Application.Quit();
        Debug.Log("Application exited.");
    }
}