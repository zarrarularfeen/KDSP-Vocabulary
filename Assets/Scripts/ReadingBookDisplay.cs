using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class ReadingBookDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Image displayImage;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private List<ContentPictureAudioTrio> content = new List<ContentPictureAudioTrio>();
    private int currentidx = 0;
    private static Books requestedBook;

    void Awake()
    {

    }

    void Start()
    {
        content = ReadingBook.Instance.GetRequestedBook(requestedBook);
        nextButton.onClick.AddListener(Next);
        previousButton.onClick.AddListener(Previous);
        nextButton.gameObject.SetActive(false);
        previousButton.gameObject.SetActive(false);
        DisplayContent();
    }

    void Update()
    {

    }

    public static void SetRequestedBook(Books book)
    {
        requestedBook = book;
    }

    void Next()
    {
        currentidx++;
        DisplayContent();
    }

    void Previous()
    {
        currentidx--;
        DisplayContent();
    }

    void DisplayContent()
    {
        if (currentidx == 0 && content.Count > 1)
        {
            nextButton.gameObject.SetActive(true);
            previousButton.gameObject.SetActive(false);
        }
        else if (currentidx == content.Count - 1)
        {
            nextButton.gameObject.SetActive(false);
            previousButton.gameObject.SetActive(true);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
            previousButton.gameObject.SetActive(true);
        }

        ContentPictureAudioTrio currentContent = content[currentidx];
        displayText.text = currentContent.content;
        displayImage.sprite = currentContent.image;
        displayImage.SetNativeSize();
    }
}