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

    private int currentIndex = 0;
    private int batchSize = 4; // can be 1 to 4

    public static VocabularyMatching Instance { get; private set; }

    public static VocabularyMode currentMode = VocabularyMode.Match;

    private GameObject currentCard;

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
        // content = ReadingBook.Instance.GetCurrentEnabledDictionary();
        // foreach (ContentPictureAudioTrio pair in content)
        // {
        //     Debug.Log(pair.content);
        // }

        Debug.Log("VocabularyMatching started with selectedContent: " + selectedContent.Count);

        foreach (ContentPictureAudioTrio pair in selectedContent)
        {
            Debug.Log(pair.content);
        }
        SetContent(currentMode);
    }

    private Coroutine activeCoroutine;
    private OutlineGenerator outlineGenerator;
    // private OutlineGenerator outlineGeneratorTarget;
    // private OutlineGenerator outlineGeneratorButton;

    public void CreateOutline(Color color)
    {
        Debug.Log("CreateOutline called");

        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        activeCoroutine = StartCoroutine(ShowBorderCoroutine(color));
    }

    private IEnumerator ShowBorderCoroutine(Color color)
    {
        Debug.Log("ShowBorderCoroutine called");

        if (outlineGenerator == null)
        {
            Debug.LogError("OutlineGenerator component not found!");
            yield break;
        }

        outlineGenerator.GenerateBorder(color);
        yield return new WaitForSeconds(1f);
        outlineGenerator.DisableBorder();
        outlineGenerator.GenerateBorder(color);
        yield return new WaitForSeconds(1f);
        outlineGenerator.DisableBorder();
        outlineGenerator.GenerateBorder(color);
        yield return new WaitForSeconds(1f);
        outlineGenerator.DisableBorder();

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
                // Set content for Select mode
                SetContentSelect();
                break;
            case VocabularyMode.Name:
                // Set content for Name mode
                Debug.Log("Setting content for Name mode");
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
            target.GetComponentInChildren<TextMeshProUGUI>().text = "";
            target.GetComponentInChildren<Image>().sprite = selectedContent[i].image;
            DropTarget dropTarget = target.GetComponent<DropTarget>();
            dropTarget.word = word;
            outlineGenerator = target.GetComponent<OutlineGenerator>();
            outlineGenerator.GenerateBorder(Color.black);
            // target.GetComponentInChildren<TextMeshProUGUI>().fontSize = 36;
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
        dragCard.GetComponentInChildren<TextMeshProUGUI>().text = "";
        dragCard.GetComponentInChildren<Image>().sprite = selectedContent[currentIndex].image;
        DraggableCard draggable = dragCard.GetComponent<DraggableCard>();
        draggable.word = word;
        Debug.Log("Spawned draggable for word: " + word);
        // dragCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 36;

        currentCard = dragCard;
        outlineGenerator = currentCard.GetComponent<OutlineGenerator>();
        // CreateOutline(Color.green);
        outlineGenerator.GenerateBorder(Color.black);
    }

    // Called when a correct match is made
    public void OnCorrectMatch()
    {
        CreateOutline(Color.green);
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
        selectCard.GetComponentInChildren<Image>().sprite = selectedContent[index].image;
        outlineGenerator = selectCard.GetComponent<OutlineGenerator>();
        outlineGenerator.GenerateBorder(Color.black);
        // selectCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 36;
        selectCard.onClick.AddListener(() => OnNameCardClicked(index));
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

        // int end = Mathf.Min(currentBatchStart + 4, selectedContent.Count);

        for (int i = batchStart; i < batchEnd; i++)
        {
            Button selectCard = Instantiate(selectButton, questionsGrid.transform);
            selectCard.gameObject.SetActive(true);
            selectCard.GetComponentInChildren<Image>().sprite = selectedContent[i].image;
            SelectCard selectCardData = selectCard.GetComponent<SelectCard>();
            selectCardData.word = selectedContent[i].content;
            outlineGenerator = selectCard.GetComponent<OutlineGenerator>();
            outlineGenerator.GenerateBorder(Color.black);
            // Capture the current value of i
            selectCard.onClick.AddListener(() => OnSelectCardClicked(selectCardData.word));
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


}
