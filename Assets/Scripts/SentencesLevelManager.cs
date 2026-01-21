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

    private int currentIndex = 0;

    // private int batchSize = 4; // can be 1 to 4

    public static SentencesLevelManager Instance { get; private set; }
    public static SentencesLevelMode currentMode = SentencesLevelMode.MatchPicture;

    //Settings
    private static bool prs = true;
    private static int batchSize = 2;

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
        batchSize = PlayerPrefs.GetInt("SentencesLevelBS", 1);

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
    }

    public static bool PRS
    {
        get { return prs; }
        set { prs = value; }
    }

    public static int Condition
    {
        get { return batchSize; }
        set { batchSize = value; }
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

        SpawnSelectButtons(batchStart, batchEnd);
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
                    target.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[i].content;
                    target.GetComponentInChildren<Image>().sprite = selectedContent[i].image;
                    break;
                case SentencesLevelMode.MatchSentencesPicture:
                    target.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[i].content;
                    target.GetComponentInChildren<Image>().sprite = selectedContent[i].image;
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

        switch (currentMode)
        {
            case SentencesLevelMode.MatchPicture:
                dragCard.GetComponentInChildren<TextMeshProUGUI>().text = "";
                dragCard.GetComponentInChildren<Image>().sprite = selectedContent[currentIndex].image;
                break;

            case SentencesLevelMode.MatchSightWord:
                dragCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[currentIndex].content;
                dragCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 90;
                dragCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                break;
            case SentencesLevelMode.MatchSightWordPicture:
                dragCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[currentIndex].content;
                // dragCard.GetComponentInChildren<Image>().sprite = null;
                dragCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 90;
                dragCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                break;
            case SentencesLevelMode.MatchSentencesPicture:
                dragCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[currentIndex].content;
                // dragCard.GetComponentInChildren<Image>().sprite = null;
                dragCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 60;
                dragCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                break;
        }

        Debug.Log("Spawned draggable for word: " + word);
        // dragCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 36;
    }

    // Called when a correct match is made
    public void OnCorrectMatch()
    {
        currentIndex++;
        // Check if all words are done
        if (currentIndex >= selectedContent.Count)
        {
            Debug.Log("All words matched!");
            selectedContent.Clear();
            SceneController.Instance.OpenLevelSelect("Home");
            return;
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

    void SetNameCard(int index)
    {
        ClearGrids();
        Button selectCard = Instantiate(selectButton, questionsGrid.transform);
        selectCard.gameObject.SetActive(true);
        selectCard.onClick.AddListener(() => OnNameCardClicked(index));
        switch (currentMode)
        {
            case SentencesLevelMode.NamePicture:
                selectCard.GetComponentInChildren<TextMeshProUGUI>().text = "";
                selectCard.GetComponentInChildren<Image>().sprite = selectedContent[index].image;
                break;
            case SentencesLevelMode.ReadSightWord:
                selectCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[index].content;
                selectCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 90;
                selectCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                break;
            case SentencesLevelMode.ReadSentences:
                selectCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[index].content;
                selectCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 60;
                selectCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                break;
        }
    }

    void OnNameCardClicked(int index)
    {
        Debug.Log("Name card clicked: " + selectedContent[index].content);
        if (index + 1 < selectedContent.Count)
        {
            SetNameCard(index + 1);
        }
        else
        {
            Debug.Log("All Name cards shown!");
            selectedContent.Clear();
            SceneController.Instance.OpenLevelSelect("Home");
        }
    }

    void SpawnSelectButtons(int batchStart, int batchEnd)
    {
        ClearGrids();

        for (int i = batchStart; i < batchEnd; i++)
        {
            Button selectCard = Instantiate(selectButton, questionsGrid.transform);
            selectCard.gameObject.SetActive(true);
            SelectCard selectCardData = selectCard.GetComponent<SelectCard>();
            selectCardData.word = selectedContent[i].content;
            selectCard.onClick.AddListener(() => OnSelectCardClicked(selectCardData.word));
            switch (currentMode)
            {
                case SentencesLevelMode.SelectPicture:
                    selectCard.GetComponentInChildren<TextMeshProUGUI>().text = "";
                    selectCard.GetComponentInChildren<Image>().sprite = selectedContent[i].image;
                    selectCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 90;
                    selectCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                    break;
                case SentencesLevelMode.SelectSightWord:
                    selectCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[i].content;
                    selectCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 90;
                    selectCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                    break;


            }
        }
    }

    void OnSelectCardClicked(string selectedWord)
    {
        string correctWord = selectedContent[currentIndex].content;
        if (selectedWord == correctWord)
        {
            Debug.Log("Correct selection for word: " + selectedWord);
            currentIndex++;
            if (currentIndex >= selectedContent.Count)
            {
                Debug.Log("All words selected!");
                selectedContent.Clear();
                SceneController.Instance.OpenLevelSelect("Home");
                return;
            }
            int currentBatch = currentIndex / batchSize;
            int batchStart = currentBatch * batchSize;
            int batchEnd = Mathf.Min(batchStart + batchSize, selectedContent.Count);
            //if we have finished current batch of 4, move to next batch
            SpawnSelectButtons(batchStart, batchEnd);
        }
        else
        {
            Debug.Log("Incorrect selection for word: " + selectedWord);
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
                AudioManager.Instance.PlayGivenAudioNonDelayed(entry.CPAT.audio);
                yield return new WaitForSeconds(3f);
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


