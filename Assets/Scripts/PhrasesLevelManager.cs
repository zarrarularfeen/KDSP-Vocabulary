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
    public static List<ContentPictureAudioTrio> selectedContent = new List<ContentPictureAudioTrio>();
    [SerializeField] private GridLayoutGroup questionsGrid;
    [SerializeField] private GridLayoutGroup answersGrid;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject dragPrefab;
    [SerializeField] private Button selectButton;
    private int currentBatchStart = 0;
    private int currentIndex = 0;
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
                break;
            case PhrasesLevelMode.UnderstandSightWord:
                // Set content for Understand mode
                break;
            case PhrasesLevelMode.UnderstandPhrase:
                // Set content for Understand Phrase mode
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
            target.GetComponentInChildren<TextMeshProUGUI>().text = "";
            target.GetComponentInChildren<Image>().sprite = selectedContent[i].image;
            DropTarget dropTarget = target.GetComponent<DropTarget>();
            dropTarget.word = word;
            // target.GetComponentInChildren<TextMeshProUGUI>().fontSize = 36;
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
        dragCard.GetComponentInChildren<TextMeshProUGUI>().text = "";
        dragCard.GetComponentInChildren<Image>().sprite = selectedContent[currentIndex].image;
        DraggableCard draggable = dragCard.GetComponent<DraggableCard>();
        draggable.word = word;
        Debug.Log("Spawned draggable for word: " + word);
        // dragCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 36;
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
        selectCard.GetComponentInChildren<Image>().sprite = selectedContent[index].image;
        // selectCard.GetComponentInChildren<TextMeshProUGUI>().fontSize = 36;
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


