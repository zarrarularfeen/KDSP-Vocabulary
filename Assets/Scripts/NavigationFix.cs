using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine.InputSystem.LowLevel;
public class NavigationFix : MonoBehaviour
{
   [SerializeField] private Button NextButton;

    void Start()
    {
        NextButton.onClick.AddListener(() => OnNextButtonClicked(NextButton));
    }

    void OnNextButtonClicked(Button button)
    {
        if (SentencesLevelManager.currentMode == SentencesLevelMode.NamePicture || SentencesLevelManager.currentMode == SentencesLevelMode.ReadSightWord || SentencesLevelManager.currentMode == SentencesLevelMode.ReadSentences)
                {
                    SceneController.Instance.OpenLevelSelect("SentencesLevel");
                }
                else
                {
                    SceneController.Instance.OpenBatchSizeSetting(Scenes.SentencesLevel);   
                }
    }
}
