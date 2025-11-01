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

    private List<ContentPicturePair> content = new List<ContentPicturePair>();
    private TextMeshProUGUI wordText;
    [SerializeField] private Button wordButton;
    [SerializeField] private GridLayoutGroup wordsGrid;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        content = ReadingBook.Instance.GetCurrentEnabledDictionary();
        foreach (ContentPicturePair pair in content)
        {
            Debug.Log(pair.content);
        }

        DisplayWords();
    }
    // Update is called once per frame


    void DisplayWords()
    {
        foreach (ContentPicturePair pair in content)
        {
            Button newButton = Instantiate(wordButton, wordsGrid.transform);
            newButton.gameObject.SetActive(true);
            wordText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            wordText.text = pair.content;
            Debug.Log("button added to grid: " + pair.content);
        }
    }

    // void setGrid()
    // {
    //     wordsGrid.GetComponent<re>
    // }
    
}
