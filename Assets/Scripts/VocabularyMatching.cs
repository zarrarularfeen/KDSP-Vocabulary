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
    private List<ContentPicturePair> content = new List<ContentPicturePair>();
    public static List<ContentPicturePair> selectedContent = new List<ContentPicturePair>();
    [SerializeField] private GridLayoutGroup questionsGrid;
    [SerializeField] private GridLayoutGroup answersGrid;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject dragPrefab;
    [SerializeField] private Button selectButton;
    private int currentBatchStart = 0;
    private int currentIndex = 0;
    public static VocabularyMatching Instance;

    public static VocabularyMode currentMode = VocabularyMode.Match;

    void Awake()
    {
        Instance = this;
    } 
    void Start()
    {
        // content = ReadingBook.Instance.GetCurrentEnabledDictionary();
        // foreach (ContentPicturePair pair in content)
        // {
        //     Debug.Log(pair.content);
        // }

        Debug.Log("VocabularyMatching started with selectedContent: " + selectedContent.Count);
        
        foreach (ContentPicturePair pair in selectedContent)
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
                // Set content for Select mode
                SetContentSelect();
                break;
            case VocabularyMode.Name:
                // Set content for Name mode
                break;
        }
    }
    
    // Set up content for Matching mode and Calls relevant methods
    void SetContentMatch()
    {
        ClearGrids();
        currentBatchStart = 0;
        currentIndex = 0;

        SpawnNextBatch();
    }
    // Set up content for Select mode and Calls relevant methods
    void SetContentSelect()
    {
        // Implement Select mode content setup
        ClearGrids();
        SetSelectCard(0);

    }
    // Spawn next batch of 4 words in questions grid for Matching mode
    void SpawnNextBatch()
    {
        foreach (Transform child in questionsGrid.transform)
        {
            Destroy(child.gameObject);
        }

        int end = Mathf.Min(currentBatchStart + 4, selectedContent.Count);

        for (int i = currentBatchStart; i < end; i++)
        {
            GameObject target = Instantiate(targetPrefab, questionsGrid.transform);
            string word = selectedContent[i].content;
            target.GetComponentInChildren<TextMeshProUGUI>().text = word;
            DropTarget dropTarget = target.GetComponent<DropTarget>();
            dropTarget.word = word;
            target.GetComponentInChildren<TextMeshProUGUI>().fontSize = 36;
        }

        SpawnNextDraggable();
    }
    // Spawn next draggable card in answers grid for Matching mode
    public void SpawnNextDraggable()
    {
        if (currentIndex >= selectedContent.Count)
        {
            Debug.Log("All words matched!");
            SceneController.Instance.OpenLevelSelect("Vocabulary");
            return;
        }

        //if we have finished current batch of 4, move to next batch
        if (currentIndex >= currentBatchStart + 4)
        {
            currentBatchStart += 4;
            if (currentBatchStart >= selectedContent.Count)
            {
                Debug.Log("All batches done!");
                return;
            }
            SpawnNextBatch();
            return;
        }

        

        GameObject dragCard = Instantiate(dragPrefab, answersGrid.transform);
        string word = selectedContent[currentIndex].content;
        dragCard.GetComponentInChildren<TextMeshProUGUI>().text = word;
        DraggableCard draggable = dragCard.GetComponent<DraggableCard>();
        draggable.word = word;
        Debug.Log("Spawned draggable for word: " + word);
        dragCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 36;
    }
    // Called when a correct match is made
    public void OnCorrectMatch()
    {
        currentIndex++;
        SpawnNextDraggable();
    }

    void SetSelectCard(int index)
    {
        ClearGrids();
        Button selectCard = Instantiate(selectButton, questionsGrid.transform);
        selectCard.gameObject.SetActive(true);
        selectCard.GetComponentInChildren<TextMeshProUGUI>().text = selectedContent[index].content;
        selectCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 36;
        selectCard.onClick.AddListener(() => OnSelectCardClicked(index));

    }

    void OnSelectCardClicked(int index)
    {
        Debug.Log("Select card clicked: " + selectedContent[index].content);
        if (index + 1 < selectedContent.Count)
        {
            SetSelectCard(index + 1);
        }
        else
        {
            Debug.Log("All select cards shown!");
            SceneController.Instance.OpenLevelSelect("Vocabulary");
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
