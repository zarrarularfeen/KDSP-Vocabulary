using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class DropTarget : MonoBehaviour, IDropHandler
{
    [HideInInspector]public string word;
    [HideInInspector]public GameObject targetPrefab;
   

    public void OnDrop(PointerEventData eventData)
    {
        DraggableCard dragged  = eventData.pointerDrag.GetComponent<DraggableCard>();
        if (dragged == null)
        {
            return;
        }

        if (dragged.word == word)
        {
            Debug.Log("Correct match for word: " + word);
            Destroy(dragged.gameObject);
            if (WordsDisplay.currentGameMode == GameMode.Vocabulary)
            {
                Debug.Log("Calling VocabularyMatching OnCorrectMatch");
                VocabularyMatching.Instance.OnCorrectMatch(targetPrefab);
            }    
            else if (WordsDisplay.currentGameMode == GameMode.Phrases)
            {
                Debug.Log("Calling PhrasesLevelManager OnCorrectMatch");
                PhrasesLevelManager.Instance.OnCorrectMatch(targetPrefab);
            }
                
        }
        else
        {
            Debug.Log("Incorrect match for word: " + word);
            if (WordsDisplay.currentGameMode == GameMode.Vocabulary)
            {
                Debug.Log("Calling VocabularyMatching OnIncorrectMatch");
                VocabularyMatching.Instance.OnIncorrectMatch(targetPrefab);
            }    
            else if (WordsDisplay.currentGameMode == GameMode.Phrases)
            {
                Debug.Log("Calling PhrasesLevelManager OnIncorrectMatch");
                PhrasesLevelManager.Instance.OnIncorrectMatch(targetPrefab);
            }
            dragged.ResetPosition();
        }
    }
}
