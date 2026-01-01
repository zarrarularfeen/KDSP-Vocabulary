using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes
{
    Home,
    GameSelection,
    ReadingBookDisplay,
    ReadingBookDisplaySelection,
    EnableBooksVocabulary,
    EnableBooksPhrases,
    Vocabulary,
    Phrases,
    Sentences,
    VocabularyMatching,
    PhrasesLevel,
    WordsDisplay

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
    public void ExitApplication()
    {
        Application.Quit();
        Debug.Log("Application exited.");
    }
}