using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System;
using JetBrains.Annotations;

public enum SentencesLevelMode
{
    MatchPicture,
    SelectPicture,
    NamePicture,
    MatchSightWord,
    SelectSightWord,
    ReadSightWord,
    MatchSightWordPicture,
    MatchSentencesPicture,
    ReadSentences,
    BuildSentences
}

public class SentencesLevelManager : MonoBehaviour
{
    public static List<SBEntry> selectedSentences = new List<SBEntry>();
    public static List<ContentPictureAudioTrio> selectedContent = new List<ContentPictureAudioTrio>();
    [SerializeField] private GridLayoutGroup questionsGrid;
    [SerializeField] private GridLayoutGroup answersGrid;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject dragPrefab;
    [SerializeField] private Button selectButton;
    [SerializeField] private GameObject FITBAnswerButton;
    [SerializeField] private GridLayoutGroup FITBAnswersGrid;
    [SerializeField] private TextMeshProUGUI FITBQuestionText;
    [SerializeField] private GameObject QuestionsBG;
    [SerializeField] private Button NameNextButton;
    [SerializeField] private Button NamePrevButton;
    [SerializeField] private Sprite correctSprite;
    [SerializeField] private Sprite wrongSprite;
    private bool isNameAudioPlaying = false;
    private readonly List<int> currentBatchOrder = new List<int>();

    private int currentIndex = 0;

    // private int batchSize = 4; // can be 1 to 4

    public static SentencesLevelManager Instance { get; private set; }
    public static SentencesLevelMode currentMode = SentencesLevelMode.MatchPicture;

    //Settings
    private static bool prs = true;
    public static int batchSize = 2;

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
        prs = SettingsMenu.GetBool("SentencesLevelPRS", true);

        selectedSentences = SentencesManager.Instance.GetCurrentEnabledDictionarySentences();

        if (currentMode == SentencesLevelMode.MatchSentencesPicture || currentMode == SentencesLevelMode.ReadSentences)
        {
            selectedContent.Clear();
            foreach (SBEntry entry in selectedSentences)
            {
                selectedContent.Add(entry.CPAT);
            }
        }
        else
        {
            selectedContent = SentencesManager.Instance.GetCurrentEnabledDictionarySightWords();
        }

        Debug.Log("SentencesLevel started with selectedContent: " + selectedContent.Count);

        foreach (ContentPictureAudioTrio pair in selectedContent)
        {
            Debug.Log(pair.content);
        }

        Debug.Log("SentencesLevel started with selectedSentences: " + selectedSentences.Count);

        foreach (SBEntry pair in selectedSentences)
        {
            Debug.Log(pair.CPAT.content);
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
                questionsGrid.cellSize = new Vector2(410, 410);
                questionsGrid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 280);
                break;

        }
        answersGrid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
    }

    public static bool PRS
    {
        get { return prs; }
        set { prs = value; }
    }

    public static void SetSentencesLevelMode(SentencesLevelMode mode)
    {
        currentMode = mode;
    }

    void SetContent(SentencesLevelMode mode)
    {
        switch (mode)
        {
            case SentencesLevelMode.MatchPicture:
                SetContentMatch();
                break;
            case SentencesLevelMode.SelectPicture:
                // Set content for Select mode
                SetContentSelect();
                break;
            case SentencesLevelMode.NamePicture:
                // Set content for Read mode
                SetContentName();
                break;
            case SentencesLevelMode.MatchSightWord:
                // Set content for Understand mode
                SetContentMatch();
                break;
            case SentencesLevelMode.SelectSightWord:
                // Set content for Understand Phrase mode
                SetContentSelect();
                break;
            case SentencesLevelMode.ReadSightWord:
                SetContentName();
                break;
            case SentencesLevelMode.MatchSightWordPicture:
                SetContentMatch();
                break;
            case SentencesLevelMode.MatchSentencesPicture:
                SetContentMatch();
                break;
            case SentencesLevelMode.BuildSentences:
                FITBAnswersGrid.gameObject.SetActive(true);
                FITBQuestionText.gameObject.SetActive(true);
                QuestionsBG.gameObject.SetActive(true);
                StartCoroutine(SetContentBuild());
                break;
            case SentencesLevelMode.ReadSentences:
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
        ClearGrids();
        currentIndex = 0;
        NameNextButton.gameObject.SetActive(true);
        NamePrevButton.gameObject.SetActive(true);

        if (currentMode == SentencesLevelMode.NamePicture ||
            currentMode == SentencesLevelMode.ReadSightWord ||
            currentMode == SentencesLevelMode.ReadSentences)
        {
            questionsGrid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);
        }

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
            DropTarget dropTarget = target.GetComponent<DropTarget>();
            dropTarget.word = word;
            dropTarget.targetPrefab = target;
            switch (currentMode)
            {
                case SentencesLevelMode.MatchPicture:
                    target.GetComponentInChildren<TextMeshProUGUI>().text = "";
                    target.GetComponentInChildren<Image>().sprite = selectedContent[i].image;
                    break;

                case SentencesLevelMode.MatchSightWord:
                    target.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[i].content;
                    target.GetComponentInChildren<TextMeshProUGUI>().fontSize = 90;
                    target.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                    break;

                case SentencesLevelMode.MatchSightWordPicture:
                    target.GetComponentInChildren<TextMeshProUGUI>().text = "";
                    target.GetComponentInChildren<Image>().sprite = selectedContent[i].image;
                    break;
                case SentencesLevelMode.MatchSentencesPicture:
                    target.GetComponentInChildren<TextMeshProUGUI>().text = "";
                    target.GetComponentInChildren<Image>().sprite = selectedContent[i].image;
                    break;
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
                    childRect.sizeDelta = new Vector2(800, 760);
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
        DraggableCard draggable = dragCard.GetComponent<DraggableCard>();
        draggable.word = word;

        switch (currentMode)
        {
            case SentencesLevelMode.MatchPicture:
                dragCard.GetComponentInChildren<TextMeshProUGUI>().text = "";
                dragCard.GetComponentInChildren<Image>().sprite = selectedContent[contentIndex].image;
                break;

            case SentencesLevelMode.MatchSightWord:
                dragCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[contentIndex].content;
                dragCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 90;
                dragCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                break;
            case SentencesLevelMode.MatchSightWordPicture:
                dragCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[contentIndex].content;
                // dragCard.GetComponentInChildren<Image>().sprite = null;
                dragCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 90;
                dragCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                break;
            case SentencesLevelMode.MatchSentencesPicture:
                dragCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[contentIndex].content;
                // dragCard.GetComponentInChildren<Image>().sprite = null;
                dragCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 60;
                dragCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                break;
        }
        // AudioManager.Instance.MatchWithFunction(selectedContent[contentIndex].audio);
        AudioManager.Instance.MatchWithFunction(selectedContent[contentIndex].content);
        AudioManager.Instance.WaitForCurrentAudio();
        Debug.Log("Spawned draggable for word: " + word);
        // dragCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 36;
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
        yield return new WaitForSeconds(1.25f);

        AudioManager.Instance.PlayPositiveReinforcementSound();
        yield return new WaitForSeconds(1.25f);

        currentIndex++;
        // Check if all words are done
        if (currentIndex >= selectedContent.Count)
        {
            Debug.Log("All words matched!");
            selectedContent.Clear();
            StartCoroutine(SceneDelayLoad("Sentences", 1.5f));
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
        selectCard.onClick.AddListener(() => OnNameCardClicked(index, selectCard));
        switch (currentMode)
        {
            case SentencesLevelMode.NamePicture:
                selectCard.GetComponentInChildren<TextMeshProUGUI>().text = "";
                selectCard.GetComponentInChildren<Image>().sprite = selectedContent[index].image;
                break;
            case SentencesLevelMode.ReadSightWord:
                selectCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[index].content;
                selectCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 70;
                selectCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                break;
            case SentencesLevelMode.ReadSentences:
                selectCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[index].content;
                selectCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 60;
                selectCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                break;
        }

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


    void OnNameCardClicked(int index, Button sourceButton)
    {
        Debug.Log("Name card clicked: " + selectedContent[index].content);
        AudioManager.Instance.WordAudioFunction(selectedContent[index].content);
    }

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

        if (previousBatch != currentBatch)
        {
            BuildCurrentBatchOrder(batchStart, batchEnd);
            ClearGrids();

            for (int i = batchStart; i < batchEnd; i++)
            {
                Button selectCard = Instantiate(selectButton, questionsGrid.transform);
                selectCard.gameObject.SetActive(true);
                SelectCard selectCardData = selectCard.GetComponent<SelectCard>();
                selectCardData.word = selectedContent[i].content;
                selectCard.onClick.AddListener(() => OnSelectCardClicked(selectCardData.word, selectCard));
                switch (currentMode)
                {
                    case SentencesLevelMode.SelectPicture:
                        selectCard.GetComponentInChildren<TextMeshProUGUI>().text = "";
                        selectCard.GetComponentInChildren<Image>().sprite = selectedContent[i].image;
                        // selectCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 90;
                        selectCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                        break;
                    case SentencesLevelMode.SelectSightWord:
                        selectCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[i].content;
                        selectCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 60;
                        selectCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                        break;


                }

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
        if (sourceButton != null)
        {
            sourceButton.interactable = false;
        }
        int previousBatch = currentIndex / batchSize;
        currentIndex++;
        if (currentIndex >= selectedContent.Count)
        {
            Debug.Log("All words selected!");
            selectedContent.Clear();
            StartCoroutine(SceneDelayLoad("sentences", 1.5f));
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

    IEnumerator SceneDelayLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneController.Instance.OpenLevelSelect(sceneName);
    }

    private List<List<string>> FITBoutput = new List<List<string>>();
    private string[] FITBcontentList;
    private int currIndex = 0;
    private int currFITBOutputListIndex = 0;
    private bool FITBCheck = false;

    public IEnumerator SetContentBuild()
    {
        ClearGrids();

        Debug.Log("SentencesLevel started with selectedSentences: " + selectedSentences.Count);

        foreach (SBEntry pair in selectedSentences)
        {
            Debug.Log(pair.CPAT.content);
        }

        foreach (SBEntry entry in selectedSentences)
        {
            Debug.Log("Current Sentence: " + entry.CPAT.content);

            FITBoutput.Clear();
            FITBcontentList = entry.CPAT.content.Split(' ');
            currFITBOutputListIndex = 0;

            foreach (BlankPositionGroup group in entry.blankPositions)
            {
                List<string> fitb = new List<string>();

                for (int i = 0; i < FITBcontentList.Length; i++)
                {
                    fitb.Add(group.positions.Contains(i) ? "_" : FITBcontentList[i]);
                }

                FITBoutput.Add(fitb);
            }

            foreach (List<string> lst in FITBoutput)
            {
                FITBCheck = false;
                SetFITBCards(lst, entry, FITBcontentList);
                currIndex = 0;
                // ✅ Proper wait
                yield return new WaitUntil(() => FITBCheck);
                // AudioManager.Instance.PlayGivenAudioNonDelayed(entry.CPAT.audio);
                AudioManager.Instance.SentencesAudioFunction(entry.CPAT.content, entry.CPAT.content);
                yield return new WaitForSeconds(4f);
            }
        }
    }

    public void SetFITBCards(List<string> lst, SBEntry entry, string[] contentList)
    {
        List<string> answers = new List<string>();
        for (int i = 0; i < contentList.Length; i++)
        {
            if (lst[i] == "_")
            {
                answers.Add(contentList[i]);
            }
        }

        SetFITBQuestion(lst);
        SetFITBAnswerGrid(answers, entry);
    }

    void SetFITBQuestion(List<string> lst)
    {
        FITBQuestionText.text = lst[0];
        for (int i = 1; i < lst.Count; i++)
        {
            FITBQuestionText.text = FITBQuestionText.text + " " + lst[i];
        }
    }

    void SetFITBAnswerGrid(List<string> answers, SBEntry entry)
    {
        int count = answers.Count;
        for (int i = 0; i < count - 1; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, count);
            string temp = answers[i];
            answers[i] = answers[randomIndex];
            answers[randomIndex] = temp;
        }

        foreach (string ans in answers)
        {
            GameObject button = Instantiate(FITBAnswerButton, FITBAnswersGrid.transform);
            button.SetActive(true);
            button.GetComponentInChildren<TextMeshProUGUI>().text = ans;
            button.GetComponent<Button>().onClick.AddListener(() => OnFITBButtonClicked(ans, button, entry));
        }
    }

    public void OnFITBButtonClicked(string ans, GameObject button, SBEntry entry)
    {
        if (FITBcontentList[entry.blankPositions[currFITBOutputListIndex].positions[currIndex]] == ans)
        {
            FITBoutput[currFITBOutputListIndex][entry.blankPositions[currFITBOutputListIndex].positions[currIndex]] = ans;
            currIndex += 1;
            SetFITBQuestion(FITBoutput[currFITBOutputListIndex]);
            Destroy(button);
        }

        FITBCheck = true;
        foreach (string s in FITBoutput[currFITBOutputListIndex])
        {
            if (s == "_")
            {
                FITBCheck = false;
            }
        }

        if (FITBCheck)
        {
            currFITBOutputListIndex += 1;
        }
    }


}


