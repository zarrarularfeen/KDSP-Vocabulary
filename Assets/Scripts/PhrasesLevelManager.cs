using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System;

public enum PhrasesLevelMode
{
    MatchSghtWord,
    SelectSightWord,
    ReadSightWord,
    UnderstandSightWord,
    UnderstandPhrase,

}
public class PhrasesLevelManager : MonoBehaviour
{
    //Words list selected from wordsdisplay
    public static List<ContentPictureAudioTrio> selectedContent = new List<ContentPictureAudioTrio>();
    //Phrases book list selected from wordsdisplay
    public static List<ContextListEntry> selectedContextList = new List<ContextListEntry>();
    [SerializeField] private GridLayoutGroup questionsGrid;
    [SerializeField] private GridLayoutGroup answersGrid;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject dragPrefab;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button backButton;


    private int currentIndex = 0;
    public static int batchSize = 2; // can be 1 to 4
    public static PhrasesLevelManager Instance { get; private set; }
    public static PhrasesLevelMode currentMode = PhrasesLevelMode.MatchSghtWord;
    [SerializeField] private Sprite correctSprite;
    [SerializeField] private Sprite wrongSprite;
    private bool isNameAudioPlaying = false;

    void Awake()
    {
        // Ensure only one instance of SceneController exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("PhrasesLevel started with selectedContent: " + selectedContent.Count);

        foreach (ContentPictureAudioTrio pair in selectedContent)
        {
            Debug.Log(pair.content);
        }

        Debug.Log("PhrasesLevel started with selectedContextList: " + selectedContextList.Count);

        foreach (ContextListEntry entry in selectedContextList)
        {
            Debug.Log(entry.context);
            foreach (ContentPictureAudioTrio pair in entry.list)
            {
                Debug.Log(" - " + pair.content);
            }
        }

        foreach (ContextListEntry entry in selectedContextList)
        {
            foreach (ContentPictureAudioTrio pair in entry.list)
            {
                if (!selectedContent.Contains(pair))
                {
                    selectedContent.Add(pair);
                }
            }
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
                questionsGrid.cellSize = new Vector2(440, 440);
                questionsGrid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 280);

                break;

        }
        if (currentMode == PhrasesLevelMode.SelectSightWord || currentMode == PhrasesLevelMode.UnderstandSightWord || currentMode == PhrasesLevelMode.UnderstandPhrase)
        {
            questionsGrid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 280);
            answersGrid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
        }
        else if (currentMode == PhrasesLevelMode.ReadSightWord)
        {
            questionsGrid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 190);
            answersGrid.gameObject.SetActive(false);
        }

        backButton.onClick.AddListener(() => OnBackButtonClicked(backButton));
    }

    public static void SetPhrasesLevelMode(PhrasesLevelMode mode)
    {
        currentMode = mode;

    }



    void SetContent(PhrasesLevelMode mode)
    {
        switch (mode)
        {
            case PhrasesLevelMode.MatchSghtWord:
                SetContentMatch();
                break;
            case PhrasesLevelMode.SelectSightWord:
                // Set content for Select mode
                SetContentSelect();
                break;
            case PhrasesLevelMode.ReadSightWord:
                // Set content for Read mode
                SetContentName();
                break;
            case PhrasesLevelMode.UnderstandSightWord:
                // Set content for Understand mode
                SetContentMatch();
                break;
            case PhrasesLevelMode.UnderstandPhrase:
                // Set content for Understand Phrase mode
                SetContentMatch();
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
        SetNameCard(0);

    }
    // Spawn next batch of 4 words in questions grid for Matching mode
    void SpawnNextBatch()
    {
        ClearGrids();

        // int end = Mathf.Min(currentBatchStart + 4, selectedContent.Count);
        int currentBatch = currentIndex / batchSize;
        int batchStart = currentBatch * batchSize;
        int batchEnd = Mathf.Min(batchStart + batchSize, selectedContent.Count);

        for (int i = batchStart; i < batchEnd; i++)
        {
            GameObject target = Instantiate(targetPrefab, questionsGrid.transform);
            string word = selectedContent[i].content;
            DropTarget dropTarget = target.GetComponent<DropTarget>();
            dropTarget.word = word;
            dropTarget.targetPrefab = target;
            if (currentMode == PhrasesLevelMode.UnderstandSightWord || currentMode == PhrasesLevelMode.UnderstandPhrase)
            {
                target.GetComponentInChildren<TextMeshProUGUI>().text = "";
                target.GetComponentInChildren<Image>().sprite = selectedContent[i].image;
            }
            else
            {
                target.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[i].content;
                // target.GetComponentInChildren<Image>().sprite = null;
                target.GetComponentInChildren<TextMeshProUGUI>().fontSize = 90;
                target.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            }

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

        GameObject dragCard = Instantiate(dragPrefab, answersGrid.transform);
        string word = selectedContent[currentIndex].content;
        DraggableCard draggable = dragCard.GetComponent<DraggableCard>();
        draggable.word = word;


        dragCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[currentIndex].content;
        // dragCard.GetComponentInChildren<Image>().sprite = null;
        dragCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 90;
        dragCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

        Debug.Log("Spawned draggable for word: " + word);
        // AudioManager.Instance.MatchWithFunction(selectedContent[currentIndex].audio);
        AudioManager.Instance.MatchWithFunction(selectedContent[currentIndex].content);
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

        //nullreference error over here using audioSource.clip.length because audioSource.clip is null
        
        yield return new WaitForSeconds(AudioManager.Instance.audioSource.clip.length);
        AudioManager.Instance.PlayPositiveReinforcementSound();
        yield return new WaitForSeconds(AudioManager.Instance.audioSource.clip.length);
        currentIndex++;
        // Check if all words are done
        if (currentIndex >= selectedContent.Count)
        {
            Debug.Log(currentIndex);
            Debug.Log(selectedContent.Count);
            Debug.Log("All words matched!");
            selectedContent.Clear();
            StartCoroutine(SceneDelayLoad("Phrases", 1.5f));
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

        selectCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[index].content;
        // selectCard.GetComponentInChildren<Image>().sprite = null;
        selectCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 70;
        selectCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

        selectCard.onClick.AddListener(() => OnNameCardClicked(index, selectCard));

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

    void OnNameCardClicked(int index, Button sourceButton)
    {
        if (isNameAudioPlaying) return;
        Debug.Log("Name card clicked: " + selectedContent[index].content);
        StartCoroutine(PlayNameThenAdvance(index, sourceButton));
    }

    IEnumerator PlayNameThenAdvance(int index, Button sourceButton)
    {
        isNameAudioPlaying = true;
        if (sourceButton != null) sourceButton.interactable = false;


        // AudioManager.Instance.PlayGivenAudioDelayed(selectedContent[index].audio, 2.0f);
        AudioManager.Instance.WordAudioFunction(selectedContent[index].content);

        // float waitTime = 2.0f + (selectedContent[index].audio != null ? selectedContent[index].audio.length : 0f);
        float waitTime = 1.5f;
        yield return new WaitForSeconds(waitTime);

        isNameAudioPlaying = false;
        if (index + 1 < selectedContent.Count)
        {
            SetNameCard(index + 1);
        }
        else
        {
            Debug.Log("All Name cards shown!");
            selectedContent.Clear();
            StartCoroutine(SceneDelayLoad("Phrases", 1.5f));
        }
    }

    void SpawnSelectButtons(int batchStart, int batchEnd, int previousBatch, int currentBatch)
    {


        // int end = Mathf.Min(currentBatchStart + 4, selectedContent.Count);
        if (previousBatch != currentBatch)
        {

            ClearGrids();
            for (int i = batchStart; i < batchEnd; i++)
            {
                Button selectCard = Instantiate(selectButton, questionsGrid.transform);
                selectCard.gameObject.SetActive(true);
                SelectCard selectCardData = selectCard.GetComponent<SelectCard>();
                selectCardData.word = selectedContent[i].content;


                selectCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[i].content;
                // selectCard.GetComponentInChildren<Image>().sprite = null;
                selectCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 70;
                selectCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
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
        // AudioManager.Instance.ShowMeFunction(selectedContent[currentIndex].audio);
        AudioManager.Instance.ShowMeFunction(selectedContent[currentIndex].content);
        // AudioManager.Instance.WaitForCurrentAudio();
    }

    void OnSelectCardClicked(string selectedWord, Button sourceButton)
    {
        string correctWord = selectedContent[currentIndex].content;
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
        yield return new WaitForSeconds(AudioManager.Instance.audioSource.clip.length);
        AudioManager.Instance.PlayPositiveReinforcementSound();
        yield return new WaitForSeconds(AudioManager.Instance.audioSource.clip.length);
        questionsGrid.transform.GetChild(currentIndex % batchSize).GetComponent<Button>().interactable = false;
        int previousBatch = currentIndex / batchSize;
        currentIndex++;

        if (currentIndex >= selectedContent.Count)
        {
            Debug.Log("All words selected!");
            selectedContent.Clear();
            StartCoroutine(SceneDelayLoad("Phrases", 1.5f));
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
        selectedContextList.Clear();
        backButton.onClick.AddListener(() => SceneController.Instance.OpenLevelSelect("Phrases"));
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


