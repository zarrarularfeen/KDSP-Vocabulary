using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using NUnit.Framework;
using Unity.VisualScripting;


public class WordsDisplay : MonoBehaviour
{

    private List<ContentPictureAudioTrio> content = new List<ContentPictureAudioTrio>();
    private TextMeshProUGUI wordText;
    [SerializeField] private Button wordButton;
    [SerializeField] private GridLayoutGroup wordsGrid;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        content = ReadingBook.Instance.GetCurrentEnabledDictionary();
        foreach (ContentPictureAudioTrio pair in content)
        {
            Debug.Log(pair.content);
        }

        DisplayWords();
    }
    // Update is called once per frame


    void DisplayWords()
    {
        foreach (ContentPictureAudioTrio pair in content)
        {
            Button newButton = Instantiate(wordButton, wordsGrid.transform);
            newButton.gameObject.SetActive(true);
            wordText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            wordText.text = pair.content;
            newButton.onClick.AddListener(() => OnWordButtonClicked(pair));
            Debug.Log("button added to grid: " + pair.content);
        }
    }

    void OnWordButtonClicked(ContentPictureAudioTrio pair)
    {
        Debug.Log("Word button clicked: " + pair.content);
        // Add your logic here for what happens when a word button is clicked


        if (!VocabularyMatching.selectedContent.Contains(pair))
        {
            VocabularyMatching.selectedContent.Add(pair);
            Debug.Log("Added to selectedContent: " + pair.content);
        }
        else
        {
            Debug.Log("Already in selectedContent: " + pair.content);
        }
    }

}
