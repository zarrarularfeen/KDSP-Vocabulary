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
    

    private int currentIndex = 0;
    private int batchSize = 4; // can be 1 to 4
    public static PhrasesLevelManager Instance { get; private set; }
    public static PhrasesLevelMode currentMode = PhrasesLevelMode.MatchSghtWord;

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
        AudioManager.Instance.MatchWithFunction(selectedContent[currentIndex].audio);    
        // dragCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 36;
    }
    // Called when a correct match is made
    public void OnCorrectMatch()
    {
        AudioManager.Instance.PlayCorrectSound();
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
        
        selectCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[index].content;
        // selectCard.GetComponentInChildren<Image>().sprite = null;
        selectCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 90;
        selectCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
        
        selectCard.onClick.AddListener(() => OnNameCardClicked(index));

    }

    void OnNameCardClicked(int index)
    {
        Debug.Log("Name card clicked: " + selectedContent[index].content);
        AudioManager.Instance.PlayGivenAudioDelayed(selectedContent[index].audio, 2.0f);
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
            SelectCard selectCardData = selectCard.GetComponent<SelectCard>();
            selectCardData.word = selectedContent[i].content;
            
            
            selectCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[i].content;
            // selectCard.GetComponentInChildren<Image>().sprite = null;
            selectCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 90;
            selectCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            selectCard.onClick.AddListener(() => OnSelectCardClicked(selectCardData.word));
            
        }
        AudioManager.Instance.ShowMeFunction(selectedContent[currentIndex].audio);
    }

    void OnSelectCardClicked(string selectedWord)
    {
        string correctWord = selectedContent[currentIndex].content;
        if (selectedWord == correctWord)
        {
            Debug.Log("Correct selection for word: " + selectedWord);
            AudioManager.Instance.PlayCorrectSound();
            questionsGrid.transform.GetChild(currentIndex % batchSize).GetComponent<Button>().interactable = false;
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
            AudioManager.Instance.PlayWrongSound();
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


