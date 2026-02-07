using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System;

public class BatchSizeSetting : MonoBehaviour
{
    public static BatchSizeSetting Instance { get; private set; }
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject slider;
    public static Scenes nextScene;

    void Awake()
    {
        // Ensure only one instance of exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        Instance = this;
    }

    void Start()
    {
        slider.GetComponentInChildren<Slider>().onValueChanged.AddListener(SliderValueChanged);
        nextButton.onClick.AddListener(NextButton);
    }

    public void SetNextScene(Scenes scene)
    {
        nextScene = scene;
    }

    public void SliderValueChanged(float value)
    {
        slider.GetComponentInChildren<TextMeshProUGUI>().text = value.ToString();
        switch (nextScene)
        {
            case Scenes.VocabularyMatching:
                VocabularyMatching.batchSize = (int)value;
                break;

            case Scenes.PhrasesLevel:
                PhrasesLevelManager.batchSize = (int)value;
                break;

            case Scenes.SentencesLevel:
                SentencesLevelManager.batchSize = (int)value;
                break;

            default:
                Debug.Log("Unknown Scene");
                break;
        }
    }

    public void NextButton()
    {
        SceneController.Instance.OpenLevelSelect(nextScene);
    }

}