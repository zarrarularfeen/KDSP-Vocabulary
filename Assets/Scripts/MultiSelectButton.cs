using UnityEngine;
using TMPro;
using Unity.Properties;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System;
using System.Linq;

[System.Serializable]
public enum Managers
{
    ReadingBook,
    PhrasesManager,
    SentencesManager,
}

[System.Serializable]
public struct MSBButton
{
    public bool enabled;
    public Books book;
    public GameObject gameObj;
}

public class MultiSelectButton : MonoBehaviour
{
   
    [SerializeField] private List<MSBButton> buttonsList = new List<MSBButton>();
    [SerializeField] private Sprite deselectedImage;
    [SerializeField] private Sprite selectedImage;
    [SerializeField] private Managers selectedManager;
    [SerializeField] private Button nextButton;
    private int enabledCount = 0;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetAllDisabled();
        nextButton.interactable = false;
        enabledCount = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick(int i)
    {
        bool newEnabled = !buttonsList[i].enabled;
        SetEnable(buttonsList[i].book, newEnabled);
        SpriteSwap(newEnabled, i);
        MSBButton entry = new MSBButton
        {
            enabled = newEnabled,
            book = buttonsList[i].book,
            gameObj = buttonsList[i].gameObj
        };
        buttonsList[i] = entry;

        if (enabledCount > 0)
        {
            nextButton.interactable = true;
        }
        else
        {
            nextButton.interactable = false;
        }
    }

    public void SetEnable(Books book, bool enabled)
    {
        if (enabled)
        {
            enabledCount++;
        }
        else
        {
            enabledCount--;
        }

        switch (selectedManager)
        {
            case Managers.ReadingBook:
                ReadingBook.Instance.SetBookEnabled(book, enabled);
                break;

            case Managers.PhrasesManager:
                PhrasesManager.Instance.SetBookEnabled(book, enabled);
                break;

            case Managers.SentencesManager:
                SentencesManager.Instance.SetBookEnabled(book, enabled);
                break;
        }
    }

    public void SpriteSwap(bool enabled, int i)
    {
        if (enabled)
        {
            buttonsList[i].gameObj.GetComponent<Image>().sprite = selectedImage;
        }
        else
        {
            buttonsList[i].gameObj.GetComponent<Image>().sprite = deselectedImage;
        }
    }

    public void SetAllDisabled()
    {
        Debug.Log("SetAllDisabled called");

        for (int i = 0; i < buttonsList.Count; i++)
        {
            SetEnable(buttonsList[i].book, false);
            MSBButton entry = new MSBButton
            {
                enabled = false,
                book = buttonsList[i].book,
                gameObj = buttonsList[i].gameObj
            };

            buttonsList[i] = entry;
        }
    }
}
