using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;


public class WordsDisplay : MonoBehaviour
{

    private List<ContentPicturePair> content = new List<ContentPicturePair>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        content = ReadingBook.Instance.GetCurrentEnabledDictionary();
        foreach (ContentPicturePair pair in content)
        {
            Debug.Log(pair.content);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
