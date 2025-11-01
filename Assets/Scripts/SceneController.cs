using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes
{
    Home,
    GameSelection,
    ReadingBookDisplay,
    ReadingBookDisplaySelection,
    EnableBooks,
    Vocabulary,
    Phrases,
    Sentences,
    VocabularyMatching,
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

    public void EnableBooks(string bookName)
    {
        if (System.Enum.TryParse(bookName, out Books book))
        {
            ReadingBook.Instance.SetBookEnabled(book, true);
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

    public void ExitApplication()
    {
        Application.Quit();
        Debug.Log("Application exited.");
    }
}