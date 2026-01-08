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
    void Start()
    {
        Debug.Log("VocabularyMatching started with selectedContent: " + selectedContent.Count);

        foreach (ContentPictureAudioTrio pair in selectedContent)
        {
            Debug.Log(pair.content);
        }
        SetContent(currentMode);
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
            dropTarget.targetPrefab = target;
            
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

        AudioManager.Instance.MatchWithFunction(selectedContent[currentIndex].audio);
    }

    // Called when a correct match is made
    public void OnCorrectMatch(GameObject targetPrefab)
    {
        StartCoroutine(FeedBackFlicker(targetPrefab.transform.GetChild(1).GetComponent<Image>(), correctSprite, 0.2f, 3));
        AudioManager.Instance.PlayCorrectSound();   
        // CreateOutline(Color.green);
        currentIndex++;
        // Check if all words are done
        if (currentIndex >= selectedContent.Count)
        {
            Debug.Log("All words matched!");
            selectedContent.Clear();
            StartCoroutine(SceneDelayLoad("Home", 4.0f));
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

    public void OnIncorrectMatch(GameObject targetPrefab)
    {
        StartCoroutine(FeedBackFlicker(targetPrefab.transform.GetChild(1).GetComponent<Image>(), wrongSprite, 0.2f, 3));
        AudioManager.Instance.PlayWrongSound();
    }

    void SetNameCard(int index)
    {
        ClearGrids();
        Button selectCard = Instantiate(selectButton, questionsGrid.transform);
        selectCard.gameObject.SetActive(true);
        selectCard.GetComponentInChildren<Image>().sprite = selectedContent[index].image;
        selectCard.onClick.AddListener(() => OnNameCardClicked(index, selectCard));
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

        
        AudioManager.Instance.PlayGivenAudioDelayed(selectedContent[index].audio, 2.0f);

        float waitTime = 2.0f + (selectedContent[index].audio != null ? selectedContent[index].audio.length : 0f);
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
            StartCoroutine(SceneDelayLoad("Home", 4.0f));
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
                selectCard.GetComponentInChildren<Image>().sprite = selectedContent[i].image;
                SelectCard selectCardData = selectCard.GetComponent<SelectCard>();
                selectCardData.word = selectedContent[i].content;
                selectCard.onClick.AddListener(() => OnSelectCardClicked(selectCardData.word, selectCard));
            }
        }
        AudioManager.Instance.ShowMeFunction(selectedContent[currentIndex].audio);
    }

    void OnSelectCardClicked(string selectedWord, Button sourceButton)
    {
        string correctWord = selectedContent[currentIndex].content;
        if (selectedWord == correctWord)
        {
            Debug.Log("Correct selection for word: " + selectedWord);
            Debug.Log("Going to flicker correct sprite");
            StartCoroutine(FeedBackFlicker(sourceButton.transform.GetChild(1).GetComponent<Image>(), correctSprite, 0.2f, 3, sourceButton));
            Debug.Log("Played correct sound");
            AudioManager.Instance.PlayCorrectSound();
            
            
            int previousBatch = currentIndex / batchSize;

            currentIndex++;
            if (currentIndex >= selectedContent.Count)
            {
                Debug.Log("All words selected!");
                selectedContent.Clear();
                StartCoroutine(SceneDelayLoad("Home", 4.0f));
                return;
            }
            int currentBatch = currentIndex / batchSize;
            int batchStart = currentBatch * batchSize;
            int batchEnd = Mathf.Min(batchStart + batchSize, selectedContent.Count);
            //if we have finished current batch of 4, move to next batch
            SpawnSelectButtons(batchStart, batchEnd, previousBatch, currentBatch);
            
        }
        else
        {
            Debug.Log("Incorrect selection for word: " + selectedWord);
            
            StartCoroutine(FeedBackFlicker(sourceButton.transform.GetChild(1).GetComponent<Image>(), wrongSprite, 0.2f, 3, sourceButton));
            AudioManager.Instance.PlayWrongSound();
            
        }
    }

    IEnumerator FeedBackFlicker(Image image, Sprite feedbackSprite, float interval, int count, Button sourceButton = null) 
    {
        Sprite originalSprite = image.sprite;
        
        if (sourceButton != null)
        {
            sourceButton.interactable = false;
        }
        for (int i = 0; i < count; i++) 
        {
            image.sprite = feedbackSprite;
            yield return new WaitForSeconds(interval);
            image.sprite = originalSprite;
            yield return new WaitForSeconds(interval);
        }
        if (sourceButton != null)
        {
            sourceButton.interactable = true;
        }
        
            
    }

    IEnumerator SceneDelayLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneController.Instance.OpenLevelSelect(sceneName);
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
