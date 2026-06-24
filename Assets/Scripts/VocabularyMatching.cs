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
    [SerializeField] private Button TickButton;

    private int currentIndex = 0;
    private int currentBatchStart = 0;
    public static int batchSize = 2; // can be 1 to 4

    public static VocabularyMatching Instance { get; private set; }

    public static VocabularyMode currentMode = VocabularyMode.Match;

    [SerializeField] private Sprite correctSprite;
    [SerializeField] private Sprite wrongSprite;
    private bool isNameAudioPlaying = false;
    private readonly List<int> currentBatchOrder = new List<int>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
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
                questionsGrid.cellSize = new Vector2(420, 420);
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
        NameNextButton.onClick.AddListener(() => OnNextButtonClicked());
        NamePrevButton.onClick.AddListener(() => OnPrevButtonClicked());
       
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

    void SetContentMatch()
    {
        ClearGrids();
        currentIndex = 0;
        currentBatchStart = 0;
        TickButton.gameObject.SetActive(false);
        SpawnNextBatch();
    }

    void SetContentSelect()
    {
        ClearGrids();
        currentIndex = 0;
        currentBatchStart = 0;
        TickButton.gameObject.SetActive(false);
        SpawnSelectButtons(currentBatchStart, GetBatchEnd(currentBatchStart), -1, 0);

    }

    void SetContentName()
    {
        ClearGrids();
        currentIndex = 0;
        NameNextButton.gameObject.SetActive(true);
        NamePrevButton.gameObject.SetActive(true);
        SetNameCard(currentIndex);
        UpdateTickState();

    }

    void SpawnNextBatch()
    {
        ClearGrids();

        int batchStart = currentBatchStart;
        int batchEnd = GetBatchEnd(batchStart);

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
                    childRect.sizeDelta = new Vector2(800, 760);
                    break;
            }

        }

        SpawnNextDraggable();
    }

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

        AudioManager.Instance.MatchWithFunction(selectedContent[contentIndex].content);
        AudioManager.Instance.WaitForCurrentAudio();

    }

    int GetBatchEnd(int batchStart)
    {
        return Mathf.Min(batchStart + batchSize, selectedContent.Count);
    }

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
        yield return new WaitForSeconds(1.25f);

        AudioManager.Instance.PlayPositiveReinforcementSound();
        yield return new WaitForSeconds(1.25f);
        currentIndex++;
        if (currentIndex >= GetBatchEnd(currentBatchStart))
        {
            UpdateTickState();
            yield break;
        }

        SpawnNextDraggable();
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
                childRect.sizeDelta = new Vector2(800, 760);
                break;
        }
        selectCard.onClick.AddListener(() => OnNameCardClicked(index, selectCard));
    }

    void OnNameCardClicked(int index, Button sourceButton)
    {
        Debug.Log("Name card clicked: " + selectedContent[index].content);
        AudioManager.Instance.WordAudioFunction(selectedContent[index].content);
    }

    void OnNameNextButtonClicked()
    {
        currentIndex++;
        if (currentIndex >= selectedContent.Count - 1)
        {
            currentIndex = selectedContent.Count - 1;
        }
        SetNameCard(currentIndex);
        UpdateTickState();
    }

    void OnNamePrevButtonClicked()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = 0;
        }
        SetNameCard(currentIndex);
        UpdateTickState();
    }

    void SpawnSelectButtons(int batchStart, int batchEnd, int previousBatch, int currentBatch)
    {
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
                        childRect.sizeDelta = new Vector2(800, 760);
                        break;
                }
            }
        }
        int contentIndex = GetCurrentBatchContentIndex();
        AudioManager.Instance.ShowMeFunction(selectedContent[contentIndex].content);
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
        int batchStart = currentMode == VocabularyMode.Name ? 0 : currentBatchStart;
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

        }
    }

    private IEnumerator HandleCorrectSelection(string selectedWord, Button sourceButton)
    {
        Image img = null;
        if (sourceButton != null && sourceButton.transform != null && sourceButton.transform.childCount > 1)
        {
            img = sourceButton.transform.GetChild(1).GetComponent<Image>();
        }
        if (img != null)
        {
            yield return StartCoroutine(FeedBackFlicker(img, correctSprite, 0.2f, 3, sourceButton));
        }
        AudioManager.Instance.PlayCorrectSound();
        yield return new WaitForSeconds(1.25f);
        AudioManager.Instance.PlayPositiveReinforcementSound();
        yield return new WaitForSeconds(1.25f);

        currentIndex++;
        int batchEnd = GetBatchEnd(currentBatchStart);
        if (currentIndex >= batchEnd)
        {
            UpdateTickState();
            yield break;
        }
        yield return new WaitForSeconds(2.5f);
        SpawnSelectButtons(currentBatchStart, batchEnd, currentBatchStart / batchSize, currentBatchStart / batchSize);
    }

    void OnNextButtonClicked()
    {
        switch (currentMode)
        {
            case VocabularyMode.Name:
                OnNameNextButtonClicked();
                break;
            case VocabularyMode.Match:
            case VocabularyMode.Select:
                MoveBatch(1);
                break;
        }
    }

    void OnPrevButtonClicked()
    {
        switch (currentMode)
        {
            case VocabularyMode.Name:
                OnNamePrevButtonClicked();
                break;
            case VocabularyMode.Match:
            case VocabularyMode.Select:
                MoveBatch(-1);
                break;
        }
    }

    void MoveBatch(int direction)
    {
        int nextBatchStart = currentBatchStart + (direction * batchSize);
        int finalBatchStart = GetFinalBatchStart();

        if (nextBatchStart < 0 || nextBatchStart > finalBatchStart)
        {
            return;
        }

        currentBatchStart = nextBatchStart;
        currentIndex = currentBatchStart;

        switch (currentMode)
        {
            case VocabularyMode.Match:
                SpawnNextBatch();
                break;
            case VocabularyMode.Select:
                SpawnSelectButtons(currentBatchStart, GetBatchEnd(currentBatchStart), -1, currentBatchStart / batchSize);
                break;
        }

        UpdateTickState();
    }

    int GetFinalBatchStart()
    {
        return selectedContent.Count == 0 ? 0 : ((selectedContent.Count - 1) / batchSize) * batchSize;
    }

    void UpdateTickState()
    {
        if (TickButton == null)
        {
            return;
        }

        bool showTick = false;
        switch (currentMode)
        {
            case VocabularyMode.Name:
                showTick = selectedContent.Count > 0 && currentIndex >= selectedContent.Count - 1;
                break;
            case VocabularyMode.Match:
            case VocabularyMode.Select:
                showTick = selectedContent.Count > 0 && currentBatchStart >= GetFinalBatchStart() && currentIndex >= GetBatchEnd(currentBatchStart);
                break;
        }

        TickButton.gameObject.SetActive(showTick);
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
