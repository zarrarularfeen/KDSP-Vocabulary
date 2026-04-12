using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System;

public enum VocabularyMode
{
    Match,
    Select,
    Name

}
public class VocabularyMatching : MonoBehaviour
{
    public static List<ContentPictureAudioTrio> selectedContent = new List<ContentPictureAudioTrio>();
    [SerializeField] private GridLayoutGroup questionsGrid;
    [SerializeField] private GridLayoutGroup answersGrid;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject dragPrefab;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button NameNextButton;
    [SerializeField] private Button NamePrevButton;

    private int currentIndex = 0;
    public static int batchSize = 2; // can be 1 to 4

    public static VocabularyMatching Instance { get; private set; }

    public static VocabularyMode currentMode = VocabularyMode.Match;

    [SerializeField] private Sprite correctSprite;
    [SerializeField] private Sprite wrongSprite;
    private bool isNameAudioPlaying = false;
    private readonly List<int> currentBatchOrder = new List<int>();

    void Awake()
    {
        // Ensure only one instance of SceneController exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        Instance = this;
    }
    void Start()
    {
        Debug.Log("VocabularyMatching started with selectedContent: " + selectedContent.Count);

        foreach (ContentPictureAudioTrio pair in selectedContent)
        {
            Debug.Log(pair.content);
        }
        SetContent(currentMode);

        switch (batchSize)
        {
            case 2:
                questionsGrid.cellSize = new Vector2(550, 550);
                questionsGrid.spacing = new Vector2(100, 0);
                break;
            case 3:
                questionsGrid.cellSize = new Vector2(550, 550);
                questionsGrid.spacing = new Vector2(100, 0);
                break;
            case 4:
                questionsGrid.cellSize = new Vector2(450, 450);
                questionsGrid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 280);
                break;
        }
        if (currentMode == VocabularyMode.Select)
        {
            questionsGrid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 280);
            answersGrid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -140);
        }
        else if (currentMode == VocabularyMode.Name)
        {
            questionsGrid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 190);
            answersGrid.gameObject.SetActive(false);
        }
        else if (currentMode == VocabularyMode.Match)
        {
            answersGrid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -140);
        }
        
        backButton.onClick.AddListener(() => OnBackButtonClicked(backButton));
    }

    public static void SetVocabularyMode(VocabularyMode mode)
    {
        currentMode = mode;

    }

    void SetContent(VocabularyMode mode)
    {
        switch (mode)
        {
            case VocabularyMode.Match:
                SetContentMatch();
                break;
            case VocabularyMode.Select:
                SetContentSelect();
                break;
            case VocabularyMode.Name:
                SetContentName();
                break;
        }
    }

    // Set up content for Matching mode and Calls relevant methods
    void SetContentMatch()
    {
        ClearGrids();

        SpawnNextBatch();
    }

    //Set up content for Select mode and Calls relevant methods
    void SetContentSelect()
    {
        // Implement Select mode content setup
        ClearGrids();
        currentIndex = 0; // start from the first word
        int batchStart = 0;
        int batchEnd = Mathf.Min(batchSize, selectedContent.Count);

        SpawnSelectButtons(batchStart, batchEnd, -1, 0);

    }

    // Set up content for Name mode and Calls relevant methods
    void SetContentName()
    {
        // Implement Select mode content setup
        ClearGrids();
        int currentIndex = 0;
        NameNextButton.gameObject.SetActive(true);
        NamePrevButton.gameObject.SetActive(true);
        SetNameCard(currentIndex);
        NameNextButton.onClick.AddListener(() => OnNameNextButtonClicked());
        NamePrevButton.onClick.AddListener(() => OnNamePrevButtonClicked());

    }

    // Spawn next batch of 4 words in questions grid for Matching mode
    void SpawnNextBatch()
    {
        ClearGrids();

        // int end = Mathf.Min(currentBatchStart + 4, selectedContent.Count);
        int currentBatch = currentIndex / batchSize;
        int batchStart = currentBatch * batchSize;
        int batchEnd = Mathf.Min(batchStart + batchSize, selectedContent.Count);

        BuildCurrentBatchOrder(batchStart, batchEnd);

        for (int i = batchStart; i < batchEnd; i++)
        {
            GameObject target = Instantiate(targetPrefab, questionsGrid.transform);
            string word = selectedContent[i].content;
            target.GetComponentInChildren<TextMeshProUGUI>().text = "";
            target.GetComponentInChildren<Image>().sprite = selectedContent[i].image;
            DropTarget dropTarget = target.GetComponent<DropTarget>();
            dropTarget.word = word;
            dropTarget.targetPrefab = target;
            RectTransform childRect = target.transform.GetChild(1).GetComponent<RectTransform>();
            switch (batchSize)
            {
                case 2:
                    childRect.sizeDelta = new Vector2(1000, 1000);
                    break;
                case 3:
                    childRect.sizeDelta = new Vector2(1000, 1000);
                    break;
                case 4:
                    childRect.sizeDelta = new Vector2(840, 800);
                    break;
            }

        }

        SpawnNextDraggable();
    }

    // Spawn next draggable card in answers grid for Matching mode
    void SpawnNextDraggable()
    {
        if (currentIndex >= selectedContent.Count)
        {
            return;
        }

        int contentIndex = GetCurrentBatchContentIndex();

        GameObject dragCard = Instantiate(dragPrefab, answersGrid.transform);
        string word = selectedContent[contentIndex].content;
        dragCard.GetComponentInChildren<TextMeshProUGUI>().text = "";
        dragCard.GetComponentInChildren<Image>().sprite = selectedContent[contentIndex].image;
        DraggableCard draggable = dragCard.GetComponent<DraggableCard>();
        draggable.word = word;
        Debug.Log("Spawned draggable for word: " + word);

        // AudioManager.Instance.MatchWithFunction(selectedContent[contentIndex].audio);
        AudioManager.Instance.MatchWithFunction(selectedContent[contentIndex].content);
        AudioManager.Instance.WaitForCurrentAudio();

    }

    // Called when a correct match is made
    public void OnCorrectMatch(GameObject targetPrefab)
    {
        StartCoroutine(HandleCorrectMatch(targetPrefab));
    }

    private IEnumerator HandleCorrectMatch(GameObject targetPrefab)
    {
        Image img = null;
        if (targetPrefab != null && targetPrefab.transform != null && targetPrefab.transform.childCount > 1)
        {
            img = targetPrefab.transform.GetChild(1).GetComponent<Image>();
        }

        if (BlockerManager.Instance != null)
        {
            BlockerManager.Instance.ActivateBlocker(1.0f);
        }

        if (img != null)
        {
            yield return StartCoroutine(FeedBackFlicker(img, correctSprite, 0.2f, 3));
        }
        AudioManager.Instance.PlayCorrectSound();
        // AudioManager.Instance.WaitForCurrentAudio();
        yield return new WaitForSeconds(1.25f);

        AudioManager.Instance.PlayPositiveReinforcementSound();
        // AudioManager.Instance.WaitForCurrentAudio();
        yield return new WaitForSeconds(1.25f);
        // CreateOutline(Color.green);
        currentIndex++;
        // Check if all words are done
        if (currentIndex >= selectedContent.Count)
        {
            Debug.Log("All words matched!");
            selectedContent.Clear();
            StartCoroutine(SceneDelayLoad("Vocabulary", 1.5f));
            yield break;
        }

        // If currentIndex has passed the current batch, spawn next batch
        if (currentIndex % batchSize == 0)
        {
            SpawnNextBatch(); // This will automatically spawn the first draggable of the new batch
        }
        else
        {
            // Spawn next draggable in current batch
            SpawnNextDraggable();
        }
    }

    public void OnIncorrectMatch(GameObject targetPrefab)
    {
        Image img = null;
        if (targetPrefab != null && targetPrefab.transform != null && targetPrefab.transform.childCount > 1)
        {
            img = targetPrefab.transform.GetChild(1).GetComponent<Image>();
        }

        if (BlockerManager.Instance != null)
        {
            BlockerManager.Instance.ActivateBlocker(1.0f);
        }
        if (img != null)
        {
            StartCoroutine(FeedBackFlicker(img, wrongSprite, 0.2f, 3));
        }
        AudioManager.Instance.PlayWrongSound();
        // AudioManager.Instance.WaitForCurrentAudio();
    }

    void SetNameCard(int index)
    {
        ClearGrids();
        Button selectCard = Instantiate(selectButton, questionsGrid.transform);
        
        

        selectCard.gameObject.SetActive(true);
        selectCard.GetComponentInChildren<Image>().sprite = selectedContent[index].image;

        RectTransform childRect = selectCard.transform.GetChild(1).GetComponent<RectTransform>();
        switch (batchSize)
        {
            case 2:
                childRect.sizeDelta = new Vector2(1000, 1000);
                break;
            case 3:
                childRect.sizeDelta = new Vector2(1000, 1000);
                break;
            case 4:
                childRect.sizeDelta = new Vector2(840, 800);
                break;
        }
        selectCard.onClick.AddListener(() => OnNameCardClicked(index, selectCard));
    }

    void OnNameCardClicked(int index, Button sourceButton)
    {
        // if (isNameAudioPlaying) return;
        Debug.Log("Name card clicked: " + selectedContent[index].content);
        AudioManager.Instance.WordAudioFunction(selectedContent[index].content);
        // StartCoroutine(PlayNameThenAdvance(index, sourceButton));
    }

    // IEnumerator PlayNameThenAdvance(int index, Button sourceButton)
    // {
    //     isNameAudioPlaying = true;
    //     if (sourceButton != null) sourceButton.interactable = false;


    //     // AudioManager.Instance.PlayGivenAudioDelayed(selectedContent[index].audio, 2.0f);
    //     AudioManager.Instance.WordAudioFunction(selectedContent[index].content);

    //     // float waitTime = 2.0f + (selectedContent[index].audio != null ? selectedContent[index].audio.length : 0f);
    //     float waitTime = 2.5f;
    //     yield return new WaitForSeconds(waitTime);

    //     isNameAudioPlaying = false;
    //     if (index + 1 < selectedContent.Count)
    //     {
    //         SetNameCard(index + 1);
    //     }
    //     else
    //     {
    //         Debug.Log("All Name cards shown!");
    //         selectedContent.Clear();
    //         StartCoroutine(SceneDelayLoad("Vocabulary", 1.5f));
    //     }
    // }

    void OnNameNextButtonClicked()
    {
        currentIndex++;
        if (currentIndex >= selectedContent.Count)
        {
            currentIndex = 0; // wrap around to the first card
        }
        SetNameCard(currentIndex);
    }

    void OnNamePrevButtonClicked()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = selectedContent.Count - 1; // wrap around to the last card
        }
        SetNameCard(currentIndex);
    }

    void SpawnSelectButtons(int batchStart, int batchEnd, int previousBatch, int currentBatch)
    {


        // int end = Mathf.Min(currentBatchStart + 4, selectedContent.Count);
        if (previousBatch != currentBatch)
        {
            BuildCurrentBatchOrder(batchStart, batchEnd);
            ClearGrids();
            for (int i = batchStart; i < batchEnd; i++)
            {
                Button selectCard = Instantiate(selectButton, questionsGrid.transform);
                selectCard.gameObject.SetActive(true);
                selectCard.GetComponentInChildren<Image>().sprite = selectedContent[i].image;
                SelectCard selectCardData = selectCard.GetComponent<SelectCard>();
                selectCardData.word = selectedContent[i].content;
                selectCard.onClick.AddListener(() => OnSelectCardClicked(selectCardData.word, selectCard));
                RectTransform childRect = selectCard.transform.GetChild(1).GetComponent<RectTransform>();
                switch (batchSize)
                {
                    case 2:
                        childRect.sizeDelta = new Vector2(1000, 1000);
                        break;
                    case 3:
                        childRect.sizeDelta = new Vector2(1000, 1000);
                        break;
                    case 4:
                        childRect.sizeDelta = new Vector2(840, 800);
                        break;
                }
            }
        }
        int contentIndex = GetCurrentBatchContentIndex();
        // AudioManager.Instance.ShowMeFunction(selectedContent[contentIndex].audio);
        AudioManager.Instance.ShowMeFunction(selectedContent[contentIndex].content);
        // AudioManager.Instance.WaitForCurrentAudio();
    }

    void BuildCurrentBatchOrder(int batchStart, int batchEnd)
    {
        currentBatchOrder.Clear();
        for (int i = batchStart; i < batchEnd; i++)
        {
            currentBatchOrder.Add(i);
        }

        for (int i = currentBatchOrder.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            int temp = currentBatchOrder[i];
            currentBatchOrder[i] = currentBatchOrder[j];
            currentBatchOrder[j] = temp;
        }
    }

    int GetCurrentBatchContentIndex()
    {
        int batchStart = (currentIndex / batchSize) * batchSize;
        int indexInBatch = currentIndex - batchStart;

        if (indexInBatch >= 0 && indexInBatch < currentBatchOrder.Count)
        {
            return currentBatchOrder[indexInBatch];
        }

        return currentIndex;
    }

    void OnSelectCardClicked(string selectedWord, Button sourceButton)
    {
        int contentIndex = GetCurrentBatchContentIndex();
        string correctWord = selectedContent[contentIndex].content;
        if (selectedWord == correctWord)
        {
            StartCoroutine(HandleCorrectSelection(selectedWord, sourceButton));
        }
        else
        {
            Debug.Log("Incorrect selection for word: " + selectedWord);
            Image img = null;
            if (sourceButton != null && sourceButton.transform != null && sourceButton.transform.childCount > 1)
            {
                img = sourceButton.transform.GetChild(1).GetComponent<Image>();
            }
            if (img != null)
            {
                StartCoroutine(FeedBackFlicker(img, wrongSprite, 0.2f, 3, sourceButton));
            }
            AudioManager.Instance.PlayWrongSound();
            // AudioManager.Instance.WaitForCurrentAudio();

        }
    }

    private IEnumerator HandleCorrectSelection(string selectedWord, Button sourceButton)
    {
        Debug.Log("Correct selection for word: " + selectedWord);
        Debug.Log("Going to flicker correct sprite");
        Image img = null;
        if (sourceButton != null && sourceButton.transform != null && sourceButton.transform.childCount > 1)
        {
            img = sourceButton.transform.GetChild(1).GetComponent<Image>();
        }
        if (img != null)
        {
            yield return StartCoroutine(FeedBackFlicker(img, correctSprite, 0.2f, 3, sourceButton));
        }
        Debug.Log("Played correct sound");
        AudioManager.Instance.PlayCorrectSound();
        yield return new WaitForSeconds(1.25f);
        AudioManager.Instance.PlayPositiveReinforcementSound();
        yield return new WaitForSeconds(1.25f);

        int previousBatch = currentIndex / batchSize;

        currentIndex++;
        if (currentIndex >= selectedContent.Count)
        {
            Debug.Log("All words selected!");
            selectedContent.Clear();
            StartCoroutine(SceneDelayLoad("Vocabulary", 1.5f));
            yield break;
        }
        int currentBatch = currentIndex / batchSize;
        int batchStart = currentBatch * batchSize;
        int batchEnd = Mathf.Min(batchStart + batchSize, selectedContent.Count);
        //if we have finished current batch of 4, move to next batch
        yield return new WaitForSeconds(2.5f);
        SpawnSelectButtons(batchStart, batchEnd, previousBatch, currentBatch);
    }

    IEnumerator FeedBackFlicker(Image image, Sprite feedbackSprite, float interval, int count, Button sourceButton = null)
    {
        if (image == null)
        {
            yield break;
        }

        Sprite originalSprite = image.sprite;

        bool buttonWasDisabled = false;
        if (sourceButton)
        {
            sourceButton.interactable = false;
            buttonWasDisabled = true;
        }

        bool aborted = false;
        for (int i = 0; i < count; i++)
        {
            if (!image)
            {
                aborted = true;
                break;
            }
            image.sprite = feedbackSprite;
            yield return new WaitForSeconds(interval);

            if (!image)
            {
                aborted = true;
                break;
            }
            image.sprite = originalSprite;
            yield return new WaitForSeconds(interval);
        }

        if (buttonWasDisabled && sourceButton)
        {
            sourceButton.interactable = true;
        }

        if (aborted)
        {
            yield break;
        }
    }

    IEnumerator SceneDelayLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneController.Instance.OpenLevelSelect(sceneName);
    }
    void OnBackButtonClicked(Button backButton)
    {
        
        selectedContent.Clear();
    }

    void ClearGrids()
    {
        foreach (Transform child in questionsGrid.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in answersGrid.transform)
        {
            Destroy(child.gameObject);
        }
    }


}
